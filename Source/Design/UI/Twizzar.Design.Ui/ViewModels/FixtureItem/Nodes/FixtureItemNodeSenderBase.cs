using System;
using System.Linq;
using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Command.Commands;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Keywords;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.Parser.SyntaxTree;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.NLog;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;
using ViCommon.EnsureHelper;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes
{
    /// <summary>
    /// Base class for sending information to the domain logic. Where the domain logic will be set by the implementation.
    /// </summary>
    public abstract class FixtureItemNodeSenderBase : IFixtureItemNodeSender
    {
        #region static fields and constants

        private static readonly IConfigurationSource UserInterfaceSource = new FromUserInterface();

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public virtual async Task UpdateMemberConfigAsync(
            IFixtureItemNode current,
            IMemberConfiguration memberConfig,
            IFixtureItemInformation fixtureItemInformation,
            Maybe<IFixtureItemNode> parent)
        {
            // When the member config is already up to date do nothing.
            if (memberConfig.Equals(fixtureItemInformation.MemberConfiguration))
            {
                return;
            }

            // if the fixture item link is a default link.
            if (fixtureItemInformation.Id.Name.IsNone && memberConfig is not UndefinedMemberConfiguration)
            {
                fixtureItemInformation =
                    await this.CreateFixtureItemIdAndUpdateParentAsync(current, fixtureItemInformation, parent);
            }

            await this.SendAsync(
                new ChangeMemberConfigurationCommand(
                    fixtureItemInformation.Id,
                    memberConfig));
        }

        /// <inheritdoc />
        public async Task ChangeValue(
            IFixtureItemNode current,
            IViToken token,
            IFixtureItemInformation fixtureItemInformation,
            Maybe<IFixtureItemNode> parent)
        {
            var config = this.ToConfig(token, fixtureItemInformation);

            if (config.Match(
                    configuration => configuration.Equals(fixtureItemInformation.MemberConfiguration),
                    _ => false))
            {
                return;
            }

            await config.DoAsync(
                configuration => this.UpdateMemberConfigAsync(
                    current,
                    configuration,
                    fixtureItemInformation,
                    parent),
                failure => Task.Run(() => this.Log(failure.Message, LogLevel.Error)));
        }

        /// <summary>
        /// Create fixture item id and update the parent.
        /// </summary>
        /// <param name="current">The current node.</param>
        /// <param name="fixtureItemInformation">The fixture item information.</param>
        /// <param name="parent">The parent node.</param>
        /// <returns>The updated fixture item information.</returns>
        protected async Task<IFixtureItemInformation> CreateFixtureItemIdAndUpdateParentAsync(
            IFixtureItemNode current,
            IFixtureItemInformation fixtureItemInformation,
            Maybe<IFixtureItemNode> parent)
        {
            try
            {
                var fixtureItemId = FixtureItemId.CreateNamed(
                        fixtureItemInformation.ParentPath,
                        fixtureItemInformation.Id.TypeFullName)
                    .WithRootItemPath(fixtureItemInformation.Id.RootItemPath);

                current.UpdateFixtureItemId(fixtureItemId);
                fixtureItemInformation = fixtureItemInformation.With(fixtureItemId);

                // Create the Fixture Item
                await this.SendAsync(new CreateFixtureItemCommand(fixtureItemInformation.Id));

                // Update parent fixture item if self is not a root member.
                if (parent.AsMaybeValue() is SomeValue<IFixtureItemNode> someParent)
                {
                    // update all siblings
                    someParent.Value.Children.Where(node => node != current)
                        .ForEach(
                            viewModel =>
                                viewModel.UpdateFixtureItemId(fixtureItemId));

                    var parentConfig = new LinkMemberConfiguration(
                        someParent.Value.FixtureItemInformation.MemberConfiguration.Name,
                        fixtureItemInformation.Id,
                        UserInterfaceSource);

                    await someParent.Value.CommitMemberConfig(parentConfig);
                }
            }
            catch (Exception e)
            {
                this.Log(e);
            }

            return fixtureItemInformation;
        }

        /// <summary>
        /// Send the update async to the domain logic.
        /// </summary>
        /// <param name="command">The command to send.</param>
        /// <typeparam name="TCommand">Type of the command to send.</typeparam>
        /// <returns>A task.</returns>
        protected abstract Task SendAsync<TCommand>(ICommand<TCommand> command)
            where TCommand : ICommand;

        /// <summary>
        /// Get the default configuration.
        /// </summary>
        /// <param name="fixtureItemInformation">The fixture information.</param>
        /// <returns>Success when there is a default config; else failure.</returns>
        protected abstract IResult<IMemberConfiguration, Failure> GetDefaultConfig(
            IFixtureItemInformation fixtureItemInformation);

        private Result<IMemberConfiguration, Failure> ToConfig(
            IViToken token,
            IFixtureItemInformation fixtureItemInformation) =>
            token switch
            {
                ViEmptyToken _ =>
                    this.GetDefaultConfig(fixtureItemInformation)
                        .MapSuccess(configuration => configuration.WithSource(new FromSystemDefault()))
                        .ToResult(),

                IViDefaultKeyword _ =>
                    this.GetDefaultConfig(fixtureItemInformation)
                        .MapSuccess(configuration => configuration.WithSource(new FromUserInterface()))
                        .ToResult(),

                IViCtorToken _ =>
                    new Failure("Ctor cannot be set."),

                IViInvalidToken viInvalidToken =>
                    new Failure(viInvalidToken.Message),

                IViNullKeywordToken _ =>
                    new NullValueMemberConfiguration(
                        fixtureItemInformation.MemberConfiguration.Name,
                        UserInterfaceSource),

                IViStringToken viStringToken =>
                    new ValueMemberConfiguration(
                        fixtureItemInformation.MemberConfiguration.Name,
                        viStringToken.Text,
                        UserInterfaceSource),

                IViEnumToken viEnumToken =>
                    new ValueMemberConfiguration(
                        fixtureItemInformation.MemberConfiguration.Name,
                        new EnumValue(
                            fixtureItemInformation.FixtureDescription.TypeFullName,
                            viEnumToken.EnumName),
                        UserInterfaceSource),

                IViUniqueKeywordToken _ =>
                    new UniqueValueMemberConfiguration(
                        fixtureItemInformation.MemberConfiguration.Name,
                        UserInterfaceSource),

                IViCharToken charToken =>
                    new ValueMemberConfiguration(
                        fixtureItemInformation.MemberConfiguration.Name,
                        charToken.Character,
                        UserInterfaceSource),

                IViNumericToken numberToken =>
                    new ValueMemberConfiguration(
                        fixtureItemInformation.MemberConfiguration.Name,
                        new SimpleLiteralValue(numberToken.NumericWithSuffix),
                        UserInterfaceSource),

                IViBoolToken boolToken =>
                    new ValueMemberConfiguration(
                        fixtureItemInformation.MemberConfiguration.Name,
                        new SimpleLiteralValue(boolToken.Boolean),
                        UserInterfaceSource),

                IViLinkToken linkToken =>
                    new LinkMemberConfiguration(
                        fixtureItemInformation.MemberConfiguration.Name,
                        FixtureItemId.Create(
                                fixtureItemInformation.Path,
                                linkToken.TypeToken.Match(
                                    typeToken => TypeFullName.CreateFromToken(typeToken.TypeFullNameToken),
                                    TypeFullName.Create(fixtureItemInformation.TypeFullName.FullName)))
                            .WithRootItemPath(fixtureItemInformation.Id.RootItemPath),
                        UserInterfaceSource),

                IViUndefinedKeywordToken _ =>
                    new UndefinedMemberConfiguration(
                        fixtureItemInformation.MemberConfiguration.Name,
                        fixtureItemInformation.TypeFullName,
                        UserInterfaceSource),

                _ => throw new ArgumentOutOfRangeException(nameof(token)),
            };

        #endregion
    }
}
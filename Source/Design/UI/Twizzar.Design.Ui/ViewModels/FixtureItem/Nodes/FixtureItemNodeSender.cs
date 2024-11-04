using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes
{
    /// <summary>
    /// Default node sender.
    /// </summary>
    public class FixtureItemNodeSender : FixtureItemNodeSenderBase
    {
        #region fields

        private readonly ICommandBus _commandBus;
        private readonly ISystemDefaultService _defaultService;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemNodeSender"/> class.
        /// </summary>
        /// <param name="commandBus"></param>
        /// <param name="defaultService"></param>
        public FixtureItemNodeSender(ICommandBus commandBus, ISystemDefaultService defaultService)
        {
            this.EnsureMany()
                .Parameter(commandBus, nameof(commandBus))
                .Parameter(defaultService, nameof(defaultService))
                .ThrowWhenNull();

            this._commandBus = commandBus;
            this._defaultService = defaultService;
        }

        #endregion

        #region members

        /// <summary>
        /// Get the default config for the specific member.
        /// </summary>
        /// <param name="fixtureItemInformation">The fixture information.</param>
        /// <returns>A new instance of <see cref="IMemberConfiguration"/> if success.</returns>
        protected override IResult<IMemberConfiguration, Failure> GetDefaultConfig(IFixtureItemInformation fixtureItemInformation) =>
            fixtureItemInformation switch
            {
                _ when fixtureItemInformation.FixtureDescription is IParameterDescription parameterDescription =>
                Result.Success<IMemberConfiguration, Failure>(
                    this._defaultService.GetDefaultConstructorParameterMemberConfigurationItem(
                        parameterDescription,
                        fixtureItemInformation.Id.RootItemPath)),

                _ when fixtureItemInformation.MemberConfiguration is CtorMemberConfiguration =>
                    Result.Failure<IMemberConfiguration, Failure>(
                        new Failure("No default for Constructor. Because the ctor is readonly.")),

                _ when fixtureItemInformation.FixtureDescription is IMemberDescription memberDescription =>
                    Result.Success<IMemberConfiguration, Failure>(
                        this._defaultService.GetDefaultMemberConfigurationItem(
                            memberDescription, fixtureItemInformation.Id.RootItemPath)),

                _ when fixtureItemInformation.FixtureDescription.IsBaseType =>
                    Result.Success<IMemberConfiguration, Failure>(
                        this._defaultService.GetBaseTypeMemberConfigurationItem(fixtureItemInformation.FixtureDescription)),

                _ =>
                    Result.Failure<IMemberConfiguration, Failure>(
                        new Failure($"Cannot determine default config.")),
            };

        /// <inheritdoc />
        protected override Task SendAsync<TCommand>(ICommand<TCommand> command) =>
            this._commandBus.SendAsync(command);

        #endregion
    }
}
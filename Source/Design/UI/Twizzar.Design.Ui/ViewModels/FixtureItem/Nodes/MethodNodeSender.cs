using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Command.Commands;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes
{
    /// <summary>
    /// The sender for a method node.
    /// </summary>
    public class MethodNodeSender : FixtureItemNodeSender
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodNodeSender"/> class.
        /// </summary>
        /// <param name="commandBus"></param>
        /// <param name="defaultService"></param>
        public MethodNodeSender(ICommandBus commandBus, ISystemDefaultService defaultService)
            : base(commandBus, defaultService)
        {
        }

        #endregion

        #region members

        /// <inheritdoc />
        public override async Task UpdateMemberConfigAsync(
            IFixtureItemNode current,
            IMemberConfiguration memberConfig,
            IFixtureItemInformation fixtureItemInformation,
            Maybe<IFixtureItemNode> parent)
        {
            this.EnsureMany()
                .Parameter(current, nameof(current))
                .Parameter(memberConfig, nameof(memberConfig))
                .Parameter(fixtureItemInformation, nameof(fixtureItemInformation))
                .ThrowWhenNull();

            if (memberConfig.Equals(fixtureItemInformation.MemberConfiguration))
            {
                return;
            }

            if (fixtureItemInformation.MemberConfiguration is MethodConfiguration methodConfiguration)
            {
                // if the fixture item link is a default link.
                if (fixtureItemInformation.Id.Name.IsNone &&
                    memberConfig is not UndefinedMemberConfiguration)
                {
                    fixtureItemInformation =
                        await this.CreateFixtureItemIdAndUpdateParentAsync(current, fixtureItemInformation, parent);
                }

                var newMethodConfiguration = methodConfiguration with { ReturnValue = memberConfig };

                await this.SendAsync(
                    new ChangeMemberConfigurationCommand(
                        fixtureItemInformation.Id,
                        newMethodConfiguration));
            }
            else
            {
                throw this.LogAndReturn(
                    new InternalException(
                        $"MemberConfiguration should be {nameof(MethodConfiguration)} and not {fixtureItemInformation.MemberConfiguration.GetType().Name}"));
            }
        }

        #endregion
    }
}
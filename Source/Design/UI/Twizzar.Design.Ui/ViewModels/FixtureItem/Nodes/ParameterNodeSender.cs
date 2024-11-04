using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using ViCommon.Functional.Monads.MaybeMonad;
using IFixtureItemNode = Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.IFixtureItemNode;

namespace Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes
{
    /// <summary>
    /// The sender for a parameter node.
    /// </summary>
    public class ParameterNodeSender : FixtureItemNodeSender
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterNodeSender"/> class.
        /// </summary>
        /// <param name="commandBus"></param>
        /// <param name="defaultService"></param>
        public ParameterNodeSender(ICommandBus commandBus, ISystemDefaultService defaultService)
            : base(commandBus, defaultService)
        {
        }

        #region Overrides of NodeSender

        /// <inheritdoc />
        public override async Task UpdateMemberConfigAsync(
            IFixtureItemNode current,
            IMemberConfiguration memberConfig,
            IFixtureItemInformation fixtureItemInformation,
            Maybe<IFixtureItemNode> parent)
        {
            if (memberConfig.Equals(fixtureItemInformation.MemberConfiguration))
            {
                return;
            }

            if (parent.AsMaybeValue() is SomeValue<IFixtureItemNode> someParent)
            {
                if (fixtureItemInformation.FixtureDescription is IParameterDescription parameterDescription)
                {
                    var parentInformation = someParent.Value.FixtureItemInformation;

                    if (parentInformation.MemberConfiguration is CtorMemberConfiguration ctorMemberConfiguration)
                    {
                        var newConfig = ctorMemberConfiguration.WithParameter(parameterDescription.Name, memberConfig);
                        await someParent.Value.CommitMemberConfig(newConfig);
                    }
                    else
                    {
                        throw this.LogAndReturn(
                            new InternalException(
                                $"The parent member configuration of a Parameter should be {nameof(CtorMemberConfiguration)} but is {parentInformation.MemberConfiguration.GetType().Name}"));
                    }
                }
                else
                {
                    throw this.LogAndReturn(
                        new InternalException(
                            $"The fixture description of the parameter should be {nameof(IParameterDescription)} but is {fixtureItemInformation?.FixtureDescription?.GetType()?.Name}"));
                }
            }
            else
            {
                throw this.LogAndReturn(
                    new InternalException(
                        $"The parameter node should have a parent."));
            }
        }

        #endregion
    }
}
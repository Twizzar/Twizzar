using System.Threading;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes
{
    /// <summary>
    /// <see cref="IFixtureItemNodeReceiver"/> for a constructor.
    /// </summary>
    public class CtorFixtureItemNodeReceiver : FixtureItemNodeReceiver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CtorFixtureItemNodeReceiver"/> class.
        /// </summary>
        /// <param name="fixtureItemNode"></param>
        /// <param name="isListening"></param>
        /// <param name="synchronizationContext"></param>
        public CtorFixtureItemNodeReceiver(
            IFixtureItemNode fixtureItemNode,
            bool isListening,
            SynchronizationContext synchronizationContext)
            : base(
                fixtureItemNode,
                isListening,
                synchronizationContext)
        {
        }

        #region Overrides of NodeRecevier

        /// <inheritdoc />
        protected override void OnMemberChanged(FixtureItemMemberChangedEvent e)
        {
            base.OnMemberChanged(e);

            if (this.FixtureItemInformation.MemberConfiguration is CtorMemberConfiguration ctorConfig)
            {
                foreach (var child in this.FixtureItemNode.Children)
                {
                    if (child.FixtureItemInformation.FixtureDescription is IParameterDescription parameterDescription)
                    {
                        var maybeNewConfig = ctorConfig.ConstructorParameters.GetMaybe(parameterDescription.Name);
                        maybeNewConfig.IfNone(() => this.Log($"Cannot find the member {parameterDescription.Name} in the constructor.", LogLevel.Debug));

                        if (maybeNewConfig.AsMaybeValue() is SomeValue<IMemberConfiguration> newConfig &&
                            newConfig.Value != child.FixtureItemInformation.MemberConfiguration)
                        {
                            child.RefreshFixtureInformation(child.FixtureItemInformation.With(newConfig.Value));
                        }
                    }
                }
            }
            else
            {
                throw this.LogAndReturn(
                    new InternalException(
                        $"The {nameof(this.FixtureItemInformation.MemberConfiguration)} of the ctor node should be of type {nameof(CtorMemberConfiguration)}"));
            }
        }

        #endregion
    }
}
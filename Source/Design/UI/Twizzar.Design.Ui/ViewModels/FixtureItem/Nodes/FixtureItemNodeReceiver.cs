using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes
{
    /// <inheritdoc cref="IFixtureItemNodeReceiver" />
    public class FixtureItemNodeReceiver : IFixtureItemNodeReceiver
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemNodeReceiver"/> class.
        /// </summary>
        /// <param name="fixtureItemNode"></param>
        /// <param name="isListening"></param>
        /// <param name="synchronizationContext"></param>
        public FixtureItemNodeReceiver(
            IFixtureItemNode fixtureItemNode,
            bool isListening,
            SynchronizationContext synchronizationContext)
        {
            this.EnsureMany()
                .Parameter(fixtureItemNode, nameof(fixtureItemNode))
                .ThrowWhenNull();

            this.FixtureItemNode = fixtureItemNode;
            this.IsListening = isListening;
            this.SynchronizationContext = Maybe.ToMaybe(synchronizationContext);
        }

        #endregion

        #region events

        /// <inheritdoc />
        public event Action<IFixtureItemInformation> FixtureInformationChanged;

        #endregion

        #region properties

        /// <summary>
        /// Gets the fixture information.
        /// </summary>
        public IFixtureItemInformation FixtureItemInformation => this.FixtureItemNode.FixtureItemInformation;

        /// <inheritdoc cref="IFixtureItemNodeReceiver"/>
        public bool IsListening { get; private set; }

        /// <inheritdoc cref="IFixtureItemNodeReceiver"/>
        public Maybe<SynchronizationContext> SynchronizationContext { get; }

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object is disposed.
        /// </summary>
        protected bool IsDisposed { get; set; } = false;

        /// <summary>
        /// Gets the associated fixture item node.
        /// </summary>
        protected IFixtureItemNode FixtureItemNode { get; }

        #endregion

        #region members

        /// <inheritdoc />
        public void Handle(FixtureItemMemberChangedFailedEvent e)
        {
            if (this.IsListening &&
                e.FixtureItemId == this.FixtureItemInformation.Id &&
                e.MemberConfiguration.Name == this.FixtureItemInformation.MemberConfiguration.Name)
            {
                this.OnMemberChangedFailed(e);
            }
        }

        /// <inheritdoc />
        public void Handle(FixtureItemMemberChangedEvent e)
        {
            if (this.IsListening &&
                e.FixtureItemId == this.FixtureItemInformation.Id &&
                e.MemberConfiguration.Name == this.FixtureItemInformation.MemberConfiguration.Name)
            {
                this.OnMemberChanged(e);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose all resources.
        /// </summary>
        /// <param name="disposing">Always true. see https://rules.sonarsource.com/csharp/RSPEC-3881. </param>
        protected virtual void Dispose(bool disposing)
        {
            this.GuardDisposed();
            this.IsListening = false;
            this.IsDisposed = true;
        }

        /// <summary>
        /// Check if the object is already disposes.
        /// </summary>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument", Justification = "Caller information is more usefull here.")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Suppression is needed.")]
        protected void GuardDisposed(
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            this.EnsureParameter(this.IsDisposed, nameof(this.IsDisposed))
                .IsFalse(b => b, _ => new ObjectDisposedException(nameof(FixtureItemNodeReceiver)))
                .ThrowOnFailure(callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// When a member change of this node has failed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMemberChangedFailed(FixtureItemMemberChangedFailedEvent e)
        {
            this.GuardDisposed();

            this.FixtureItemNode.DisplayOnMemberChangedFailed(e);
        }

        /// <summary>
        /// Called when this member configuration has changed.
        /// </summary>
        /// <param name="e">The event.</param>
        protected virtual void OnMemberChanged(FixtureItemMemberChangedEvent e)
        {
            this.GuardDisposed();

            if (this.FixtureItemInformation.MemberConfiguration.Equals(e.MemberConfiguration) &&
                !this.FixtureItemNode.NodeValueController.IsCommitDirty &&
                !(this.FixtureItemInformation.MemberConfiguration is CtorMemberConfiguration))
            {
                return;
            }

            var fixtureInformation = this.FixtureItemInformation.With(e.MemberConfiguration);
            this.FixtureInformationChanged?.Invoke(fixtureInformation);
        }

        #endregion
    }
}
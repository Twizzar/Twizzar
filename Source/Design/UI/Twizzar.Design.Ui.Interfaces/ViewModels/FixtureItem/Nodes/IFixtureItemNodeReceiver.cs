using System;
using System.Threading;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.CoreInterfaces;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes
{
    /// <summary>
    /// Listens to changes form the Domain Logic and provides a event.
    /// </summary>
    public interface IFixtureItemNodeReceiver :
        IEventListener<FixtureItemMemberChangedEvent>,
        IEventListener<FixtureItemMemberChangedFailedEvent>,
        IService,
        IDisposable
    {
        /// <summary>
        /// Event called when the fixture information has changed.
        /// </summary>
        event Action<IFixtureItemInformation> FixtureInformationChanged;

        /// <summary>
        /// Gets a value indicating whether the listener is listening.
        /// </summary>
        new bool IsListening { get; }

        /// <summary>
        /// Gets the synchronization context.
        /// </summary>
        new Maybe<SynchronizationContext> SynchronizationContext { get; }
    }
}
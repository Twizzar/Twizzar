using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.Command.Services
{
    /// <summary>
    /// Implementation of <see cref="IEventSourcingContainer"/> using auto fac.
    /// </summary>
    public class AutofacEventSourcingContainer : IEventSourcingContainer, IEventSourcingRegisterService
    {
        private readonly ILifetimeScope _scope;
        private readonly Dictionary<Type, HashSet<WeakReference>> _internalListeners =
            new();

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacEventSourcingContainer"/> class.
        /// </summary>
        /// <param name="scope">The autofac life time scope.</param>
        public AutofacEventSourcingContainer(ILifetimeScope scope)
        {
            this._scope = this.EnsureCtorParameterIsNotNull(scope, nameof(scope));
        }

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        /// <inheritdoc />
        public Maybe<ICommandHandler<TCommand>> GetCommandHandler<TCommand>()
            where TCommand : ICommand =>
            Maybe.ToMaybe(
                this._scope.Resolve<ICommandHandler<TCommand>>());

        /// <inheritdoc />
        public IEnumerable<IEventListener<TEvent>> GetEventListeners<TEvent>()
            where TEvent : IEvent
        {
            var internalListeners = this.RemoveZombies<TEvent>();
            return this._scope.Resolve<IEnumerable<IEventListener<TEvent>>>().Concat(internalListeners);
        }

        /// <inheritdoc />
        public void RegisterListener<TEvent>(IEventListener<TEvent> listener)
            where TEvent : IEvent
        {
            if (this._internalListeners.ContainsKey(typeof(TEvent)))
            {
                this._internalListeners[typeof(TEvent)].Add(new WeakReference(listener));
            }
            else
            {
                this._internalListeners.Add(
                    typeof(TEvent),
                    new HashSet<WeakReference>() { new WeakReference(listener) });
            }
        }

        /// <inheritdoc />
        public Maybe<IEventStoreToQueryCacheSynchronizer> GetEventQuerySynchronizer() =>
            Maybe.ToMaybe(
                this._scope.Resolve<IEventStoreToQueryCacheSynchronizer>());

        private IEnumerable<IEventListener<TEvent>> RemoveZombies<TEvent>()
            where TEvent : IEvent
        {
            if (!this._internalListeners.ContainsKey(typeof(TEvent)))
            {
                return Enumerable.Empty<IEventListener<TEvent>>();
            }

            this._internalListeners[typeof(TEvent)].RemoveWhere(reference => !reference.IsAlive);
            return this._internalListeners[typeof(TEvent)]
                .Select(reference => (IEventListener<TEvent>)reference.Target);
        }
    }
}

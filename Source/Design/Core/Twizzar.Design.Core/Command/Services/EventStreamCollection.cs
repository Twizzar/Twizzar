using System.Collections.Concurrent;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.CoreInterfaces.Resources;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Core.Command.Services
{
    /// <inheritdoc cref="IEventStreamCollection"/>
    public class EventStreamCollection : IEventStreamCollection
    {
        private const string GeneralEventsStreamKey = "GeneralEventsStream";

        private readonly ConcurrentDictionary<string, IEventStream> _eventStreams = new();

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        /// <inheritdoc />
        public void Add(EventMessage eventMessage)
        {
            this.EnsureParameter(eventMessage, nameof(eventMessage))
                .ThrowWhenNull();

            var key = GetStreamKey(eventMessage);

            var stream = this._eventStreams.GetOrAdd(key, new EventStream());
            stream.Add(eventMessage);
        }

        /// <inheritdoc />
        public Maybe<IEventStream> GetStream()
            => this.GetStream(GeneralEventsStreamKey);

        /// <inheritdoc />
        public Maybe<IEventStream> GetStream(string key)
        {
            return this._eventStreams.TryGetValue(key, out var stream)
                ? Maybe.Some(stream)
                : Maybe.None();
        }

        /// <inheritdoc />
        public void ClearStream(string key)
        {
            if (!this._eventStreams.ContainsKey(key))
                return;
            if (this._eventStreams.TryRemove(key, out var eventStream))
                eventStream.Clear();
            else
                this.Log("Failed to clear the event stream.", LogLevel.Fatal);
        }

        private static string GetStreamKey(EventMessage eventMessage) =>
            eventMessage.Event switch
            {
                IFixtureItemEvent fixtureItemEvent => fixtureItemEvent.FixtureItemId.RootItemPath
                    .SomeOrProvided(
                        () => throw new InternalException(
                            MessagesDesign.EventStreamCollection_InternalException_RootItemIdSet)),

                _ => GeneralEventsStreamKey,
            };
    }
}
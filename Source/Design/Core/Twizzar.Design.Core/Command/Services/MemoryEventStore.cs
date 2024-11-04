using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.CoreInterfaces.Resources;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional;
using ViCommon.Functional.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Core.Command.Services
{
    /// <summary>
    /// In memory implementation of <see cref="IEventStore"/>.
    /// </summary>
    public class MemoryEventStore : IEventStore
    {
        #region fields

        private readonly IEventStreamCollection _streamCollection;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryEventStore"/> class.
        /// </summary>
        /// <param name="streamCollection"></param>
        public MemoryEventStore(IEventStreamCollection streamCollection)
        {
            this._streamCollection = streamCollection ?? throw new ArgumentNullException(nameof(streamCollection));
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public Task Store(EventMessage e)
        {
            this.EnsureParameter(e, nameof(e)).ThrowWhenNull();

            this._streamCollection.Add(eventMessage: e);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task ClearAll(string rootItemId)
        {
            this._streamCollection.ClearStream(rootItemId);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<Maybe<TEvent>> FindLast<TEvent>()
            where TEvent : IEvent =>
            Task.FromResult(
                this._streamCollection
                    .GetStream()
                    .Bind(s => s.Last<TEvent>())
                    .Map(m => (TEvent)m.Event));

        /// <inheritdoc />
        public Task<Maybe<TEvent>> FindLast<TEvent>(FixtureItemId id)
            where TEvent : IEvent =>
            this.FindAll(id)
                .Map(
                    events => events
                        .OfType<TEvent>()
                        .LastOrNone());

        /// <inheritdoc />
        public Task<Maybe<TEvent>> FindLast<TEvent>(string rootItemPath)
            where TEvent : IEvent =>
            Task.FromResult(
                this._streamCollection.GetStream(rootItemPath)
                    .Bind(s => s.Last<TEvent>())
                    .Map(m => (TEvent)m.Event));

        /// <inheritdoc />
        public Task<IEnumerable<TEvent>> FindAll<TEvent>()
            where TEvent : IEvent =>
            Task.FromResult(
                this._streamCollection
                    .GetStream()
                    .Map(s => s.FindAll<TEvent>())
                    .SomeOrProvided(new List<TEvent>()));

        /// <inheritdoc />
        public Task<IEnumerable<IFixtureItemEvent>> FindAll(FixtureItemId id) =>
            id.RootItemPath
                .MapAsync(this.FindAll<IFixtureItemEvent>)
                .MapAsync(events => events.Where(e => e.FixtureItemId == id))
                .MatchAsync(
                    FunctionalCommon.Identity,
                    () =>
                        throw new InternalException(
                            MessagesDesign.EventStreamCollection_InternalException_RootItemIdSet));

        /// <inheritdoc />
        public Task<IEnumerable<TEvent>> FindAll<TEvent>(string rootItemId)
            where TEvent : IEvent =>
            this._streamCollection
                .GetStream(rootItemId)
                .MapAsync(Task.FromResult)
                .MatchAsync(
                    some: stream => stream.FindAll<TEvent>(),
                    none: Enumerable.Empty<TEvent>);

        #endregion
    }
}
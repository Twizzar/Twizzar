using System.Globalization;
using System.Threading.Tasks;

using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.FixtureItem.Definition;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.CoreInterfaces.Resources;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.NLog.Interfaces;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.ResultMonad;

using static ViCommon.Functional.FunctionalCommon;

namespace Twizzar.Design.Core.Command.FixtureItem.Definition
{
    /// <inheritdoc />
    public class FixtureItemDefinitionRepository : IFixtureItemDefinitionRepository
    {
        private readonly IEventStore _eventStore;
        private readonly IFixtureItemDefinitionQuery _fixtureItemDefinitionQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemDefinitionRepository"/> class.
        /// </summary>
        /// <param name="eventStore">The event store.</param>
        /// <param name="fixtureItemDefinitionQuery">The fixture item definition query.</param>
        public FixtureItemDefinitionRepository(IEventStore eventStore, IFixtureItemDefinitionQuery fixtureItemDefinitionQuery)
        {
            this.EnsureMany()
                .Parameter(eventStore, nameof(eventStore))
                .Parameter(fixtureItemDefinitionQuery, nameof(fixtureItemDefinitionQuery))
                .ThrowWhenNull();

            this._eventStore = eventStore;
            this._fixtureItemDefinitionQuery = fixtureItemDefinitionQuery;
        }

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region Implementation of IChangeMemberConfigurationAndCreateParentsService

        /// <inheritdoc />
        public async Task<IResult<IFixtureItemDefinitionNode, Failure>> CreateFixtureItem(
            FixtureItemId id)
        {
            if (!await this.FixtureItemExitsInEventStore(id))
            {
                var node = await this.RestoreDefinitionNode(id);
                await node.DoAsync(n => n.CreateNamedFixtureItem(), _ => Task.CompletedTask);

                return node;
            }

            return Result.Failure<IFixtureItemDefinitionNode, Failure>(new Failure(
                string.Format(
                    CultureInfo.InvariantCulture,
                    MessagesDesign.Fixture_Item_already_exists,
                    id.Name,
                    id.TypeFullName)));
        }

        /// <inheritdoc />
        public async Task<IResult<IFixtureItemDefinitionNode, Failure>> RestoreDefinitionNode(FixtureItemId id)
        {
            var nodeResult = await this._fixtureItemDefinitionQuery.GetDefinitionNode(id);
            var node = nodeResult.Match(f => throw new InternalException(f.Message));
            var events = await this._eventStore.FindAll(id);

            var result = node.Replay(events);

            return result.Map(unit => node, Identity);
        }

        /// <inheritdoc />
        public async Task<bool> FixtureItemExitsInEventStore(FixtureItemId id)
        {
            var createEvent = await this._eventStore.FindLast<FixtureItemCreatedEvent>(id);
            return createEvent.IsSome;
        }

        #endregion
    }
}

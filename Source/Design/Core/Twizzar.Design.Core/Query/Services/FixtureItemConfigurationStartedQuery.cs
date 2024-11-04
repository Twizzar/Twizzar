using System.Threading.Tasks;

using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.NLog.Interfaces;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Core.Query.Services
{
    /// <summary>
    /// Abstract query for getting the <see cref="FixtureItemConfigurationStartedEvent"/>.
    /// </summary>
    public abstract class FixtureItemConfigurationStartedQuery : IQuery
    {
        #region fields

        private readonly IEventStore _eventStore;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemConfigurationStartedQuery"/> class.
        /// </summary>
        /// <param name="eventStore"></param>
        protected FixtureItemConfigurationStartedQuery(IEventStore eventStore)
        {
            this.EnsureParameter(eventStore, nameof(eventStore)).ThrowWhenNull();

            this._eventStore = eventStore;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <summary>
        /// Get the <see cref="FixtureItemConfigurationStartedEvent"/>.
        /// </summary>
        /// <param name="rootItemPath"></param>
        /// <returns>
        /// When successful returns the <see cref="FixtureItemConfigurationStartedEvent"/> else a <see cref="Failure"/>.
        /// Returns a Failure when:
        /// - The rootItemPath is None.
        /// - The <see cref="FixtureItemConfigurationStartedEvent"/> was not found in the event stream.
        /// </returns>
        protected Task<IResult<FixtureItemConfigurationStartedEvent, Failure>>
            GetFixtureItemConfigurationStartedEventAsync(Maybe<string> rootItemPath) =>
            rootItemPath
                .ToResult(new Failure($"{nameof(rootItemPath)} is None"))
                .BindAsync(
                    path =>
                        this._eventStore.FindLast<FixtureItemConfigurationStartedEvent>(path)
                            .Map(maybe => maybe.ToResult(new Failure("Project name not set"))));

        #endregion
    }
}
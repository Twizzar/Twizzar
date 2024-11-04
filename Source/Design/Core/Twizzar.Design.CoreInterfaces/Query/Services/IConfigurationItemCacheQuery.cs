using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.Query.Services
{
    /// <summary>
    /// Get a partial <see cref="IConfigurationItem"/> from the cached user configurations.
    /// This service is synchronized with the <see cref="IEventBus"/>.
    /// </summary>
    public interface IConfigurationItemCacheQuery : IEventListener<FixtureItemConfigurationEndedEvent>, IEventStoreToQueryCacheSynchronizer, IQuery
    {
        /// <summary>
        /// Get the cache <see cref="IConfigurationItem"/> for a specific fixture item id.
        /// </summary>
        /// <param name="id">The fixture item id.</param>
        /// <returns>A partial configuration.</returns>
        Maybe<IConfigurationItem> GetCached(FixtureItemId id);
    }
}

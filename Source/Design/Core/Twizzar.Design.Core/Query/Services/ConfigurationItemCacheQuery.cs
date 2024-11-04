using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.FixtureConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Core.Query.Services
{
    /// <summary>
    /// Caching of fixture item events.
    /// </summary>
    public class ConfigurationItemCacheQuery : IConfigurationItemCacheQuery
    {
        private readonly Dictionary<FixtureItemId, IConfigurationItem> _cachedConfigurationItems = new();
        private readonly IConfigurationItemFactory _configurationItemFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationItemCacheQuery"/> class.
        /// </summary>
        /// <param name="configurationItemFactory">The configuration item factory.</param>
        public ConfigurationItemCacheQuery(IConfigurationItemFactory configurationItemFactory)
        {
            this._configurationItemFactory = this.EnsureCtorParameterIsNotNull(configurationItemFactory, nameof(configurationItemFactory));
        }

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region Implementation of IEventListener<in EventStoreCleared>

        /// <inheritdoc />
        public bool IsListening => true;

        /// <inheritdoc />
        public Maybe<SynchronizationContext> SynchronizationContext => Maybe.None();

        /// <inheritdoc />
        public void Handle(FixtureItemConfigurationEndedEvent e)
        {
            this.ClearCachedProject(e.RootFixtureItemPath);
        }

        #endregion

        #region Implementation of IEventQuerySynchronizer

        /// <inheritdoc />
        public void Synchronize(EventMessage message)
        {
            this.EnsureParameter(message, nameof(message)).ThrowWhenNull();

            switch (message.Event)
            {
                case FixtureItemMemberChangedEvent fixtureItemMemberChangedEvent:
                    this.Cache(fixtureItemMemberChangedEvent);
                    break;
                case FixtureItemCreatedEvent fixtureItemCreatedEvent:
                    this.Cache(fixtureItemCreatedEvent);
                    break;
            }
        }

        /// <inheritdoc />
        public Maybe<IConfigurationItem> GetCached(FixtureItemId id) =>
            this._cachedConfigurationItems.GetMaybe(id);

        #endregion

        private void Cache(FixtureItemMemberChangedEvent e)
        {
            var id = e.FixtureItemId;

            if (this._cachedConfigurationItems.ContainsKey(id))
            {
                var cachedConfig = this._cachedConfigurationItems[id];
                var memberConfigurations = cachedConfig.MemberConfigurations
                    .AddOrOverride(e.MemberConfiguration.Name, e.MemberConfiguration);

                this._cachedConfigurationItems[id] = cachedConfig.WithMemberConfigurations(memberConfigurations);
            }
            else
            {
                throw new InternalException($"{nameof(FixtureItemMemberChangedEvent)} should not be raised before {nameof(FixtureItemCreatedEvent)}");
            }
        }

        private void Cache(FixtureItemCreatedEvent e)
        {
            var id = e.FixtureItemId;
            if (this._cachedConfigurationItems.ContainsKey(id))
            {
                throw new InternalException($"{nameof(FixtureItemCreatedEvent)} should only be called once for id {id}");
            }
            else
            {
                var config = this._configurationItemFactory.CreateConfigurationItem(
                    id,
                    ImmutableDictionary<string, IFixtureConfiguration>.Empty,
                    ImmutableDictionary<string, IMemberConfiguration>.Empty,
                    ImmutableDictionary.Create<string, IImmutableList<object>>());

                this._cachedConfigurationItems.Add(id, config);
            }
        }

        /// <summary>
        /// Clear the cached elements for specific project name.
        /// </summary>
        /// <param name="rootItemPath">The root item path.</param>
        private void ClearCachedProject(Maybe<string> rootItemPath)
        {
            this._cachedConfigurationItems
                .Where(elem => elem.Key.RootItemPath == rootItemPath)
                .Select(elem => elem.Key)
                .ToList()
                .ForEach(id => this._cachedConfigurationItems.Remove(id));
        }
    }
}

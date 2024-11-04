using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Core.Query.Services
{
    /// <inheritdoc />
    public class ConfigurationItemQuery : IConfigurationItemQuery
    {
        private readonly ISystemDefaultService _systemDefaultService;
        private readonly IConfigurationItemCacheQuery _cachedConfigQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationItemQuery"/> class.
        /// </summary>
        /// <param name="systemDefaultService">The system default service.</param>
        /// <param name="cachedCache">The cached query.</param>
        public ConfigurationItemQuery(ISystemDefaultService systemDefaultService, IConfigurationItemCacheQuery cachedCache)
        {
            this._systemDefaultService = this.EnsureCtorParameterIsNotNull(systemDefaultService, nameof(systemDefaultService));
            this._cachedConfigQuery = this.EnsureCtorParameterIsNotNull(cachedCache, nameof(cachedCache));
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
        public Task<IConfigurationItem> GetConfigurationItem(FixtureItemId id, ITypeDescription typeDescription)
        {
            this.EnsureMany()
                .Parameter(id, nameof(id))
                .Parameter(typeDescription, nameof(typeDescription))
                .ThrowWhenNull();

            // Get the system default config for the type
            var systemDefault = this._systemDefaultService.GetDefaultConfigurationItem(
                typeDescription,
                id.RootItemPath)
                .Match(f =>
                    throw new InvalidTypeDescriptionException(f.Message));

            // Get maybe the user config for the type cached. Is None when nothing is cached/configured for this fixture item.
            var cached = this._cachedConfigQuery.GetCached(id);

            // Merge the cached into the system default. To get a full configured item.
            return Task.FromResult(systemDefault.Merge(cached)
                .Match(f =>
                    throw new InvalidConfigurationException(f.Message, f.ConfigurationItem)));
        }
    }
}

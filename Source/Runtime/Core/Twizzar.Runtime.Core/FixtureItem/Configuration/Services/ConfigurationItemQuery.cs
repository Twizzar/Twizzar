using System;
using System.Threading.Tasks;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.Failures;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Runtime.Core.FixtureItem.Configuration.Services
{
    /// <inheritdoc />
    public class ConfigurationItemQuery : IConfigurationItemQuery
    {
        #region fields

        private readonly IUserConfigurationQuery _userConfigurationQuery;
        private readonly ISystemDefaultService _systemDefaultService;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationItemQuery"/> class.
        /// </summary>
        /// <param name="userConfigurationQuery">Query for getting configurations set by the user.</param>
        /// <param name="systemDefaultService">Query for getting system default configurations.</param>
        public ConfigurationItemQuery(
            IUserConfigurationQuery userConfigurationQuery,
            ISystemDefaultService systemDefaultService)
        {
            this.EnsureMany()
                .Parameter(userConfigurationQuery, nameof(userConfigurationQuery))
                .Parameter(systemDefaultService, nameof(systemDefaultService))
                .ThrowWhenNull();

            this._userConfigurationQuery = userConfigurationQuery;
            this._systemDefaultService = systemDefaultService;
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
        public async Task<IConfigurationItem> GetConfigurationItem(
            FixtureItemId id,
            ITypeDescription typeDescription)
        {
            this.EnsureMany<object>()
                .Parameter(id, nameof(id))
                .Parameter(typeDescription, nameof(typeDescription))
                .ThrowWhenNull();

            var systemDefaultResult = this._systemDefaultService
                .GetDefaultConfigurationItem(typeDescription, id.RootItemPath);

            var userNamedConfig = await this._userConfigurationQuery.GetNamedConfig(id);

            var systemDefault = systemDefaultResult.AsResultValue() switch
            {
                SuccessValue<IConfigurationItem> c => c.Value,
                FailureValue<InvalidTypeDescriptionFailure> f => throw new InvalidTypeDescriptionException(
                    f.Value.Message),
                _ => throw new PatternErrorBuilder(nameof(systemDefaultResult))
                    .IsNotOneOf(nameof(IConfigurationItem), nameof(InvalidTypeDescriptionFailure)),
            };

            var result = systemDefault.Merge(userNamedConfig);

            return result.AsResultValue() switch
            {
                SuccessValue<IConfigurationItem> c => c.Value,
                FailureValue<InvalidConfigurationFailure> f =>
                    throw new InvalidConfigurationException(f.Value.Message, f.Value.ConfigurationItem),
                _ => throw new Exception($"Unexpected Failure occurred in result {result} at {id.ToPathString()}"),
            };
        }

        #endregion
    }
}
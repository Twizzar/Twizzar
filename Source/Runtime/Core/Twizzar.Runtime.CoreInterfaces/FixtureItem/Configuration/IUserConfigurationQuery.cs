using System.Threading.Tasks;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration
{
    /// <summary>
    /// Query for getting user named configurations and user default configurations.
    /// </summary>
    public interface IUserConfigurationQuery : IQuery
    {
        #region members

        /// <summary>
        /// Gets the named config to the <see cref="FixtureItemId"/>.
        /// </summary>
        /// <param name="id">The id for the config.</param>
        /// <returns>The <see cref="IConfigurationItem"/> or null if none exists.</returns>
        Task<Maybe<IConfigurationItem>> GetNamedConfig(FixtureItemId id);

        #endregion
    }
}
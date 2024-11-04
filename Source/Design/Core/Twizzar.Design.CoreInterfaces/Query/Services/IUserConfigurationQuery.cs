using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.Query.Services
{
    /// <summary>
    /// Query for getting user named configurations and user default configurations.
    /// </summary>
    public interface IUserConfigurationQuery : IQuery
    {
        /// <summary>
        /// Get all configurations for a specific project name.
        /// </summary>
        /// <param name="rootItemPath">The root item path.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A sequence of configuration items.</returns>
        Task<IEnumerable<IConfigurationItem>> GetAllAsync(Maybe<string> rootItemPath, CancellationToken cancellationToken);
    }
}
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <summary>
    /// Service which find <see cref="IConfigurationItem"/>s when the config symbol is provided.
    /// </summary>
    public interface IRoslynConfigurationItemReader : IService
    {
        /// <summary>
        /// Returns all found <see cref="IConfigurationItem"/>s.
        /// </summary>
        /// <param name="builderInformation">The symbol of the config class.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A dictionary with all found <see cref="IConfigurationItem"/>s where the key is the <see cref="FixtureItemId"/>.</returns>
        Task<IImmutableDictionary<FixtureItemId, IConfigurationItem>> ReadConfigurationItemsAsync(
            IBuilderInformation builderInformation,
            CancellationToken cancellationToken);
    }
}
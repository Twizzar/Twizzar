using System.Threading.Tasks;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;

namespace Twizzar.Design.CoreInterfaces.Command.FixtureItem.Config
{
    /// <summary>
    /// Service for writing <see cref="IMemberConfiguration"/> to an configuration class.
    /// </summary>
    public interface IRoslynConfigWriter : IService
    {
        /// <summary>
        /// Update the provided <see cref="IMemberConfiguration"/> in the configuration class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="memberConfiguration"></param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task UpdateConfigAsync(FixtureItemId id, IMemberConfiguration memberConfiguration);
    }
}
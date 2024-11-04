using System.Threading.Tasks;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration
{
    /// <summary>
    /// Provides the file paths for a specific project.
    /// </summary>
    /// <seealso cref="IService" />
    public interface IFilePathProvider : IService
    {
        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns>The path to the config file.</returns>
        Task<Maybe<string>> GetConfigFilePath(string projectName);
    }
}

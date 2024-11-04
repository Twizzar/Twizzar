using System.Threading.Tasks;

using Twizzar.SharedKernel.CoreInterfaces;

using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.Common.VisualStudio
{
    /// <summary>
    /// Manages modifications to a Visual Studio project.
    /// </summary>
    public interface IVsProjectManager : IService
    {
        /// <summary>
        /// Finds the name of the project. By providing a file of the project.
        /// </summary>
        /// <param name="projectFileName">Name of a file in the project.</param>
        /// <returns>The name of the project.</returns>
        Maybe<string> FindProjectName(string projectFileName);

        /// <summary>
        /// Gets the project path.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns>The path of the project if the project exists in the solution else None.</returns>
        Task<Maybe<string>> GetProjectPath(string projectName);

        /// <summary>
        /// Sets the given file dependent upon another one.
        /// </summary>
        /// <param name="childName">The file to nest.</param>
        /// <param name="parentName">The file to set dependent.</param>
        /// <returns>A Task.</returns>
        Task SetDependentUponAsync(string childName, string parentName);
    }
}

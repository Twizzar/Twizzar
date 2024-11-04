using Twizzar.Design.CoreInterfaces.Common.VisualStudio;

namespace Twizzar.Design.CoreInterfaces.Common.FixtureItem.Config
{
    /// <summary>
    /// Config file path provider for <see cref="IViProject"/>.
    /// </summary>
    public interface IViProjectFilePathProvider
    {
        /// <summary>
        /// Get the project file path by its <see cref="IViProject"/>.
        /// </summary>
        /// <param name="project"></param>
        /// <returns>The path to the config file.</returns>
        string GetConfigFilePath(IViProject project);
    }
}

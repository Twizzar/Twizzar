namespace Twizzar.Design.CoreInterfaces.Common.VisualStudio
{
    /// <summary>
    /// Cache for caching occurred VS events.
    /// </summary>
    public interface IVsEventCache
    {
        /// <summary>
        /// Check if all references of a project are already loaded.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns>True when they are already loaded; otherwise false also false when the project does not exists.</returns>
        bool AllReferencesAreLoaded(string projectName);
    }
}
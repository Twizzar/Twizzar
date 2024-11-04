namespace Twizzar.Design.CoreInterfaces.Common.VisualStudio
{
    /// <summary>
    /// Register method for the event cache.
    /// </summary>
    public interface IVsEventCacheRegistrant
    {
        /// <summary>
        /// Register that are all events are loaded for a project.
        /// </summary>
        /// <param name="project">The project.</param>
        void RegisterAllReferencesLoaded(IViProject project);

        /// <summary>
        /// Register that a project is unloaded.
        /// </summary>
        /// <param name="project">The project.</param>
        void RegisterProjectUnloaded(IViProject project);
    }
}
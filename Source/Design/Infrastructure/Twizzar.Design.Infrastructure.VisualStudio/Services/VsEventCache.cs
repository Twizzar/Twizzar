using System.Collections.Generic;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;

namespace Twizzar.Design.Infrastructure.VisualStudio.Services
{
    /// <inheritdoc cref="IVsEventCache" />
    public sealed class VsEventCache : IVsEventCache, IVsEventCacheRegistrant
    {
        #region fields

        private readonly HashSet<string> _projectReferencesLoaded = new();

        #endregion

        #region members

        /// <inheritdoc />
        public bool AllReferencesAreLoaded(string projectName) =>
            this._projectReferencesLoaded.Contains(projectName);

        #endregion

        #region Implementation of IVsEventCacheRegistrant

        /// <inheritdoc />
        public void RegisterAllReferencesLoaded(IViProject project)
        {
            this._projectReferencesLoaded.Add(project.Name);
        }

        /// <inheritdoc />
        public void RegisterProjectUnloaded(IViProject project)
        {
            this._projectReferencesLoaded.Remove(project.Name);
        }

        #endregion
    }
}
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.CoreInterfaces.Common.Messaging.Events
{
    /// <summary>
    /// Event called when a project is renamed.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ProjectRenamedEvent : ValueObject, IUiEvent
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectRenamedEvent"/> class.
        /// </summary>
        /// <param name="oldProject"></param>
        /// <param name="newProject"></param>
        public ProjectRenamedEvent(IViProject oldProject, IViProject newProject)
        {
            this.OldProject = oldProject;
            this.NewProject = newProject;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the old project name.
        /// </summary>
        public IViProject OldProject { get; }

        /// <summary>
        /// Gets the new project name.
        /// </summary>
        public IViProject NewProject { get; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.OldProject;
            yield return this.NewProject;
        }

        #endregion
    }
}
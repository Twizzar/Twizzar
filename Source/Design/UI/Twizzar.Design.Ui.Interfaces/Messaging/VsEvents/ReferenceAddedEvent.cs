using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.Ui.Interfaces.Messaging.VsEvents
{
    /// <summary>
    /// Event called when a project reference was added after the initial load of the project.
    /// </summary>
    /// <seealso cref="ValueObject" />
    /// <seealso cref="IUiEvent" />
    [ExcludeFromCodeCoverage]
    public class ReferenceAddedEvent : ValueObject, IUiEvent
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceAddedEvent"/> class.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="project">The project.</param>
        public ReferenceAddedEvent(IViReference reference, IViProject project)
        {
            this.Reference = reference;
            this.Project = project;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the reference.
        /// </summary>
        public IViReference Reference { get; }

        /// <summary>
        /// Gets the project.
        /// </summary>
        public IViProject Project { get; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Reference;
            yield return this.Project;
        }

        #endregion
    }
}
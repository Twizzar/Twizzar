using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.Ui.Interfaces.Messaging.VsEvents
{
    /// <summary>
    /// Event raised after an project is opened.
    /// </summary>
    /// <seealso cref="ValueObject" />
    [ExcludeFromCodeCoverage]
    public class AfterOpenProjectEvent : ValueObject, IUiEvent
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="AfterOpenProjectEvent"/> class.
        /// </summary>
        /// <param name="project">The project.</param>
        public AfterOpenProjectEvent(IViProject project)
        {
            this.Project = project ?? throw new ArgumentNullException(nameof(project));
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the name of the project.
        /// </summary>
        public IViProject Project { get; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Project;
        }

        #endregion
    }
}
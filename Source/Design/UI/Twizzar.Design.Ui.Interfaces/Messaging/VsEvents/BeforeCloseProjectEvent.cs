using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.Ui.Interfaces.Messaging.VsEvents
{
    /// <summary>Event before a project is closed. </summary>
    /// <seealso cref="ValueObject" />
    /// <seealso cref="IUiEvent" />
    [ExcludeFromCodeCoverage]
    public class BeforeCloseProjectEvent : ValueObject, IUiEvent
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="BeforeCloseProjectEvent"/> class.
        /// </summary>
        /// <param name="project">The project.</param>
        public BeforeCloseProjectEvent(IViProject project)
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
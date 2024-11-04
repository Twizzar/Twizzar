using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.SharedKernel.CoreInterfaces;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Infrastructure.VisualStudio.Models
{
    /// <inheritdoc cref="IViProject" />
    [ExcludeFromCodeCoverage] // dependent on VsSDK
    public class VsProject : Entity<VsProject, string>, IViProject
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="VsProject"/> class.
        /// </summary>
        /// <param name="dteProject">The DTE project.</param>
        /// <exception cref="ArgumentNullException">dteProject.</exception>
        public VsProject(Project dteProject)
            : base(dteProject.UniqueName)
        {
            this.EnsureParameter(dteProject, nameof(dteProject)).ThrowWhenNull();
            ThreadHelper.ThrowIfNotOnUIThread();

            this.Name = dteProject.Name;
            this.FullName = dteProject.FullName;
            this.ProjectDirectory = Path.GetDirectoryName(dteProject.FullName);
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string FullName { get; }

        /// <inheritdoc />
        public string ProjectDirectory { get; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override bool Equals(string a, string b) =>
            a == b;

        #endregion
    }
}
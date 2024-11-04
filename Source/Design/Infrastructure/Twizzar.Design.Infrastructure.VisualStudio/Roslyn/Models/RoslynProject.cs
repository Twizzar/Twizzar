using System.IO;
using Microsoft.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn.Models
{
    /// <summary>
    /// Implementation of <see cref="IViProject"/> for a roslyn <see cref="Project"/>.
    /// </summary>
    public class RoslynProject : IViProject
    {
        private readonly Project _project;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynProject"/> class.
        /// </summary>
        /// <param name="project"></param>
        public RoslynProject(Project project)
        {
            EnsureHelper.GetDefault.Parameter(project, nameof(project)).ThrowWhenNull();

            this._project = project;
        }

        #region Implementation of IViProject

        /// <inheritdoc />
        public string Name => this._project.GetFileNameWithoutExtension();

        /// <inheritdoc />
        public string FullName => this._project.FilePath;

        /// <inheritdoc />
        public string ProjectDirectory => Path.GetDirectoryName(this._project.FilePath);

        #endregion
    }
}
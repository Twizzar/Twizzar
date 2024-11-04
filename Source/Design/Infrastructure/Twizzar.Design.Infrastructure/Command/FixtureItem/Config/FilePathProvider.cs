using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Common.FixtureItem.Config;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.Command.FixtureItem.Config
{
    /// <summary>
    /// Implements <see cref="IFilePathProvider"/> and <see cref="IViProjectFilePathProvider"/>.
    /// This service is the only service which knows the name of a config file.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FilePathProvider : IFilePathProvider, IViProjectFilePathProvider
    {
#if INTERNAL
        private const string DefinitionFileNamePostfix = ".internal.fixture.yaml";
#else
        private const string DefinitionFileNamePostfix = ".fixture.yaml";
#endif

        private readonly IVsProjectManager _vsProjectManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePathProvider"/> class.
        /// </summary>
        /// <param name="vsProjectManager">The vs project manager service.</param>
        public FilePathProvider(IVsProjectManager vsProjectManager)
        {
            this._vsProjectManager = this.EnsureCtorParameterIsNotNull(vsProjectManager, nameof(vsProjectManager));
        }

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public async Task<Maybe<string>> GetConfigFilePath(string projectName)
        {
            var path = await this._vsProjectManager.GetProjectPath(projectName);

            return path.Map(
                p =>
                    p + @"\" + projectName + DefinitionFileNamePostfix);
        }

        /// <inheritdoc />
        public string GetConfigFilePath(IViProject project) =>
            Path.Combine(project.ProjectDirectory, project.Name + DefinitionFileNamePostfix);
    }
}
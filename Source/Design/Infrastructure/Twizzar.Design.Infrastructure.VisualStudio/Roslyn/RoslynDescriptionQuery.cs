using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Common.FixtureItem.Description.Services;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces.Roslyn;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.Design.Shared.Infrastructure.VisualStudio2019.Name;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <inheritdoc cref="ITypeDescriptionQuery" />
    public class RoslynDescriptionQuery : ITypeDescriptionQuery
    {
        #region fields

        private readonly Workspace _workspace;
        private readonly IRoslynDescriptionFactory _descriptionFactory;
        private readonly IProjectNameQuery _projectNameQuery;
        private readonly ITypeSymbolQuery _typeSymbolQuery;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynDescriptionQuery"/> class.
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="descriptionFactory"></param>
        /// <param name="projectNameQuery"></param>
        /// <param name="typeSymbolQuery"></param>
        public RoslynDescriptionQuery(
            Workspace workspace,
            IRoslynDescriptionFactory descriptionFactory,
            IProjectNameQuery projectNameQuery,
            ITypeSymbolQuery typeSymbolQuery)
        {
            this.EnsureMany()
                .Parameter(workspace, nameof(workspace))
                .Parameter(descriptionFactory, nameof(descriptionFactory))
                .Parameter(typeSymbolQuery, nameof(typeSymbolQuery))
                .ThrowWhenNull();

            this._workspace = workspace;
            this._descriptionFactory = descriptionFactory;
            this._projectNameQuery = projectNameQuery;
            this._typeSymbolQuery = typeSymbolQuery;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public async Task<IResult<ITypeDescription, Failure>> GetTypeDescriptionAsync(ITypeFullName typeFullName, Maybe<string> rootItemPath)
        {
            this.EnsureParameter(typeFullName, nameof(typeFullName)).ThrowWhenNull();

            return
                await
                    this._projectNameQuery.GetProjectNameAsync(rootItemPath)
                        .BindAsync(
                            projectName => this._workspace.CurrentSolution.GetProjectByName(projectName)
                                .ToResult(new Failure("Project name not found in the current solution"))
                                .BindAsync(project => this.GetTypeDescriptionAsync(project, typeFullName)));
        }

        /// <summary>
        /// Resolves the references of the project and provide a list of all DLLs.
        /// </summary>
        /// <param name="projectName">The project name.</param>
        /// <returns>A list with the referenced DLLs.</returns>
        public Task<IResult<IEnumerable<string>, Failure>> GetReferencedAssembliesAsync(string projectName)
        {
            this.EnsureParameter(projectName, nameof(projectName)).ThrowWhenNull();

            return from project in this._workspace.CurrentSolution.GetProjectByName(projectName)
                    .ToResult(new Failure("Project name not found in the current solution"))
                from compilation in GetCompilationAsync(project)
                select compilation.References
                    .OfType<PortableExecutableReference>()
                    .Select(reference => reference.FilePath);
        }

        private Task<IResult<ITypeDescription, Failure>> GetTypeDescriptionAsync(
            Project project,
            ITypeFullName typeFullName) =>
                from compilation in GetCompilationAsync(project)
                from typeSymbol in this.GetTypeSymbol(typeFullName, compilation)
                select this._descriptionFactory.CreateDescription(typeSymbol);

        private IResult<ITypeSymbol, Failure> GetTypeSymbol(ITypeFullName typeFullName, Compilation compilation) =>
            typeFullName.Cast().Token switch
            {
                SymbolTypeFullNameToken x => Result.Success<ITypeSymbol, Failure>(x.Symbol),
                _ => this._typeSymbolQuery.GetTypeSymbol(compilation, typeFullName),
            };

        private static async Task<IResult<Compilation, Failure>> GetCompilationAsync(Project project) =>
            ToMaybe(await project.GetCompilationAsync())
                .ToResult(new Failure($"Cannot get compilation for the project {project.Name}"));

        #endregion
    }
}
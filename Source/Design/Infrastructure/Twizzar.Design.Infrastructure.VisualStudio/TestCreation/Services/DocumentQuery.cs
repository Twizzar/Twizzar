using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;

using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace TestCreation.Services;

/// <summary>
/// Service for creating or retrieving a document.
/// </summary>
public class DocumentQuery : ProgressUpdater, IDocumentQuery
{
    #region fields

    private readonly Workspace _workspace;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentQuery"/> class.
    /// </summary>
    /// <param name="workspace">The current workspace for code generation.</param>
    public DocumentQuery(Workspace workspace)
    {
        EnsureHelper.GetDefault
            .Many()
            .Parameter(workspace, nameof(workspace))
            .ThrowWhenNull();

        this._workspace = workspace;
    }

    #endregion

    #region properties

    /// <inheritdoc />
    public override int NumberOfProgressSteps => 1;

    #endregion

    #region members

    /// <inheritdoc />
    public Task<Maybe<CreationContext>> GetOrCreateDocumentAsync(
        CreationInfo destination,
        CreationContext sourceContext)
    {
        EnsureHelper.GetDefault
            .Many()
            .Parameter(destination, nameof(destination))
            .Parameter(sourceContext, nameof(sourceContext))
            .ThrowWhenNull();

        var project = this._workspace.CurrentSolution.Projects
            .FirstOrDefault(p => p.FilePath == destination.Project);

        if (project is null)
        {
            return Task.FromResult(Maybe.None<CreationContext>());
        }

        var document = GetDocument(project, destination)
            .SomeOrProvided(() =>
            {
                this.Report($"Document {destination.File} not found it will be created.");
                var (prefix, fileName, extension) = destination.File.SplitPath();
                var relativePath = prefix.ReplaceSafe(destination.Project.SplitPath().Prefix, string.Empty)
                    .TrimEnd('/', '\\');
                var folders = string.IsNullOrEmpty(relativePath.Trim()) ? null : relativePath.Split('/', '\\');
                return project.AddDocument($"{fileName}{extension}", string.Empty, folders: folders);
            });

        return Maybe.SomeAsync(CreateCreationContext(destination, sourceContext, document));
    }

    private static CreationContext CreateCreationContext(
        CreationInfo destination,
        CreationContext sourceContext,
        Document document) =>
        new(
            destination,
            sourceContext.SourceMember,
            sourceContext.SourceType,
            new CodeAnalysisContext(document),
            Maybe.None());

    private static Maybe<Document> GetDocument(Project project, CreationInfo destination) =>
        project.Documents.FirstOrNone(d => d.FilePath == destination.File);

    #endregion
}
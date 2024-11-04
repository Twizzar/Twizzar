using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Common.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Extensions;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio2019.Roslyn;

/// <inheritdoc cref="ICompilationTypeCache"/>
public class CompilationTypeCache : ICompilationTypeCache
{
    #region fields

    private readonly Workspace _workspace;
    private readonly IRoslynDescriptionFactory _descriptionFactory;

    private readonly Dictionary<string, IReadOnlyList<ITypeDescription>> _cache = new();
    private readonly Dictionary<string, Compilation> _compilationCache = new();

    private readonly DefaultDictionary<string, CancellationTokenSource> _cancellationTokenSources =
        new(s => new CancellationTokenSource());

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="CompilationTypeCache"/> class.
    /// </summary>
    /// <param name="workspace"></param>
    /// <param name="descriptionFactory"></param>
    public CompilationTypeCache(Workspace workspace, IRoslynDescriptionFactory descriptionFactory)
    {
        this.EnsureMany()
            .Parameter(workspace, nameof(workspace))
            .Parameter(descriptionFactory, nameof(descriptionFactory))
            .ThrowWhenNull();

        this._workspace = workspace;
        this._descriptionFactory = descriptionFactory;

        this._workspace.WorkspaceChanged += this.WorkspaceOnWorkspaceChanged;
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
    public void Initialize()
    {
        Task.Run(this.InitializeAsync);
    }

    /// <inheritdoc />
    public IReadOnlyList<ITypeDescription> GetAllTypeForAssembly(string assemblyName) =>
        this._compilationCache.GetMaybe(assemblyName)
            .Match(
                compilation => this.GetAllTypes(compilation),
                () => new List<ITypeDescription>());

    /// <summary>
    /// Initialize the service async.
    /// </summary>
    /// <returns></returns>
    public async Task InitializeAsync()
    {
        try
        {
            foreach (var project in this._workspace.CurrentSolution.Projects
                         .Where(project => project.HasTwizzarAnalyzer()))
            {
                var compilation = await project.GetCompilationAsync();

                if (compilation?.AssemblyName is not null)
                {
                    this.RenewCache(compilation);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // ignore
        }
        catch (Exception exception)
        {
            this.Log(exception);
        }
    }

    private void WorkspaceOnWorkspaceChanged(object sender, WorkspaceChangeEventArgs e)
    {
        if (e.ProjectId is null)
        {
            return;
        }

        var project = e.NewSolution.GetProject(e.ProjectId);

        if (project is null)
        {
            return;
        }

        Task.Run(() => this.UpdateCacheAsync(e, project));
    }

    private CancellationToken CancelAndRenew(Compilation compilation)
    {
        this._cancellationTokenSources[compilation.AssemblyName].Cancel();
        this._cancellationTokenSources[compilation.AssemblyName] = new CancellationTokenSource();
        return this._cancellationTokenSources[compilation.AssemblyName].Token;
    }

    private async Task UpdateCacheAsync(WorkspaceChangeEventArgs e, Project project)
    {
        if (!project.HasTwizzarAnalyzer())
        {
            return;
        }

        var compilation = await project.GetCompilationAsync();

        if (compilation?.AssemblyName is null)
        {
            return;
        }

        try
        {
            switch (e.Kind)
            {
                case WorkspaceChangeKind.SolutionCleared:
                case WorkspaceChangeKind.SolutionRemoved:
                case WorkspaceChangeKind.SolutionChanged:
                    this._cache.Clear();
                    this._compilationCache.Clear();
                    break;
                case WorkspaceChangeKind.ProjectAdded:
                case WorkspaceChangeKind.ProjectChanged:
                {
                    this.RenewCache(compilation);
                    break;
                }
                case WorkspaceChangeKind.ProjectRemoved:
                    this._cache.Remove(compilation.AssemblyName);
                    this._compilationCache.Remove(compilation.AssemblyName);
                    break;
                default:
                    await this.UpdateCacheForDocumentAsync(e, compilation);
                    break;
            }
        }
        catch (OperationCanceledException)
        {
            // ignore
        }
        catch (Exception exp)
        {
            this.Log(exp);
        }
    }

    private async Task UpdateCacheForDocumentAsync(WorkspaceChangeEventArgs e, Compilation compilation)
    {
        if (!this._compilationCache.ContainsKey(compilation.Assembly.Name))
        {
            return;
        }

        var oldCompilation = this._compilationCache[compilation.Assembly.Name];

        if (compilation == oldCompilation || compilation.GetHashCode() == oldCompilation.GetHashCode())
        {
            return;
        }

        var oldDoc = e.OldSolution.GetDocument(e.DocumentId);
        var newDoc = e.NewSolution.GetDocument(e.DocumentId);

        if (oldDoc is null || newDoc is null)
        {
            return;
        }

        var changes = await newDoc.GetTextChangesAsync(oldDoc);

        if (changes.Any(change => !string.IsNullOrEmpty(change.NewText?.Trim())))
        {
            this.RenewCache(compilation);
        }
    }

    private void RenewCache(Compilation compilation)
    {
        var token = this.CancelAndRenew(compilation);
        this._compilationCache[compilation.Assembly.Name] = compilation;
        Task.Run(
            () =>
            {
                this._cache[compilation.Assembly.Name] = this.GetAllTypesUncached(compilation, token);
            },
            token).Forget();
    }

    private IReadOnlyList<ITypeDescription> GetAllTypes(
        Compilation compilation,
        CancellationToken cancellationToken = default)
    {
        if (this._cache.TryGetValue(compilation.Assembly.Name, out var cachedResult))
        {
            return cachedResult;
        }

        return this.GetAllTypesUncached(compilation, cancellationToken);
    }

    private IReadOnlyList<ITypeDescription> GetAllTypesUncached(
        Compilation compilation,
        CancellationToken cancellationToken) =>
        compilation.GetAllTypes(cancellationToken)
            .Select(symbol => this._descriptionFactory.CreateDescription(symbol))
            .ToList();

    #endregion
}
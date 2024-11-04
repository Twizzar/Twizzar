using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

/// <inheritdoc cref="ITemplateFile" />
public class TemplateFile : ITemplateFile
{
    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateFile"/> class.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="snippets"></param>
    public TemplateFile(string path, IEnumerable<ITemplateSnippet> snippets)
        : this(path, ToSnippetsDictionary(snippets))
    {
    }

    private TemplateFile(string path, IImmutableDictionary<SnippetType, IReadOnlyList<ITemplateSnippet>> snippets)
    {
        EnsureHelper.GetDefault.Many()
            .Parameter(path, nameof(path))
            .Parameter(snippets, nameof(snippets))
            .ThrowWhenNull();

        this.Path = path;
        this.Snippets = snippets;
    }

    #endregion

    #region properties

    /// <inheritdoc/>
    public string Path { get; init; }

    /// <inheritdoc/>
    public IImmutableDictionary<SnippetType, IReadOnlyList<ITemplateSnippet>> Snippets { get; }

    #endregion

    #region members

    /// <inheritdoc />
    public ITemplateSnippet GetSingleSnipped(SnippetType type) => this.GetSingle(type);

    /// <inheritdoc />
    public ITemplateFile WithBackupFile(ITemplateFile templateFile)
    {
        var snippets = templateFile.Snippets
            .Merge(this.Snippets);

        return new TemplateFile(this.Path, snippets);
    }

    private static ImmutableDictionary<SnippetType, IReadOnlyList<ITemplateSnippet>> ToSnippetsDictionary(
        IEnumerable<ITemplateSnippet> snippets) =>
        snippets
            .Append(TemplateSnippet.Create(SnippetType.Arrange))
            .Append(TemplateSnippet.Create(SnippetType.ArgumentArrange))
            .Append(TemplateSnippet.Create(SnippetType.Act))
            .Append(TemplateSnippet.Create(SnippetType.MethodSignature))
            .GroupBy(snippet => snippet.Type)
            .ToImmutableDictionary(
                grouping => grouping.Key,
                grouping => (IReadOnlyList<ITemplateSnippet>)grouping.ToList());

    private ITemplateSnippet GetSingle(SnippetType type) =>
        this.Snippets.GetMaybe(type).SomeOrProvided(new List<ITemplateSnippet>()) switch
        {
            { Count: 1 } x => x.Single(),
            { Count: > 1 } => throw new TemplateSnippetDuplicateException(type),
            _ => throw new TemplateSnippetNotFoundException(type.ToTag()),
        };

    #endregion
}
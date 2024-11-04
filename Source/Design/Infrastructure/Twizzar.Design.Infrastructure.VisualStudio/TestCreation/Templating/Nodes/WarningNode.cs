using System;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

/// <summary>
/// Node which will display a warning in the generated code.
/// </summary>
public class WarningNode : SnippetNode
{
    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="WarningNode"/> class.
    /// </summary>
    /// <param name="snippet"></param>
    /// <param name="context"></param>
    /// <param name="parent"></param>
    public WarningNode(ITemplateSnippet snippet, ITemplateContext context, Maybe<ISnippetNode> parent)
        : base(snippet, context, parent)
    {
    }

    #endregion

    #region members

    /// <inheritdoc />
    public override string GetCode() => this.Snippet.Content;

    /// <summary>
    /// Create a new <see cref="WarningNode"/>.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    internal static WarningNode Create(ISnippetNode node) =>
        new(
            new TemplateSnippet(CreateTag(), GetWarning(node), SnippetType.Warning),
            node.Context,
            Maybe.None());

    private static string GetWarning(ISnippetNode node)
    {
        return $@"
/* !!! WARNING !!! WARNING !!! WARNING !!! WARNING !!! WARNING !!! WARNING !!! WARNING !!!
 *
 * Loop detected during code generation from template
 * Review the template file and fix any issues:
 * ${node.File.Path}
 *
 * Encountered issue in template {node.Parent.Match(n => n.Snippet.TagUsage, string.Empty)}
 *
 * Usage of tag {node.Snippet.TagUsage} causes an infinite loop.
 *
 * !!! WARNING !!!  WARNING !!! WARNING !!! WARNING !!! WARNING !!! WARNING !!! WARNING !!!*/";
    }

    private static string CreateTag() => $"{nameof(WarningNode)}-{Guid.NewGuid()}";

    #endregion
}
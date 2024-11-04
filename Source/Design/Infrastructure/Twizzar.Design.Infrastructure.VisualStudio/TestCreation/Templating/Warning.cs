using System;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

public class WarningNode : SnippedNode
{
    #region ctors

    /// <inheritdoc />
    public WarningNode(ITemplateSnipped snipped, ITemplateContext templateFile, Maybe<SnippedNode> parent)
        : base(snipped, templateFile, parent)
    {
    }

    #endregion

    #region members

    internal static WarningNode Create(SnippedNode node) =>
        new(
            new TemplateSnipped(CreateTag(), GetWarning(node), SnippedType.Warning),
            node.Context,
            Maybe.None());

    private static string GetWarning(SnippedNode node)
    {
        return $@"
/* !!! WARNING !!! WARNING !!! WARNING !!! WARNING !!! WARNING !!! WARNING !!! WARNING !!!
 *
 * Loop detected during code generation from template
 * Review the template file and fix any issues:
 * ${node.File.Path}
 *
 * Encountered issue in template {node.Parent.Match(n => n.Snipped.TagUsage, string.Empty)}
 *
 * Usage of tag {node.Snipped.TagUsage} causes an infinite loop.
 *
 * !!! WARNING !!!  WARNING !!! WARNING !!! WARNING !!! WARNING !!! WARNING !!! WARNING !!!*/";
    }

    private static string CreateTag() => $"{nameof(WarningNode)}-{Guid.NewGuid()}";

    #endregion
}
using System.Collections.Generic;
using System.Linq;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

/// <summary>
/// Default implementation of the <see cref="ISnippetNode"/>.
/// </summary>
public class SnippetNode : ISnippetNode
{
    #region fields

    private readonly List<ISnippetNode> _children = new();

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="SnippetNode"/> class.
    /// </summary>
    /// <param name="snippet"></param>
    /// <param name="context"></param>
    /// <param name="parent"></param>
    public SnippetNode(
        ITemplateSnippet snippet,
        ITemplateContext context,
        Maybe<ISnippetNode> parent)
    {
        EnsureHelper.GetDefault.Many()
            .Parameter(snippet, nameof(snippet))
            .Parameter(context, nameof(context))
            .ThrowWhenNull();

        this.Snippet = snippet;
        this.Parent = parent;
        this.File = context.File;
        this.Parent.IfSome(p => p.Add(this));
        this.Context = context;
    }

    #endregion

    #region properties

    /// <inheritdoc />
    public ITemplateContext Context { get; }

    /// <inheritdoc />
    public Maybe<ISnippetNode> Parent { get; private set; }

    /// <inheritdoc />
    public ITemplateSnippet Snippet { get; protected set; }

    /// <inheritdoc />
    public ITemplateFile File { get; }

    /// <inheritdoc />
    public bool IsCyclic
    {
        get
        {
            var current = this.Parent;
            var maxRecursionCount = 50;

            while (current.IsSome && --maxRecursionCount >= 0)
            {
                var node = current.GetValueUnsafe();

                if (node.Snippet == this.Snippet)
                {
                    return true;
                }

                current = node.Parent;
            }

            return false;
        }
    }

    /// <inheritdoc />
    public string Content => this.IsCyclic ? this.GetWarning() : this.GetCode();

    #endregion

    #region members

    /// <inheritdoc />
    public virtual string GetCode() =>
        this._children.Aggregate(this.Snippet.Content, ReplaceTag);

    /// <inheritdoc />
    public void Add(ISnippetNode child)
    {
        if (child is SnippetNode { Parent.IsNone: true } snippetNode)
        {
            snippetNode.Parent = this;
        }

        this._children.Add(child);
    }

    private static string ReplaceTag(string current, ISnippetNode child) =>
        current.Replace(child.Snippet.TagUsage, child.Content.TrimEnd('\n', '\r'));

    private string GetWarning()
    {
        var warning = WarningNode.Create(this);
        return warning.Content;
    }

    #endregion
}
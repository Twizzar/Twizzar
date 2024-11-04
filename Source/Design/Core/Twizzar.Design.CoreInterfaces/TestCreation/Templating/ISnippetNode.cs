using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.TestCreation.Templating;

/// <summary>
/// Represents a snipped node in the template graph.
/// </summary>
public interface ISnippetNode
{
    #region properties

    /// <summary>
    /// Gets the parent node, node if this is the root.
    /// </summary>
    Maybe<ISnippetNode> Parent { get; }

    /// <summary>
    /// Gets the snipped.
    /// </summary>
    ITemplateSnippet Snippet { get; }

    /// <summary>
    /// Gets the template context.
    /// </summary>
    ITemplateContext Context { get; }

    /// <summary>
    /// Gets the file.
    /// </summary>
    ITemplateFile File { get; }

    /// <summary>
    /// Gets a value indicating whether this node creates a cycle in the graph.
    /// </summary>
    bool IsCyclic { get; }

    /// <summary>
    /// Gets the content of the node,
    /// this will return a warning if <see cref="IsCyclic"/> else this will return <see cref="GetCode"/>.
    /// </summary>
    string Content { get; }

    #endregion

    #region members

    /// <summary>
    /// Gets the content of this node.
    /// This will gets the content of all children and replace the respected tags with the content of the child.
    /// </summary>
    /// <returns></returns>
    string GetCode();

    /// <summary>
    /// Add a child to this node.
    /// </summary>
    /// <param name="child"></param>
    void Add(ISnippetNode child);

    #endregion
}
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.TestCreation.Templating;

/// <summary>
/// Factory for creating a <see cref="ITemplateSnippet"/>.
/// </summary>
public interface ISnippetNodeFactory
{
    /// <summary>
    /// Create the <see cref="ITemplateSnippet"/>.
    /// </summary>
    /// <param name="snippet"></param>
    /// <param name="context"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    ISnippetNode Create(ITemplateSnippet snippet, ITemplateContext context, Maybe<ISnippetNode> parent);
}
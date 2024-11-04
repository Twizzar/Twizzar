using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

/// <summary>
/// Node for the arrange tag.
/// </summary>
public class ArrangeNode : SnippetNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArrangeNode"/> class.
    /// </summary>
    /// <param name="snippet"></param>
    /// <param name="context"></param>
    /// <param name="parent"></param>
    public ArrangeNode(ITemplateSnippet snippet, ITemplateContext context, Maybe<ISnippetNode> parent)
        : base(snippet, context, parent)
    {
        this.Snippet = snippet.WithContent(GetContent(this.Context.SourceCreationContext.SourceMember, this.File));
    }

    /**
        | Member                               | arrange     |
        | ------------------------------------ | ----------- |
        | non-static void method               | sut-arrange |
        | non-static non-void method           | sut-arrange |
        | static extension void method         | sut-arrange |
        | static extension non-void method     | sut-arrange |
        | static non-extension void method     | -           |
        | static non-extension non-void method | -           |
        | property/field getter                | sut-arrange |
        | property/field setter                | sut-arrange |
     */
    private static string GetContent(IMemberDescription memberDescription, ITemplateFile file) =>
        memberDescription switch
        {
            IMethodDescription { IsStatic: true } => string.Empty,
            _ => file.GetSingleSnipped(SnippetType.SutArrange).Content,
        };
}
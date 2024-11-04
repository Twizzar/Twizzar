using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

/// <summary>
/// Node for the method-signature tag.
/// </summary>
public class MethodSignatureNode : SnippetNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MethodSignatureNode"/> class.
    /// </summary>
    /// <param name="snippet"></param>
    /// <param name="context"></param>
    /// <param name="parent"></param>
    public MethodSignatureNode(ITemplateSnippet snippet, ITemplateContext context, Maybe<ISnippetNode> parent)
        : base(snippet, context, parent)
    {
        this.Snippet = snippet.WithContent(
            GetSnipped(context.SourceCreationContext.SourceMember, context.File).Content);
    }

    private static ITemplateSnippet GetSnipped(IMemberDescription sourceMember, ITemplateFile file)
    {
        var taskFullName = typeof(System.Threading.Tasks.Task).FullName;

        if (taskFullName is not null &&
            sourceMember is IMethodDescription methodDescription &&
            methodDescription.TypeFullName.FullName.StartsWith(taskFullName))
        {
            return file.GetSingleSnipped(SnippetType.AsyncMethodSignature);
        }

        return file.GetSingleSnipped(SnippetType.VoidMethodSignature);
    }
}
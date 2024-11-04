using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

/// <summary>
/// Node for the act tag.
/// </summary>
public class ActNode : SnippetNode
{
    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="ActNode"/> class.
    /// </summary>
    /// <param name="snippet"></param>
    /// <param name="context"></param>
    /// <param name="parent"></param>
    public ActNode(ITemplateSnippet snippet, ITemplateContext context, Maybe<ISnippetNode> parent)
        : base(snippet, context, parent)
    {
        this.Snippet = snippet.WithContent(
            GetSnipped(context.SourceCreationContext.SourceMember, context.File).Content);
    }

    #endregion

    #region members

    /**
       | Member                               | act                        |
       | ------------------------------------ | -------------------------- |
       | non-static void method               | void-method-act            |
       | non-static non-void method           | non-void-method-act        |
       | static  void method                  | static-void-method-act     |
       | static  non-void method              | static-non-void-method-act |
       | property/field getter                | property-field-getter-act  |
       | property/field setter                | property-field-setter-act  |
     */
    private static ITemplateSnippet GetSnipped(IMemberDescription memberDescription, ITemplateFile file) =>
        memberDescription switch
        {
            IMethodDescription { IsStatic: false, TypeFullName.FullName: "System.Threading.Tasks.Task" } =>
                file.GetSingleSnipped(SnippetType.AsyncMethodAct),

            IMethodDescription { IsStatic: false, TypeFullName.FullName: { } fullName }
                when fullName.StartsWith("System.Threading.Tasks.Task`1") =>
                    file.GetSingleSnipped(SnippetType.AsyncResultMethodAct),

            IMethodDescription { IsStatic: true, TypeFullName.FullName: "System.Threading.Tasks.Task" } =>
                file.GetSingleSnipped(SnippetType.StaticAsyncMethodAct),

            IMethodDescription { IsStatic: true, TypeFullName.FullName: { } fullName }
                when fullName.StartsWith("System.Threading.Tasks.Task`1") =>
                    file.GetSingleSnipped(SnippetType.StaticAsyncResultMethodAct),

            IMethodDescription { IsStatic: false, TypeFullName.FullName: "System.Void" } =>
                file.GetSingleSnipped(SnippetType.VoidMethodAct),

            IMethodDescription { IsStatic: false } =>
                file.GetSingleSnipped(SnippetType.NonVoidMethodAct),

            IMethodDescription { IsStatic: true, TypeFullName.FullName: "System.Void" } =>
                file.GetSingleSnipped(SnippetType.StaticVoidMethodAct),

            IMethodDescription { IsStatic: true } =>
                file.GetSingleSnipped(SnippetType.StaticNonVoidMethodAct),

            IPropertyDescription { CanWrite: false } =>
                file.GetSingleSnipped(SnippetType.PropertyFieldGetterAct),

            _ => file.GetSingleSnipped(SnippetType.PropertyFieldSetterAct),
        };

    #endregion
}
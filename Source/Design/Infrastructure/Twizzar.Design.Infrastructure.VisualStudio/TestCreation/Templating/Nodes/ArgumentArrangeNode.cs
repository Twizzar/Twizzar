using System.Text;

using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

/// <summary>
/// Node for the argument-arrange tag.
/// </summary>
public class ArgumentArrangeNode : SnippetNode
{
    private readonly IShortTypesConverter _shortTypesConverter;

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentArrangeNode"/> class.
    /// </summary>
    /// <param name="snippet"></param>
    /// <param name="context"></param>
    /// <param name="parent"></param>
    /// <param name="shortTypesConverter"></param>
    public ArgumentArrangeNode(ITemplateSnippet snippet, ITemplateContext context, Maybe<ISnippetNode> parent, IShortTypesConverter shortTypesConverter)
        : base(snippet, context, parent)
    {
        EnsureHelper.GetDefault.Parameter(shortTypesConverter, nameof(shortTypesConverter))
            .ThrowWhenNull();

        this._shortTypesConverter = shortTypesConverter;
        this.Snippet = snippet
            .WithContent(GetContent(context.SourceCreationContext.SourceMember, this.File));
    }

    #endregion

    #region members

    /// <inheritdoc />
    public override string GetCode()
    {
        if (this.Context.SourceCreationContext.SourceMember is IMethodDescription methodDescription)
        {
            var sb = new StringBuilder();

            foreach (var parameter in methodDescription.DeclaredParameters)
            {
                var argumentType = this._shortTypesConverter.ConvertToShort(
                    parameter.TypeFullName,
                    parameter.TypeFullName.GetFriendlyCSharpTypeName());

                sb.AppendLine(base.GetCode()
                    .Replace("$argumentType$", argumentType)
                    .Replace("$argumentName$", parameter.Name));
            }

            return sb.ToString();
        }
        else if (this.Context.SourceCreationContext.SourceMember is IPropertyDescription propertyDescription)
        {
            var argumentType = this._shortTypesConverter.ConvertToShort(
                propertyDescription.TypeFullName,
                propertyDescription.TypeFullName.GetFriendlyCSharpTypeName());

            return base.GetCode().Replace("$argumentType$", argumentType);
        }

        return base.GetCode();
    }

    /**
        | Member                               | arguments-arrange             |
        | ------------------------------------ | ----------------------------- |
        | non-static void method               | method-arguments-arrange      |
        | non-static non-void method           | method-arguments-arrange      |
        | static extension void method         | method-arguments-arrange      |
        | static extension non-void method     | method-arguments-arrange      |
        | static non-extension void method     | method-arguments-arrange      |
        | static non-extension non-void method | method-arguments-arrange      |
        | property/field getter                | -                             |
        | property/field setter                | property-field-setter-arrange |
     */
    private static string GetContent(IMemberDescription memberDescription, ITemplateFile file) =>
        memberDescription switch
        {
            IMethodDescription =>
                file.GetSingleSnipped(SnippetType.MethodArgumentArrange).Content,

            IPropertyDescription { CanWrite: true } =>
                file.GetSingleSnipped(SnippetType.PropertyFieldSetterArranger).Content,

            _ =>
                string.Empty,
        };

    #endregion
}
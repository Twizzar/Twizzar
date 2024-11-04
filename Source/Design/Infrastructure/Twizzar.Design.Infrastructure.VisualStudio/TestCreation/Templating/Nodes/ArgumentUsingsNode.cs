using System.Linq;
using System.Text;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

/// <summary>
/// Node for the argument-usings tag.
/// </summary>
public class ArgumentUsingsNode : SnippetNode
{
    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentUsingsNode"/> class.
    /// </summary>
    /// <param name="snippet"></param>
    /// <param name="context"></param>
    /// <param name="parent"></param>
    public ArgumentUsingsNode(ITemplateSnippet snippet, ITemplateContext context, Maybe<ISnippetNode> parent)
        : base(snippet, context, parent)
    {
    }

    #endregion

    #region members

    /// <inheritdoc />
    public override string GetCode()
    {
        var code = base.GetCode();

        // for every additional using apply the snipped and replace the $argumentNamespace$ variable with the using.
        return this.Context.AdditionalUsings
            .Aggregate(
                new StringBuilder(),
                (builder, usingName) => builder.AppendLine(code.Replace("$argumentNamespace$", usingName)))
            .ToString().TrimEnd('\n', '\r');
    }

    #endregion
}
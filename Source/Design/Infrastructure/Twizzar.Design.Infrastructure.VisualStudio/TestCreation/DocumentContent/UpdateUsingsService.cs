using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.DocumentContent;

/// <inheritdoc />
[ExcludeFromCodeCoverage]
public class UpdateUsingsService : IUpdateUsingService
{
    #region fields

    private readonly ITemplateCodeProvider _templateCodeProvider;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUsingsService"/> class.
    /// </summary>
    /// <param name="templateCodeProvider"></param>
    public UpdateUsingsService(ITemplateCodeProvider templateCodeProvider)
    {
        this._templateCodeProvider = templateCodeProvider;
    }

    #endregion

    #region members

    /// <inheritdoc />
    public Document UpdateUsings(
        Document document,
        SyntaxNode roslynRootNode,
        ITemplateContext templateContext)
    {
        var compilationUnit = roslynRootNode.DescendantNodesAndSelf().OfType<CompilationUnitSyntax>().Single();
        var existingUsings = compilationUnit.Usings.Select(syntax => syntax.Name.ToString());
        var usingsToBeAdded = templateContext.AdditionalUsings.Except(existingUsings);

        if (!usingsToBeAdded.Any())
        {
            return document;
        }

        templateContext = templateContext.WithAdditionalUsings(usingsToBeAdded.ToImmutableHashSet());

        var code = this._templateCodeProvider.GetCode(SnippetType.ArgumentUsing, templateContext);

        var pos = compilationUnit.Usings.Span.End;
        var newText = compilationUnit.GetText().Replace(pos, 0, code);
        return document.WithText(newText);
    }

    #endregion
}
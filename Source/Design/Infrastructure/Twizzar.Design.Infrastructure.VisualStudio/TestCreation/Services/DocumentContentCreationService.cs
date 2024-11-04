using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Shell;
using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.DocumentContent;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;

namespace TestCreation.Services;

/// <inheritdoc cref="IDocumentContentCreationService"/>
public class DocumentContentCreationService : ProgressUpdater, IDocumentContentCreationService
{
    #region fields

    private readonly ITemplateCodeProvider _templateCodeProvider;
    private readonly Workspace _workspace;
    private readonly IUpdateUsingService _updateUsingService;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentContentCreationService"/> class.
    /// </summary>
    /// <param name="templateCodeProvider"></param>
    /// <param name="workspace"></param>
    /// <param name="updateUsingService"></param>
    public DocumentContentCreationService(
        ITemplateCodeProvider templateCodeProvider,
        Workspace workspace,
        IUpdateUsingService updateUsingService)
    {
        this._templateCodeProvider =
            templateCodeProvider ?? throw new ArgumentNullException(nameof(templateCodeProvider));

        this._workspace = workspace;
        this._updateUsingService = updateUsingService ?? throw new ArgumentNullException(nameof(updateUsingService));
    }

    #endregion

    #region properties

    /// <inheritdoc />
    public override int NumberOfProgressSteps => 1;

    #endregion

    #region members

    /// <inheritdoc />
    public Task<bool> CreateContentAsync(CreationContext destination)
    {
        if (destination == null)
        {
            throw new ArgumentNullException(nameof(destination));
        }

        return this.CreateContentInternalAsync(destination);
    }

    private async Task<bool> CreateContentInternalAsync(CreationContext destination)
    {
        var document = destination.CodeAnalysisContext.GetDocument();

        var rootNode = await document.GetSyntaxRootAsync() ??
                       throw new InternalException($"Cannot get the root node of the document {document.FilePath}.");

        var templateContext = destination.TemplateContext.SomeOrProvided(
            () => throw new InternalException("TemplateContext cannot be None"));

        if (!rootNode.DescendantNodes().Any())
        {
            this.Report("Document empty create file content.");
            document = this.CreateFileContent(templateContext, document);
        }
        else
        {
            document = this._updateUsingService.UpdateUsings(document, rootNode, templateContext);
            rootNode = await document.GetSyntaxRootAsync();

            var compilationUnit = rootNode.AncestorsAndSelf().OfType<CompilationUnitSyntax>().Single();

            var classDeclaration = GetClassDeclarationSyntax(destination, compilationUnit);

            if (classDeclaration is null)
            {
                this.Report($"Class {destination.Info.Type} not found generate the class.");
                document = this.CreateTestClass(compilationUnit, templateContext, document);
            }
            else
            {
                this.Report($"Generate the test method {destination.Info.Member}.");
                var code = Environment.NewLine +
                           this._templateCodeProvider.GetCode(SnippetType.Method, templateContext);

                var textSpan = classDeclaration.CloseBraceToken.Span;
                var sourceText = compilationUnit.GetText(Encoding.UTF8);
                sourceText = sourceText.Replace(textSpan.Start, 0, code);
                document = document.WithText(sourceText);
            }
        }

        document = await Formatter.FormatAsync(document);
        return await this.ApplyChangesAsync(document);
    }

    private async Task<bool> ApplyChangesAsync(Document document)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        return this._workspace.TryApplyChanges(document.Project.Solution);
    }

    private Document CreateTestClass(
        CompilationUnitSyntax compilationUnit,
        ITemplateContext templateContext,
        Document document)
    {
        var position = compilationUnit.DescendantNodes()
            .OfType<NamespaceDeclarationSyntax>()
            .FirstOrNone()
            .Map(syntax => syntax.CloseBraceToken.Span.Start)
            .SomeOrProvided(() => compilationUnit.Span.End);

        var code = this._templateCodeProvider.GetCode(SnippetType.Class, templateContext);
        var sourceText = compilationUnit.GetText(Encoding.UTF8);
        sourceText = sourceText.Replace(position, 0, code);
        document = document.WithText(sourceText);
        return document;
    }

    private static ClassDeclarationSyntax GetClassDeclarationSyntax(
        CreationContext destination,
        CompilationUnitSyntax compilationUnit)
    {
        return compilationUnit.DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .SingleOrDefault(syntax => syntax.Identifier.Text.Trim() == destination.Info.Type.Trim());
    }

    private Document CreateFileContent(ITemplateContext templateContext, Document document)
    {
        var code = this._templateCodeProvider.GetCode(SnippetType.File, templateContext);

        if (string.IsNullOrWhiteSpace(code))
        {
            return document;
        }

        document = document.WithText(SourceText.From(code, Encoding.UTF8));
        return document;
    }

    #endregion
}
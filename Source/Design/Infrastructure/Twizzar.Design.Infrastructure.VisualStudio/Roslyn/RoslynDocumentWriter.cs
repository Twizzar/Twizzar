using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Shell;

using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Command.Commands;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces.Roslyn;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Shared.Infrastructure;
using Twizzar.Design.Shared.Infrastructure.Extension;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

using static System.IO.Path;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

using Task = System.Threading.Tasks.Task;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <summary>
    /// Class implement <see cref="IDocumentWriter"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RoslynDocumentWriter : IDocumentWriter, IService
    {
        #region fields

        private readonly Workspace _workspace;
        private readonly string _filePath;
        private readonly IVsProjectManager _projectManager;
        private readonly IRoslynContextQuery _roslynContextQuery;
        private readonly ICommandBus _commandBus;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynDocumentWriter"/> class.
        /// </summary>
        /// <param name="workspace">The roslyn workspace.</param>
        /// <param name="filePath">The file path to write in.</param>
        /// <param name="projectManager">The vs project manager.</param>
        /// <param name="roslynContextQuery"></param>
        /// <param name="commandBus"></param>
        public RoslynDocumentWriter(
            Workspace workspace,
            string filePath,
            IVsProjectManager projectManager,
            IRoslynContextQuery roslynContextQuery,
            ICommandBus commandBus)
        {
            this.EnsureMany()
                .Parameter(workspace, nameof(workspace))
                .Parameter(filePath, nameof(filePath))
                .Parameter(projectManager, nameof(projectManager))
                .Parameter(roslynContextQuery, nameof(roslynContextQuery))
                .Parameter(commandBus, nameof(commandBus))
                .ThrowWhenNull();

            this._workspace = workspace;
            this._filePath = filePath;
            this._projectManager = projectManager;
            this._roslynContextQuery = roslynContextQuery;
            this._commandBus = commandBus;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public async Task PrepareClassAsync(IAdornmentInformation adornmentInformation, string configLabel)
        {
            var context = await this._roslynContextQuery.GetContextAsync(this._filePath);

            if (context.IsSuccess)
            {
                await this.PrepareClassAsync(adornmentInformation, configLabel, context.GetSuccessUnsafe());
            }
            else
            {
                this.Log(context.GetFailureUnsafe().Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// Generates the code-behind and it's content when it doesn't exist yet.
        /// </summary>
        /// <param name="adornmentInformation"></param>
        /// <param name="configName"></param>
        /// <returns>Task to wait until operation is completed.</returns>
        public async Task PrepareCodeBehindAsync(
            IAdornmentInformation adornmentInformation,
            string configName)
        {
            var context = await this._roslynContextQuery.GetContextAsync(this._filePath);

            if (context.IsSuccess)
            {
                await this.PrepareCodeBehindInternalAsync(adornmentInformation, configName, context.GetSuccessUnsafe());
            }
            else
            {
                this.Log(context.GetFailureUnsafe().Message, LogLevel.Error);
            }
        }

        private async Task PrepareCodeBehindInternalAsync(
            IAdornmentInformation adornmentInformation,
            string configName,
            IRoslynContext context)
        {
            var span = adornmentInformation.ObjectCreationSpan;
            var rootNode = context.RootNode;
            var sourceClass = FindSourceClass(rootNode, span);
            var currentDocument = context.Document;
            var solution = this._workspace.CurrentSolution;

            // Ensure code behind exists
            currentDocument = GetOrCreateCodeBehindDocument(currentDocument, solution, out var newFilePath);

            var compilationUnit = await this.GetCompilationAndAddNamespaceAsync(currentDocument, sourceClass);
            compilationUnit = AddPartialClassToCompilation(compilationUnit, sourceClass);
            compilationUnit = AddUsingsToCompilation(adornmentInformation, compilationUnit);

            compilationUnit = AddConfigClassToCompilation(
                compilationUnit,
                adornmentInformation,
                configName,
                sourceClass);

            compilationUnit = AddAutoGeneratedComment(compilationUnit);
            var syncedDocument = SyncDocument(currentDocument, compilationUnit);

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            this._workspace.TryApplyChanges(syncedDocument.Project.Solution);

            await this.SetDependentUponAsync(sourceClass, newFilePath);
        }

        private static CompilationUnitSyntax AddAutoGeneratedComment(CompilationUnitSyntax compilationUnit)
        {
            var maybeFirstElement = compilationUnit.ChildNodes().FirstOrNone();

            if (maybeFirstElement.AsMaybeValue() is SomeValue<SyntaxNode> firstElement)
            {
                var hasComment = firstElement.Value
                    .GetLeadingTrivia()
                    .Where(trivia => trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                    .Any(trivia => trivia.ToString().Contains("// <auto-generated />"));

                if (!hasComment)
                {
                    var newFirstElement = firstElement.Value.WithLeadingTrivia(compilationUnit.GetLeadingTrivia()
                        .Add(Comment("// <auto-generated />")));

                    compilationUnit = compilationUnit.ReplaceNode(firstElement.Value, newFirstElement);
                }
            }

            return compilationUnit;
        }

        private async Task PrepareClassAsync(
            IAdornmentInformation adornmentInformation,
            string configLabel,
            IRoslynContext context)
        {
            var solution = this._workspace.CurrentSolution;
            var rootNode = context.RootNode;
            var currentDocument = context.Document;

            var span = adornmentInformation.ObjectCreationSpan
                .ToTextSpan();

            rootNode = rootNode
                .FindNode(span)
                .CastTo<ObjectCreationExpressionSyntax>()
                .Match(
                    objectCreation =>
                        rootNode
                            .ReplaceNode(
                                objectCreation,
                                objectCreation.WithType(IdentifierName(configLabel))),
                    failure =>
                    {
                        this.Log(failure.Message, LogLevel.Error);
                        return rootNode;
                    });

            var classMaybe = rootNode
                .FindNode(span)
                .Ancestors()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrNone();

            rootNode = classMaybe
                .Bind(
                    classNode => classNode.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword))
                        ? Maybe.None()
                        : Maybe.Some(classNode))
                .Map<(ClassDeclarationSyntax Old, ClassDeclarationSyntax New)>(
                    c => (
                        c,
                        c.WithModifiers(
                            c.Modifiers.Add(
                                Token(SyntaxKind.PartialKeyword)
                                    .WithTrailingTrivia(Space)))))
                .Match(
                    n => rootNode.ReplaceNode(n.Old, n.New),
                    () => rootNode);

            var newSolution = solution.WithDocumentSyntaxRoot(currentDocument.Id, rootNode);

            var newCustomBuilder = rootNode.DescendantNodesAndSelf()
                .OfType<ObjectCreationExpressionSyntax>()
                .Where(syntax => syntax.Type.ToString().Contains(configLabel))
                .FirstOrNone();

            await newCustomBuilder.IfSomeAsync(syntax =>
                this._commandBus.SendAsync(
                    new StartFixtureItemConfigurationCommand(
                        adornmentInformation.FixtureItemId,
                        adornmentInformation.ProjectName,
                        adornmentInformation.DocumentFilePath,
                        syntax.GetLocation().SourceSpan.ConvertToViSpan())));

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            this._workspace.TryApplyChanges(newSolution);
        }

        private async Task SetDependentUponAsync(SyntaxNode sourceClass, string newFilePath)
        {
            try
            {
                var sourceFileName = GetFileName(sourceClass.SyntaxTree.FilePath);
                var codeBehindFileName = GetFileName(newFilePath);
                await this._projectManager.SetDependentUponAsync(codeBehindFileName, sourceFileName);
            }
            catch (Exception e)
            {
                this.Log(e);
            }
        }

        private async Task<string> FindSourceNamespaceAsync(BaseTypeDeclarationSyntax sourceClass)
        {
            var solution = this._workspace.CurrentSolution;

            var document = solution.GetDocument(sourceClass.SyntaxTree) ??
                           throw new InternalException("Failed to retrieve semantic model for source.");

            var semanticModel = await document.GetSemanticModelAsync();

            var typeSymbol = semanticModel.GetDeclaredSymbol(sourceClass) as ITypeSymbol;
            return typeSymbol?.GetTypeFullName().GetNameSpace();
        }

        private static ClassDeclarationSyntax FindSourceClass(SyntaxNode syntaxNode, IViSpan span) =>
            syntaxNode
                .FindNode(span.ToTextSpan())
                .Ancestors()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault() ??
            throw new InternalException("Failed to initialize code-behind.");

        private static Document GetOrCreateCodeBehindDocument(
            Document currentDocument,
            Solution solution,
            out string newFilePath)
        {
            if (currentDocument == null)
                throw new ArgumentNullException(nameof(currentDocument));

            currentDocument = solution.GetDocument(currentDocument.Id) ??
                              throw new InternalException("Failed to retrieve the current document.");

            var currentFilePath = currentDocument.FilePath ??
                                  throw new InternalException("Failed to determine code behind file path.");

            var currentFileName = GetFileNameWithoutExtension(currentFilePath);
            var currentFileExtension = GetExtension(currentFilePath);

            var currentFileDirectory = GetDirectoryName(currentFilePath) ??
                                       throw new InternalException("Failed to get the file path for the code-behind.");

            var newFileName = $"{currentFileName}.twizzar{currentFileExtension}";
            newFilePath = Combine(currentFileDirectory, newFileName);

            var codeBehindDocument = GetDocument(solution, newFilePath);

            return !codeBehindDocument.IsSome
                ? currentDocument.Project
                    .AddDocument(
                        newFileName,
                        text: string.Empty,
                        filePath: newFilePath)
                : codeBehindDocument.GetValueUnsafe();
        }

        private async Task<CompilationUnitSyntax> GetCompilationAndAddNamespaceAsync(
            Document currentDocument,
            BaseTypeDeclarationSyntax sourceClass)
        {
            var sourceNamespace = await this.FindSourceNamespaceAsync(sourceClass);
            var syntaxTree = await currentDocument.GetSyntaxTreeAsync();

            if (syntaxTree == null)
                throw new InternalException("Couldn't load syntax tree.");

            var syntaxRoot = await syntaxTree.GetRootAsync();

            return syntaxRoot is CompilationUnitSyntax compilationUnit
                ? syntaxRoot.ChildNodes().OfType<NamespaceDeclarationSyntax>().SingleOrDefault() == null
                    ? compilationUnit
                        .AddMembers(NamespaceDeclaration(IdentifierName(sourceNamespace)))
                    : compilationUnit
                : throw new InternalException("Couldn't retrieve compilation unit.");
        }

        private static CompilationUnitSyntax AddPartialClassToCompilation(
            CompilationUnitSyntax compilationUnit,
            BaseTypeDeclarationSyntax sourceClass)
        {
            var namespaceNode = FindNamespace(compilationUnit);

            if (namespaceNode == null)
                throw new InternalException("Failed to determine code behind file name.");

            var partialClass = FindPartialClassInTestSource(sourceClass, namespaceNode);

            if (partialClass != null)
                return compilationUnit;

            partialClass = CreatePartialClass(sourceClass);

            var newNamespaceNode = namespaceNode.AddMembers(partialClass);
            return compilationUnit.ReplaceNode(namespaceNode, newNamespaceNode);
        }

        private static NamespaceDeclarationSyntax FindNamespace(SyntaxNode compilationUnit) =>
            compilationUnit?
                .ChildNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .SingleOrDefault();

        private static ClassDeclarationSyntax FindPartialClassInTestSource(
            BaseTypeDeclarationSyntax sourceClass,
            SyntaxNode namespaceNode) =>
            namespaceNode
                .ChildNodes()
                .OfType<ClassDeclarationSyntax>()
                .SingleOrDefault(c => c.Identifier.ValueText == sourceClass.Identifier.ValueText);

        private static ClassDeclarationSyntax FindPartialClassInCodeBehind(
            SyntaxNode compilationUnit,
            BaseTypeDeclarationSyntax sourceClass) =>
            compilationUnit.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Single(c => c.Identifier.ValueText == sourceClass.Identifier.ValueText);

        private static ClassDeclarationSyntax CreatePartialClass(BaseTypeDeclarationSyntax sourceClass) =>
            ClassDeclaration(sourceClass.Identifier)
                .AddModifiers(
                    Token(SyntaxKind.PartialKeyword));

        private static Document SyncDocument(Document currentDocument, CompilationUnitSyntax compilationUnit) =>
            currentDocument.WithSyntaxRoot(compilationUnit.NormalizeWhitespace());

        private static CompilationUnitSyntax AddConfigClassToCompilation(
            CompilationUnitSyntax compilationUnit,
            IAdornmentInformation adornmentInformation,
            string configName,
            BaseTypeDeclarationSyntax sourceClass)
        {
            var configClassName = adornmentInformation.FixtureItemId.TypeFullName.GetFriendlyCSharpFullName();
            var partialClass = FindPartialClassInCodeBehind(compilationUnit, sourceClass);
            var subClass = FindConfigClass(partialClass, configClassName);

            if (subClass != null)
                return compilationUnit;

            subClass = CreateConfigClass(configName, configClassName);
            var newPartialClass = partialClass.AddMembers(subClass);

            return compilationUnit.ReplaceNode(partialClass, newPartialClass);
        }

        private static ClassDeclarationSyntax FindConfigClass(SyntaxNode partialClass, string configClassName) =>
            partialClass
                .ChildNodes()
                .OfType<ClassDeclarationSyntax>()
                .SingleOrDefault(c => c.Identifier.ValueText == configClassName);

        private static ClassDeclarationSyntax CreateConfigClass(string configName, string className) =>
            ClassDeclaration(configName)
                .AddModifiers(
                    Token(SyntaxKind.PrivateKeyword))
                .AddBaseListTypes(
                    SimpleBaseType(
                        GenericName("ItemBuilder")
                            .AddTypeArgumentListArguments(
                                IdentifierName(className),
                                IdentifierName($"{configName}Paths"))));

        private static CompilationUnitSyntax AddUsingsToCompilation(
            IAdornmentInformation adornmentInformation,
            CompilationUnitSyntax compilationUnit)
        {
            var configNameSpace = adornmentInformation.FixtureItemId.TypeFullName.GetNameSpace();
            var usings = compilationUnit.ChildNodes().OfType<UsingDirectiveSyntax>().ToHashSet();

            compilationUnit = AddMissingUsingsToCompilation(usings, compilationUnit, ApiNames.ApiNamespace);

            if (!string.IsNullOrEmpty(configNameSpace))
            {
                compilationUnit = AddMissingUsingsToCompilation(usings, compilationUnit, configNameSpace);
            }

            return compilationUnit;
        }

        private static CompilationUnitSyntax AddMissingUsingsToCompilation(
            IEnumerable<UsingDirectiveSyntax> usings,
            CompilationUnitSyntax compilationUnit,
            string fullNameSpace)
        {
            if (usings.Any(u => u.Name.ToString() == fullNameSpace))
            {
                return compilationUnit;
            }

            var @using = UsingDirective(ParseName(fullNameSpace));

            return compilationUnit.AddUsings(@using);
        }

        private static Maybe<Document> GetDocument(Solution currentSolution, string filePath)
        {
            var documentId = currentSolution
                .GetDocumentIdsWithFilePath(filePath)
                .FirstOrDefault();

            return documentId == null
                ? Maybe.None<Document>()
                : currentSolution.GetDocument(documentId);
        }

        #endregion
    }
}
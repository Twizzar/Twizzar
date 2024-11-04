using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Shell;

using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation;
using Twizzar.Design.Shared.Infrastructure.VisualStudio2022.Extension;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.Functional;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;
using Result = ViCommon.Functional.Monads.ResultMonad.Result;

#pragma warning disable SA1102 // Query clause should follow previous clause

namespace TestCreation.Services;

/// <inheritdoc cref="INavigationService" />
public class NavigationService : INavigationService
{
    #region fields

    private readonly Workspace _workspace;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    /// <param name="workspace"></param>
    public NavigationService(Workspace workspace)
    {
        this._workspace = workspace;
    }

    #endregion

    #region members

    /// <inheritdoc />
    public Task<IResult<Unit, Failure>> NavigateAsync(CreationInfo target, CancellationToken cancellationToken)
    {
        if (target == null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        return this.NavigateInternalAsync(target, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IResult<Unit, Failure>> NavigateBackAsync(CreationContext sourceContext, CancellationToken cancellationToken)
    {
        var document = sourceContext.CodeAnalysisContext.GetDocument();
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken);

        if (sourceContext.SourceMember.GetSymbol() is IMethodSymbol methodSymbol)
        {
            var maybeAttribute = methodSymbol
                .GetAttributes()
                .FirstOrNone(data => data.AttributeClass?.Name == "TestSourceAttribute");

            var maybeId = maybeAttribute.Bind(
                attribute =>
                    this.TryGetDocumentIdByAttributeNameOfArgument(attribute, semanticModel)
                        .BindNone(() =>
                            this.TryGetDocumentIdByAttributeMethodName(methodSymbol, attribute, semanticModel)));

            if (maybeId.AsMaybeValue() is SomeValue<DocumentId> id)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
                this._workspace.OpenDocument(id);
                return Result.Success<Unit, Failure>(Unit.New);
            }
            else
            {
                return new Failure("Cannot find a valid [TestSource] attribute.")
                    .ToResult<Unit, Failure>();
            }
        }

        return new Failure($"MemberUnderTest is not a Method. Input: {sourceContext.SourceMember}")
            .ToResult<Unit, Failure>();
    }

    private Maybe<DocumentId> TryGetDocumentIdByAttributeNameOfArgument(
        AttributeData attribute,
        SemanticModel semanticModel) =>
            from node in Maybe.ToMaybe(attribute?.ApplicationSyntaxReference?.GetSyntax())

            // find the nameof statement ([TestSource(nameof(MyClass.MyMethod)]
            from nameOfSyntax in node.DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(syntax => syntax.Expression is IdentifierNameSyntax { Identifier.ValueText: "nameof" })
                .FirstOrNone()

            // find the member invocation in the nameof argument: nameof(MyClass.MyMethod) -> MyClass.MyMethod
            let memberAccess = nameOfSyntax.ArgumentList.Arguments.First().Expression
            from memberSymbol in semanticModel.GetSymbolInfo(memberAccess).CandidateSymbols.FirstOrNone()
            from documentId in this.GetDocumentIdFormMemberSymbol(memberSymbol)
            select documentId;

    private Maybe<DocumentId> TryGetDocumentIdByAttributeMethodName(
        ISymbol methodSymbol,
        AttributeData attribute,
        SemanticModel semanticModel) =>
            from argument in attribute.ConstructorArguments.FirstOrNone()

            // find the method name form the argument nameof("MyMethod") -> "MyMethod"
            from methodName in Maybe.ToMaybe(argument.Value as string)

            /* search for a method invocation where the identifier name is methodName
             The syntax tree for the node we are interested should look similar to something like this:
             Invocation Expression: sut.MyMethod();
             ├─MemberAccessExpression: sut.MyMethod
             │   └─IdentifierName: MyMethod
             └─ArgumentList: ()
            */
            from memberAccess in methodSymbol.DeclaringSyntaxReferences
                .Select(reference => reference.SyntaxTree.GetRoot())
                .SelectMany(root => root.DescendantNodes())
                .OfType<MemberAccessExpressionSyntax>()
                .Where(syntax => syntax.Name.Identifier.ValueText == methodName)
                .FirstOrNone()
            from memberSymbol in Maybe.ToMaybe(
                semanticModel.GetSymbolInfo(memberAccess).Symbol)
            from documentId in this.GetDocumentIdFormMemberSymbol(memberSymbol)
            select documentId;

    private Maybe<DocumentId> GetDocumentIdFormMemberSymbol(ISymbol memberSymbol) =>
        from syntaxReference in memberSymbol.DeclaringSyntaxReferences.FirstOrNone()
        let filePath = syntaxReference.SyntaxTree.FilePath
        from documentId in this._workspace.CurrentSolution.GetDocumentIdsWithFilePath(filePath).FirstOrNone()
        select documentId;

    private async Task<IResult<Unit, Failure>> NavigateInternalAsync(CreationInfo target, CancellationToken cancellationToken)
    {
        var documentId = this._workspace.CurrentSolution.GetDocumentIdsWithFilePath(target.File).SingleOrDefault();

        if (documentId is not null)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            this._workspace.OpenDocument(documentId);
            return Result.Success<Unit, Failure>(Unit.New);
        }

        return new Failure($"Cannot find the the file {target.File}.")
            .ToResult<Unit, Failure>();
    }

    #endregion
}
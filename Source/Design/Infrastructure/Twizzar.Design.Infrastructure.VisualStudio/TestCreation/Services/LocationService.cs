using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation;
using Twizzar.Design.Shared.Infrastructure.Description;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.Design.Shared.Infrastructure.VisualStudio2022.Extension;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace TestCreation.Services;

/// <summary>
/// Class to collect the mouse cursor location information.
/// </summary>
public class LocationService : ILocationService
{
    #region fields

    private readonly IRoslynContextQuery _roslynContextQuery;
    private readonly IBaseTypeService _baseTypeService;
    private readonly IRoslynDescriptionFactory _roslynDescriptionFactory;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="LocationService"/> class.
    /// </summary>
    /// <param name="roslynContextQuery"></param>
    /// <param name="baseTypeService"></param>
    /// <param name="roslynDescriptionFactory"></param>
    public LocationService(
        IRoslynContextQuery roslynContextQuery,
        IBaseTypeService baseTypeService,
        IRoslynDescriptionFactory roslynDescriptionFactory)
    {
        this._roslynContextQuery = roslynContextQuery ?? throw new ArgumentNullException(nameof(roslynContextQuery));
        this._baseTypeService = baseTypeService ?? throw new ArgumentNullException(nameof(baseTypeService));

        this._roslynDescriptionFactory = roslynDescriptionFactory ??
                                         throw new ArgumentNullException(nameof(roslynDescriptionFactory));
    }

    #endregion

    #region members

    /// <inheritdoc />
    public async Task<CreationContext> GetCurrentLocation(string filePath, int cursorIndex)
    {
        var context = await this.GetRoslynContextAsync(filePath)
            .MatchAsync(
                roslynContext => roslynContext,
                failure => throw new InternalException(failure.Message));

        var currentNode = context.RootNode.FindNode(new TextSpan(cursorIndex, 0), true, true);

        var memberDeclarationSyntax = GetMemberDeclarationSyntax(currentNode, cursorIndex)
            .Match(
                syntax => syntax,
                failure => throw new InternalException(failure.Message));

        var namespaceDeclarationSyntax = GetAscendingSyntaxNode<BaseNamespaceDeclarationSyntax>(currentNode);
        var typeDeclarationSyntax = GetAscendingSyntaxNode<TypeDeclarationSyntax>(currentNode);

        var creationContextResult =
            from creationInfo in GetCreationInfo(
                context,
                namespaceDeclarationSyntax.Map(syntax => syntax.Name.ToString()).SomeOrProvided(string.Empty),
                typeDeclarationSyntax.Map(syntax => syntax.Identifier.ToString()).SomeOrProvided(string.Empty),
                memberDeclarationSyntax)
            from memberDescription in this.GetMemberDescription(context, memberDeclarationSyntax)
            let typeDescription = this.GetTypeDescription(memberDescription.GetSymbol())
            let codeAnalysisContext = new CodeAnalysisContext(context.Document)
            select new CreationContext(creationInfo, memberDescription, typeDescription, codeAnalysisContext, Maybe.None());

        return creationContextResult.Match(
            creationContext => creationContext,
            failure => throw new InternalException(failure.Message));
    }

    /// <inheritdoc />
    public async Task<bool> CheckIfValidLocationAsync(string filePath, int cursorIndex)
    {
        var context = await this.GetRoslynContextAsync(filePath)
            .MatchAsync(
                roslynContext => roslynContext,
                failure => throw new InternalException(failure.Message));

        var currentNode = context.RootNode.FindNode(new TextSpan(cursorIndex, 0), true, true);
        return GetMemberDeclarationSyntax(currentNode, cursorIndex).IsSuccess;
    }

    private static IResult<MemberDeclarationSyntax, Failure> GetMemberDeclarationSyntax(
        SyntaxNode currentNode,
        int cursorIndex)
    {
        var memberDeclarationSyntax = GetAscendingSyntaxNode<MethodDeclarationSyntax>(currentNode)
            .Match(
                syntax => syntax,
                () => GetAscendingSyntaxNode<PropertyDeclarationSyntax>(currentNode)
                    .Match(
                        syntax => syntax as MemberDeclarationSyntax,
                        () => null));

        if (memberDeclarationSyntax == null)
        {
            return new Failure("neither in Method- nor in Property declaration Syntax")
                .ToResult<MemberDeclarationSyntax, Failure>();
        }

        if (!memberDeclarationSyntax.ChildTokens().Any(syntaxToken => syntaxToken.IsKind(SyntaxKind.PublicKeyword)))
        {
            return new Failure("Member is not public.").ToResult<MemberDeclarationSyntax, Failure>();
        }

        return memberDeclarationSyntax.Span.Contains(cursorIndex)
            ? Result<MemberDeclarationSyntax, Failure>.Success(memberDeclarationSyntax)
            : new Failure("The caret is not on the right position").ToResult<MemberDeclarationSyntax, Failure>();
    }

    private static Maybe<T> GetAscendingSyntaxNode<T>(SyntaxNode currentNode)
        where T : SyntaxNode =>
        currentNode
            .AncestorsAndSelf()
            .OfType<T>()
            .FirstOrNone();

    private async Task<IResult<IRoslynContext, Failure>> GetRoslynContextAsync(string filePath) =>
        await this._roslynContextQuery.GetContextAsync(filePath);

    private IResult<IMemberDescription, Failure> GetMemberDescription(
        IRoslynContext context,
        MemberDeclarationSyntax memberDeclarationSyntax)
    {
        var symbol = context.SemanticModel.GetDeclaredSymbol(memberDeclarationSyntax);

        return symbol switch
        {
            IMethodSymbol methodSymbol =>
                new RoslynMethodDescription(methodSymbol, false, this._baseTypeService, this._roslynDescriptionFactory)
                    .ToSuccess<IMemberDescription, Failure>(),
            IPropertySymbol propertySymbol =>
                new RoslynPropertyDescription(propertySymbol, this._baseTypeService, this._roslynDescriptionFactory)
                    .ToSuccess<IMemberDescription, Failure>(),
            _ => new Failure("Member is not a method or a property").ToResult<IMemberDescription, Failure>(),
        };
    }

    private ITypeDescription GetTypeDescription(ISymbol memberSymbol) =>
        this._roslynDescriptionFactory.CreateDescription(memberSymbol.ContainingType);

    private static IResult<CreationInfo, Failure> GetCreationInfo(
        IRoslynContext context,
        string nameSpace,
        string typeName,
        MemberDeclarationSyntax memberDeclarationSyntax)
    {
        var memberName = memberDeclarationSyntax switch
        {
            MethodDeclarationSyntax x => x.Identifier.ToString(),
            PropertyDeclarationSyntax x => x.Identifier.ToString(),
            _ => throw new InternalException(
                "memberDeclarationSyntax is not of type MethodDeclarationSyntax  or PropertyDeclarationSyntax  "),
        };

        var projectFilePath = context.Document?.Project.FilePath;
        var documentFilePath = context.Document?.FilePath;
        var solutionPath = context.Document?.Project.Solution.FilePath;

        if (projectFilePath is null || documentFilePath is null || memberName is null)
        {
            return new Failure(
                    "One of the following info is not available: project path, document path, solution path or the member name.")
                .ToResult<CreationInfo, Failure>();
        }

        return new CreationInfo(
            solutionPath,
            projectFilePath,
            documentFilePath,
            nameSpace,
            typeName,
            memberName).ToSuccess<CreationInfo, Failure>();
    }

    #endregion
}
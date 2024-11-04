using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

using Twizzar.Analyzer.Core.Diagnostics;
using Twizzar.Analyzer.Core.Interfaces;
using Twizzar.Analyzer.SourceTextGenerators;
using Twizzar.Analyzer2022.App.SourceTextGenerators;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Shared.Infrastructure;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.Design.Shared.Infrastructure.PathTree;
using Twizzar.Design.Shared.Infrastructure.VisualStudio2019.Name;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Analyzer.Core.SourceTextGenerators
{
    /// <inheritdoc cref="IPathSourceTextGenerator" />
    public class PathSourceTextGenerator : IPathSourceTextGenerator
    {
        #region fields

        private readonly IPathSourceTextMemberGenerator _pathSourceTextMemberGenerator;
        private readonly IRoslynDescriptionFactory _descriptionFactory;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="PathSourceTextGenerator"/> class.
        /// </summary>
        /// <param name="descriptionFactory"></param>
        /// <param name="pathSourceTextMemberGenerator"></param>
        public PathSourceTextGenerator(
            IRoslynDescriptionFactory descriptionFactory,
            IPathSourceTextMemberGenerator pathSourceTextMemberGenerator)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(descriptionFactory, nameof(descriptionFactory))
                .Parameter(pathSourceTextMemberGenerator, nameof(pathSourceTextMemberGenerator))
                .ThrowWhenNull();

            this._descriptionFactory = descriptionFactory;
            this._pathSourceTextMemberGenerator = pathSourceTextMemberGenerator;
        }

        #endregion

        #region members

        /// <inheritdoc />
        public IResult<SourceText, PathGenerationFailure> GenerateSourceText(
            string className,
            string nameSpace,
            ITypeSymbol fixtureItemSymbol,
            IPathNode root,
            bool generateFuturePaths,
            Compilation compilation,
            ISymbol sourceSymbol,
            CancellationToken cancellationToken = default)
        {
            var codeResult = this.GenerateCode(
                className,
                nameSpace,
                fixtureItemSymbol,
                root,
                generateFuturePaths,
                compilation,
                sourceSymbol,
                cancellationToken);

            return codeResult
                .MapSuccess(code => SyntaxFactory.ParseCompilationUnit(code).NormalizeWhitespace())
                .MapSuccess(node => node.GetText(Encoding.UTF8));
        }

        private IResult<string, PathGenerationFailure> GenerateCode(
            string className,
            string nameSpace,
            ITypeSymbol fixtureItemSymbol,
            IPathNode root,
            bool generateFuturePaths,
            Compilation compilation,
            ISymbol sourceSymbol,
            CancellationToken cancellationToken)
        {
            // generate a set of all namespaces for the using statements
            var usingSet = new HashSet<string>
            {
                "System",
                "Twizzar.Fixture",
                "Twizzar.Fixture.Utils",
                "Twizzar.Fixture.Member",
                "System.Linq.Expressions",
            };

            var namespaceStartBlock = @$"namespace {nameSpace} {{";

            var fixtureItemTypeDescription = this._descriptionFactory.CreateDescription(fixtureItemSymbol);

            var accessModifierResult = RoslynHelper.GetAccessModifierToken(fixtureItemSymbol);

            if (accessModifierResult.AsResultValue() is FailureValue<Failure> failure)
            {
                return Result.Failure<string, PathGenerationFailure>(
                    new PathGenerationFailure(
                        failure.Value.Message,
                        fixtureItemSymbol,
                        ItemConfigPathDiagnostics.OnlyPublicInternalSupported));
            }

            var accessModifier = accessModifierResult.GetSuccessUnsafe();

            var fixtureItemTypeName = fixtureItemTypeDescription.GetFriendlyReturnTypeFullName();

            if (!fixtureItemSymbol.ContainingNamespace?.IsGlobalNamespace ?? false)
            {
                usingSet.AddIfNotExists(fixtureItemTypeDescription.TypeFullName.GetNameSpace());
            }

            var reservedMembers = new HashSet<string>(ApiNames.ReservedPathProviderMembers);

            var membersForVerification = new List<MemberVerificationInfo>();

            cancellationToken.ThrowIfCancellationRequested();

            var members = this._pathSourceTextMemberGenerator
                .GenerateMembers(
                    fixtureItemTypeDescription,
                    Maybe.Some(root),
                    fixtureItemTypeName,
                    usingSet,
                    reservedMembers,
                    generateFuturePaths,
                    compilation,
                    sourceSymbol,
                    membersForVerification,
                    className,
                    cancellationToken: cancellationToken);

            var usingStatements = usingSet
                .Select(s => $"using {s};\n")
                .AggregateWithNewLine();

            var verificationExtensionMethods = GenerateVerificationExtensionMethods(
                fixtureItemTypeName,
                className,
                membersForVerification,
                compilation);

            var code = $@"
                // Auto-generated code
                // <auto-generated />
                {usingStatements}

                #pragma warning disable CS0108 // Member hides inherited member; missing new keyword
                #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

                {namespaceStartBlock}

                    {accessModifier} class {className} : PathProvider<{fixtureItemTypeName}>
                    {{
                        {members}
                    }}

                    {accessModifier} static class {className}VerifierExtensions
                    {{
                        {verificationExtensionMethods}
                    }}
                }}
            ";

            return Result.Success<string, PathGenerationFailure>(code);
        }

        private static string GenerateVerificationExtensionMethods(
            string fixtureItemType,
            string pathClassName,
            IEnumerable<MemberVerificationInfo> membersForVerification,
            Compilation compilation)
        {
            var usedTypeParamSet = new HashSet<(string MemberType, string ParamName, string MemberPathName)>();
            var sb = new StringBuilder();

            foreach (var (baseDescription, memberPathName) in membersForVerification.Distinct())
            {
                if (baseDescription is IMethodDescription methodDescription)
                {
                    foreach (var declaredParameter in methodDescription.DeclaredParameters)
                    {
                        var genericParameterMapping = methodDescription.GenericTypeArguments
                            .Select(pair => pair.Value.Name)
                            .ToDictionary(s => s, s => $"Tz{s}");

                        var usedGenericParameters = new HashSet<string>();
                        var paramType = RoslynHelper.GetTypeNameWithMappedGenericParameters(
                            declaredParameter,
                            genericParameterMapping,
                            usedGenericParameters);

                        var paramName = declaredParameter.Name.Trim();
                        var paramNameUpper = (char.ToUpper(paramName[0]) + paramName.Substring(1)).Trim();

                        if (usedTypeParamSet.Contains((paramNameUpper, paramType, memberPathName)))
                        {
                            continue;
                        }

                        var genericParam = string.Empty;
                        var genericConstrain = string.Empty;
                        if (usedGenericParameters.Count > 0)
                        {
                            string GetName(ITypeFullName typeFullName)
                            {
                                var symbol = ((SymbolTypeFullNameToken)typeFullName.Cast().Token).Symbol;
                                return RoslynHelper.GetTypeNameWithMappedGenericParameters(symbol, genericParameterMapping);
                            }

                            genericParam = usedGenericParameters.ToDisplayString(", ", "<", ">");
                            genericConstrain = methodDescription.GenericTypeArguments
                                .Where(pair => usedGenericParameters.Contains(genericParameterMapping[pair.Value.Name]))
                                .Where(pair => pair.Value.GetAllConstrainsAsString(name => name.FullName).Any())
                                .Select(pair =>
                                    $"where {genericParameterMapping[pair.Value.Name]} : {pair.Value.GetAllConstrainsAsString(GetName).ToCommaSeparated()}")
                                .ToDisplayString(" ");
                        }

                        usedTypeParamSet.Add((paramNameUpper, paramType, memberPathName));

                        sb.Append(@$"
public static IMethodVerifier<{fixtureItemType}, {pathClassName}, {memberPathName}> Where{paramNameUpper}Is{genericParam}(
    this IMethodVerifier<{fixtureItemType}, {pathClassName}, {memberPathName}> self,
    Expression<Func<{paramType}, bool>> predicate) {genericConstrain} =>
        self.WhereParamIs<{paramType}>(""{paramName}"", predicate);

public static IMethodVerifier<{fixtureItemType}, {pathClassName}, {memberPathName}> Where{paramNameUpper}Is{genericParam}(
    this IMethodVerifier<{fixtureItemType}, {pathClassName}, {memberPathName}> self, {paramType} value) {genericConstrain} =>
        self.WhereParamIs<{paramType}>(""{paramName}"", value);

public static IMethodVerifier<{fixtureItemType}, {pathClassName}, {memberPathName}> Where{paramNameUpper}Is{genericParam}(
    this IMethodVerifier<{fixtureItemType}, {pathClassName}, {memberPathName}> self, Func<{pathClassName}, MemberPath<{fixtureItemType}, {paramType}>> selector) {genericConstrain} =>
        self.WhereParamIs<{paramType}>(""{paramName}"", selector);
");
                    }
                }
            }

            return sb.ToString();
        }

        #endregion
    }
}
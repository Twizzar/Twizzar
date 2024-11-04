using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

using FluentAssertions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using NUnit.Framework;

using Twizzar.Analyzer.Core.Interfaces;
using Twizzar.Analyzer.Core.SourceTextGenerators;
using Twizzar.Analyzer.Core.Tests.Builders;
using Twizzar.Design.Shared.Infrastructure.Description;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.Design.Shared.Infrastructure.PathTree;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.TestCommon;

using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Analyzer.Core.Tests.SourceTextGenerators
{
    [TestFixture]
    public partial class PathSourceTextGeneratorTests
    {
        #region fields

        private IRoslynDescriptionFactory _descriptionFactory;
        private ITypeDescription _typeDescription;
        private IPathNode _rootPathNode;
        private Compilation _compilation = CSharpCompilation.Create("Test");

        #endregion

        #region members

        [SetUp]
        public void SetUp()
        {
            this._typeDescription = new RoslynTypeDescription(
                new TypeSymbolBuilder()
                    .WithName("MyType")
                    .WithGlobalNamespace().Build(),
                Mock.Of<IBaseTypeService>(),
                Mock.Of<IRoslynDescriptionFactory>());

            this._descriptionFactory = Mock.Of<IRoslynDescriptionFactory>(
                factory =>
                    factory.CreateDescription(It.IsAny<ITypeSymbol>()) == this._typeDescription);

            this._rootPathNode = Mock.Of<IPathNode>(
                node =>
                    node.Parent == Maybe.None<IPathNode>() &&
                    node.Children == ImmutableDictionary<string, IPathNode>.Empty &&
                    node.TypeSymbol == Maybe.None<ITypeSymbol>() &&
                    node.MemberName == "Root");
        }

        [Test]
        public void Ctor_parameters_throw_ArgumentNullException_when_null()
        {
            // assert
            TwizzarInternal.Fixture.Verify.Ctor<PathSourceTextGenerator>()
                .ShouldThrowArgumentNullException();
        }

        [Test]
        public void Empty_pathProvider_class_is_created()
        {
            // arrange
            var expected = NormalizeCode("namespace MyNamespace { public class MyBuilder : PathProvider<MyType> {} public static class MyBuilderVerifierExtensions {}}");

            var typeSymbol = new TypeSymbolBuilder()
                .WithName("MyBuilder")
                .WithGlobalNamespace()
                .Build();

            var sut = new PathSourceTextGenerator(
                this._descriptionFactory,
                Mock.Of<IPathSourceTextMemberGenerator>());

            var pathNode = PathNode.ConstructRoot(new [] { Array.Empty<string>() });

            // act
            var result = sut.GenerateSourceText(
                "MyBuilder",
                "MyNamespace",
                typeSymbol,
                pathNode,
                true,
                this._compilation,
                Mock.Of<ITypeSymbol>());

            // assert
            var sourceText = TestHelper.AssertAndUnwrapSuccess(result);
            var output = CSharpSyntaxTree.ParseText(sourceText)
                .GetCompilationUnitRoot()
                .WithUsings(SyntaxFactory.List<UsingDirectiveSyntax>())
                .NormalizeWhitespace()
                .ToString();

            output.Should().Be(expected);
        }

        [Test]
        public void FixtureWithGlobalNamespaceDoesNotGenerateAUsing()
        {
            // arrange
            var typeSymbol = new TypeSymbolBuilder()
                .WithName("MyBuilder")
                .WithGlobalNamespace()
                .Build();

            var sut = new PathSourceTextGenerator(
                this._descriptionFactory,
                Mock.Of<IPathSourceTextMemberGenerator>());

            var pathNode = PathNode.ConstructRoot(new[] { Array.Empty<string>() });

            // act
            var sourceTextResult = sut.GenerateSourceText(
                "MyBuilder",
                "MyNamespace",
                typeSymbol,
                pathNode,
                true,
                this._compilation,
                Mock.Of<ITypeSymbol>());

            // assert
            var sourceText = TestHelper.AssertAndUnwrapSuccess(sourceTextResult);
            var output = CSharpSyntaxTree.ParseText(sourceText)
                .GetCompilationUnitRoot();

            output.Usings.Should().NotContain(syntax => syntax.Name.ToString() == string.Empty);
        }

        [Test]
        public void DependencyOfFixtureWithGlobalNamespaceDoesNotGenerateAUsing()
        {
            // arrange
            var member = Mock.Of<IPropertySymbol>(symbol =>
                symbol.Name == "Member" &&
                symbol.Type == new TypeSymbolBuilder().Build() &&
                symbol.ContainingNamespace ==
                    Mock.Of<INamespaceSymbol>(namedTypeSymbol => namedTypeSymbol.IsGlobalNamespace == true));

            //var member = new ItemBuilder<IPropertySymbol>()
            //    .With(p => p.ContainingNamespace.IsGlobalNamespace.Value(true))
            //    .With(p => p.Name.Value("Member"))
            //    .With(p => p.Type.Value(new TypeSymbolBuilder().Build()))
            //    .Build();

            var typeSymbol = new TypeSymbolBuilder()
                .WithName("MyBuilder")
                .WithNamespace("Test")
                .WithMembers(member)
                .Build();

            var typeDescription = new RoslynTypeDescription(
                new TypeSymbolBuilder()
                    .WithName("MyType")
                    .WithNamespace("Test").Build(),
                Mock.Of<IBaseTypeService>(),
                Mock.Of<IRoslynDescriptionFactory>());

            var descriptionFactory = Mock.Of<IRoslynDescriptionFactory>(
                factory =>
                    factory.CreateDescription(It.IsAny<ITypeSymbol>()) == typeDescription);

            var sut = new PathSourceTextGenerator(
                descriptionFactory,
                Mock.Of<IPathSourceTextMemberGenerator>());

            var pathNode = PathNode.ConstructRoot(new[] { new[] { "Member" } });

            // act
            var sourceTextResult = sut.GenerateSourceText(
                "MyBuilder",
                "MyNamespace",
                typeSymbol,
                pathNode,
                true,
                this._compilation,
                Mock.Of<ITypeSymbol>());

            // assert
            var sourceText = TestHelper.AssertAndUnwrapSuccess(sourceTextResult);

            var output = CSharpSyntaxTree.ParseText(sourceText)
                .GetCompilationUnitRoot();

            output.Usings.Should().NotContain(syntax => syntax.Name.ToString() == string.Empty);
        }

        [Test]
        public void Member_get_generated_correctly()
        {
            // arrange
            const string members = "public static int MyInt => 5";
            var expected = NormalizeCode(@$"
namespace MyNamespace
{{
    public class MyBuilder : PathProvider<MyType> {{ {members} }}
    public static class MyBuilderVerifierExtensions
    {{
    }}
}}");

            var typeSymbol = new TypeSymbolBuilder()
                .WithName("MyBuilder")
                .WithGlobalNamespace()
                .Build();

            var pathSourceTextMemberGenerator = new PathSourceTextMemberGeneratorDummy(members);

            var sut = new PathSourceTextGenerator(
                this._descriptionFactory,
                pathSourceTextMemberGenerator);

            var pathNode = PathNode.ConstructRoot(new[] { new[] { "MyInt" } });

            // act
            var result = sut.GenerateSourceText(
                "MyBuilder",
                "MyNamespace",
                typeSymbol,
                pathNode,
                true,
                this._compilation,
                Mock.Of<ITypeSymbol>());

            // assert
            var sourceText = TestHelper.AssertAndUnwrapSuccess(result);
            var output = CSharpSyntaxTree.ParseText(sourceText)
                .GetCompilationUnitRoot()
                .WithUsings(SyntaxFactory.List<UsingDirectiveSyntax>())
                .NormalizeWhitespace()
                .ToString();

            output.Should().Be(expected);
        }

        private static string NormalizeCode(string code) =>
            CSharpSyntaxTree.ParseText(code).GetRoot().NormalizeWhitespace().ToString();

        #endregion


        /// <summary>
        /// Because for some reason this cannot be setup with a mock.
        /// </summary>
        private class PathSourceTextMemberGeneratorDummy : IPathSourceTextMemberGenerator
        {
            private readonly string _members;

            public PathSourceTextMemberGeneratorDummy(string members)
            {
                this._members = members;
            }

            #region Implementation of IPathSourceTextMemberGenerator

            /// <inheritdoc />
            public string GenerateMembers(
                ITypeDescription typeDescription,
                Maybe<IPathNode> parent,
                string fixtureItemTypeName,
                HashSet<string> usingStatements,
                HashSet<string> reservedMembers,
                bool generateFuturePaths,
                Compilation compilation,
                ISymbol sourceType,
                in List<MemberVerificationInfo> membersForVerification,
                string declaredType,
                bool isRoot = true,
                CancellationToken cancellationToken = default) =>
                    this._members;

            #endregion
        }
    }
}
using System.Linq;

using FluentAssertions;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;
using Twizzar.Design.Shared.Infrastructure.PathTree;

namespace Twizzar.Design.Shared.Infrastructure.Tests.PathTree
{
    [TestFixture]
    public class PathTreeBuilderTests
    {
        [TestCase("this.With(p => p.Test.Value(5));", true)]
        [TestCase("this.With(p => p.Test.Value(5))", true)]
        [TestCase("this.With(p => p.Test)", false)]
        [TestCase("With(p => p.Test)", false)]
        public void Simple_property_tree_is_generated_correctly(string body, bool hasInvocation)
        {
            var tree = CSharpSyntaxTree.ParseText(
                $@"{body}");

            var identifierName = tree
                .GetRoot()
                .DescendantNodesAndSelf()
                .OfType<IdentifierNameSyntax>()
                .Single(syntax => syntax.Identifier.Text == "p");

            var cSharpCompilation = CSharpCompilation.Create("test", new[] { tree });
            var semanticModel = cSharpCompilation.GetSemanticModel(tree);

            var root = PathTreeBuilder.ConstructRootNode(new[] { identifierName }, semanticModel);
            root.Children.Keys.Should().Contain("Test");
            var invocationSyntax = root.DescendantNodes().Last().InvocationSyntax;
            invocationSyntax.IsSome.Should().Be(hasInvocation);
        }

        [TestCase("Stub")]
        [TestCase("InstanceOf")]
        public void TypeSymbols_is_set_for_provided_Method(string providedMethod)
        {
            var tree = CSharpSyntaxTree.ParseText(
                @$"
                   this.With(p => p.Test.{providedMethod}<IMyType>());
                   this.With(p => p.Test2.Value(5));
");

            var identifierNames = tree.GetRoot().DescendantNodesAndSelf().OfType<IdentifierNameSyntax>()
                .Where(syntax => syntax.Identifier.Text == "p");

            var cSharpCompilation = CSharpCompilation.Create("test", new[] { tree });
            var semanticModel = cSharpCompilation.GetSemanticModel(tree);

            var root = PathTreeBuilder.ConstructRootNode(identifierNames, semanticModel);

            var typeSymbol = root["Test"].TypeSymbol;
            typeSymbol.IsSome.Should().BeTrue();
            typeSymbol.GetValueUnsafe().Name.Should().Be("IMyType");

            root["Test2"].TypeSymbol.IsSome.Should().BeFalse();
        }

        [Test]
        public void TypeSymbols_is_set_for_last_node_in_path()
        {
            var tree = CSharpSyntaxTree.ParseText(
                @$"this.With(p => p.Level1.Level2.InstanceOf<IMyType>());");

            var identifierName = tree.GetRoot()
                .DescendantNodesAndSelf()
                .OfType<IdentifierNameSyntax>()
                .Single(syntax => syntax.Identifier.Text == "p");

            var cSharpCompilation = CSharpCompilation.Create("test", new[] { tree });
            var semanticModel = cSharpCompilation.GetSemanticModel(tree);

            var root = PathTreeBuilder.ConstructRootNode(new[] { identifierName }, semanticModel);

            root["Level1"].TypeSymbol.IsSome.Should().BeFalse();

            var typeSymbol = root["Level1"]["Level2"].TypeSymbol;
            typeSymbol.IsSome.Should().BeTrue();
            typeSymbol.GetValueUnsafe().Name.Should().Be("IMyType");
        }
    }
}
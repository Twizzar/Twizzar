using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn.DocumentReader;
using Twizzar.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Roslyn.DocumentReader;

[TestFixture]
public class ItemBuilderFinderTests
{
    [Test]
    public void Simple_ItemBuilder_is_found_correctly()
    {
        var code = 
            @"
using Twizzar.Fixture;

var b = new ItemBuilder<int>();
";

        var (root, sm) = GetTreeAndSemanticModel(code);
        var sut = new ItemBuilderFinder(sm);

        var result = sut.FindBuilderInformation(root);

        result.Should().HaveCount(1);
        var (_, fixtureItemType, isCustomBuilder) = result.First();
        fixtureItemType.Name.Should().Be("Int32");
        isCustomBuilder.Should().BeFalse();
    }

    [Test]
    public void Custom_ItemBuilder_is_found_correctly()
    {
        var code =
            @"
using Twizzar.Fixture;

var b = new MyCustomBuilder();

class MyCustomBuilder : ItemBuilder<int> { }
";

        var (root, sm) = GetTreeAndSemanticModel(code);
        var sut = new ItemBuilderFinder(sm);

        var result = sut.FindBuilderInformation(root);

        result.Should().HaveCount(1);
        var (_, fixtureItemType, isCustomBuilder) = result.First();
        fixtureItemType.Name.Should().Be("Int32");
        isCustomBuilder.Should().BeTrue();
    }

    [Test]
    public void BuilderCreation_without_parentheses_are_not_found()
    {
        // arrange
        var code = @"
using Twizzar.Fixture;

var b = new ItemBuilder<int>;
";
        var (root, sm) = GetTreeAndSemanticModel(code);
        var sut = new ItemBuilderFinder(sm);

        // act
        var result = sut.FindBuilderInformation(root);

        // assert
        result.Should().BeEmpty();
    }
        
    private static (SyntaxNode root, SemanticModel semanticModel) GetTreeAndSemanticModel(string source)
    {
        var compilation = CreateCompilation(source);
        var tree = compilation.SyntaxTrees.First();
        var semanticModel = compilation.GetSemanticModel(tree);

        return (tree.GetRoot(), semanticModel);
    }

    private static Compilation CreateCompilation(string source)
        => CSharpCompilation.Create("compilation",
            new[] { CSharpSyntaxTree.ParseText(source) },
            new[]
            {
                MetadataReference.CreateFromFile(typeof(int).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ItemBuilder<>).Assembly.Location),
            },
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));
}
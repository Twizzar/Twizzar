using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;
using Twizzar.Design.Shared.Infrastructure.Discovery;
using Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations;
using Verify = TwizzarInternal.Fixture.Verify;

namespace Twizzar.Analyzer2019.App.Tests;

[TestFixture]
public class DiscovererTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<Discoverer>().ShouldThrowArgumentNullException();
    }

    [Test]
    public void CustomBuilder_gets_discovered_correctly()
    {
        // arrange
        var sut = new Discoverer();

        var pathType = "TestPaths";
        var (root, semanticModel) = GetTreeAndSemanticModel(
            @$"
using System;
using Twizzar.Fixture;

public class CustomBuilder : ItemBuilder<int, {pathType}> {{}}
");
        var factory = new SimpleOperationFactory(root, semanticModel, CancellationToken.None);

        // act
        var discoverCustomItemBuilder = sut.DiscoverCustomItemBuilder(factory.Init())
            .ToSimpleOperation().Evaluate().ToList();

        // assert
        discoverCustomItemBuilder
            .Should()
            .HaveCount(1);

        var pathProviderInformation = discoverCustomItemBuilder.First();

        pathProviderInformation.FixtureItemSymbol.Name.Should().Be("Int32");
        pathProviderInformation.NameSpace.Should().Be("");
        pathProviderInformation.TypeName.Should().Be(pathType);
    }

    [Test]
    public void ItemBuilder_ctor_gets_discovered_correctly()
    {
        // arrange
        var sut = new Discoverer();
        var (root, semanticModel) = GetTreeAndSemanticModel(
            @"
using Twizzar.Fixture;

new ItemBuilder<int>();
");

        var node = root
            .DescendantNodesAndSelf()
            .OfType<ObjectCreationExpressionSyntax>()
            .First();

        var factory = new SimpleOperationFactory(root, semanticModel, CancellationToken.None);

        // act
        var result = sut.DiscoverItemBuilderCreation(factory.Init())
            .ToSimpleOperation().Evaluate();

        // assert

        var (itemBuilderCreationInformation, pathProviderInformation) = result.Single();

        pathProviderInformation.FixtureItemSymbol.Name.Should().Be("Int32");
        pathProviderInformation.NameSpace.Should().Be("Twizzar.Fixture");
        pathProviderInformation.TypeName.Should().Be("Int32Path");

        itemBuilderCreationInformation.Symbol.Name.Should().Be("Int32");
        itemBuilderCreationInformation.ObjectCreationExpression.Should().Be(node);
        itemBuilderCreationInformation.FixtureItemFullName.Should().Be("System.Int32");
        itemBuilderCreationInformation.PathProviderName.Should().Be("Twizzar.Fixture.Int32Path");
    }

    [Test]
    public void Lambda_member_selection_gets_discovered_correctly()
    {
        // arrange
        var sut = new Discoverer();
        var (root, semanticModel) = GetTreeAndSemanticModel(
            @"
using Twizzar.Fixture;

new ItemBuilder<int>().With(p => p.Member1.Member2.Value(3));
");

        var lambdaNode = root
            .DescendantNodesAndSelf()
            .OfType<LambdaExpressionSyntax>()
            .First();

        var expectedIdentifierNameSyntax = lambdaNode.DescendantNodesAndSelf()
            .OfType<IdentifierNameSyntax>()
            .Single(syntax => syntax.Identifier.Text == "p");

        var factory = new SimpleOperationFactory(root, semanticModel, CancellationToken.None);

        // act
        var result = sut.DiscoverMemberSelection(factory.Init())
            .ToSimpleOperation().Evaluate();

        // assert
        var (identifierNameSyntax, _, pathProviderInformation) = result.Single();

        identifierNameSyntax.Should().Be(expectedIdentifierNameSyntax);
        pathProviderInformation.FixtureItemSymbol.Name.Should().Be("Int32");
        pathProviderInformation.NameSpace.Should().Be("Twizzar.Fixture");
        pathProviderInformation.TypeName.Should().Be("Int32Path");
    }

    [Test]
    public void For_CustomBuilder_lambada_in_verify_get_discovered()
    {
        // arrange
        var sut = new Discoverer();
        var (root, semanticModel) = GetTreeAndSemanticModel(
            @$"
using System;
using Twizzar.Fixture;

namespace MyNamespace;

class MyPathProvider : PathProvider<int> {{ }}

class MyBuilder : ItemBuilder<int, MyPathProvider> {{}}

class Test
{{
    void Method()
    {{
        var _= new MyBuilder().Build(out var s);

        s.Verify(p => p.Member1.Member2);
    }}
}}
");

        var lambdaNode = root
            .DescendantNodesAndSelf()
            .OfType<LambdaExpressionSyntax>()
            .First();

        var expectedIdentifierNameSyntax = lambdaNode.DescendantNodesAndSelf()
            .OfType<IdentifierNameSyntax>()
            .Single(syntax => syntax.Identifier.Text == "p");

        var factory = new SimpleOperationFactory(root, semanticModel, CancellationToken.None);

        // act
        var result = sut.DiscoverMemberSelection(factory.Init())
            .ToSimpleOperation().Evaluate();

        // assert
        var (identifierNameSyntax, _, pathProviderInformation) = result.Single();

        identifierNameSyntax.Should().Be(expectedIdentifierNameSyntax);
        pathProviderInformation.FixtureItemSymbol.Name.Should().Be("Int32");
        pathProviderInformation.TypeName.Should().Be("MyPathProvider");
        pathProviderInformation.NameSpace.Should().Be("MyNamespace");
    }

    [Test]
    public void With_invoked_on_method_is_discovered()
    {
        // arrange
        var sut = new Discoverer();
        var (root, semanticModel) = GetTreeAndSemanticModel(
            @$"
using System;
using Twizzar.Fixture;

namespace MyNamespace;

class MyPathProvider : PathProvider<int> {{ }}

class MyBuilder : ItemBuilder<int, MyPathProvider> {{}}

class Test
{{
    MyBuilder GetBuilder() => new MyBuilder();

    void Method()
    {{
        var _ = GetBuilder()
            .With(p => p.Member1.Member2)
            .Build();
    }}
}}
");

        var lambdaNode = root
            .DescendantNodesAndSelf()
            .OfType<LambdaExpressionSyntax>()
            .First();

        var expectedIdentifierNameSyntax = lambdaNode.DescendantNodesAndSelf()
            .OfType<IdentifierNameSyntax>()
            .Single(syntax => syntax.Identifier.Text == "p");

        var factory = new SimpleOperationFactory(root, semanticModel, CancellationToken.None);

        // act
        var result = sut.DiscoverMemberSelection(factory.Init())
            .ToSimpleOperation().Evaluate();

        // assert
        var (identifierNameSyntax, _, pathProviderInformation) = result.Single();

        identifierNameSyntax.Should().Be(expectedIdentifierNameSyntax);
        pathProviderInformation.FixtureItemSymbol.Name.Should().Be("Int32");
        pathProviderInformation.TypeName.Should().Be("MyPathProvider");
        pathProviderInformation.NameSpace.Should().Be("MyNamespace");
    }

    [Test]
    public void CustomBuilder_with_many_is_discovered()
    {
        // arrange
        var sut = new Discoverer();
        var (root, semanticModel) = GetTreeAndSemanticModel(
            @$"
using System;
using Twizzar.Fixture;

namespace MyNamespace;

class MyPathProvider : PathProvider<int> {{ }}

class MyBuilder : ItemBuilder<int, MyPathProvider> {{}}

class Test
{{
    void Method()
    {{
        var _ = new MyBuilder()
            .With(p => p.Member3.Value(3))
            .With(p => p.Member1.InstanceOf<Car>())
            .Build();
    }}
}}
");

        var factory = new SimpleOperationFactory(root, semanticModel, CancellationToken.None);

        // act
        var result = sut.DiscoverMemberSelection(factory.Init())
            .ToSimpleOperation().Evaluate().ToList();

        // assert
        result.Should().HaveCount(2);
        foreach (var (_, _, pathProviderInformation) in result)
        {
            pathProviderInformation.FixtureItemSymbol.Name.Should().Be("Int32");
            pathProviderInformation.TypeName.Should().Be("MyPathProvider");
            pathProviderInformation.NameSpace.Should().Be("MyNamespace");
        }
    }

    [Test]
    public void With_invoked_on_method_many_is_discovered()
    {
        // arrange
        var sut = new Discoverer();
        var (root, semanticModel) = GetTreeAndSemanticModel(
            @"
using System;
using Twizzar.Fixture;

namespace MyNamespace;

class A<T> {{}}

class Test
{{
    void Method()
    {{
        var _ = new ItemBuilder<A<int>>()
            .With(p => p.Member1.Value(3))
            .With(p => p.Member1.Member2.Value(3))
            .Build();
    }}
}}
");

        var factory = new SimpleOperationFactory(root, semanticModel, CancellationToken.None);

        // act
        var result = sut.DiscoverMemberSelection(factory.Init())
            .ToSimpleOperation().Evaluate().ToList();

        // assert
        result.Should().HaveCount(2);
        foreach (var (_, _, pathProviderInformation) in result)
        {
            pathProviderInformation.FixtureItemSymbol.Name.Should().Be("A");
            pathProviderInformation.TypeName.Should().Be("AInt32Path");
            pathProviderInformation.NameSpace.Should().Be("Twizzar.Fixture");
        }
    }

    [Test]
    public void With_in_customBuilder_is_discovered()
    {
        // arrange
        var sut = new Discoverer();
        var (root, semanticModel) = GetTreeAndSemanticModel(
            @"
using System;
using Twizzar.Fixture;

namespace MyNamespace.Test;

class MyBuilder : ItemBuilder<int, MyPathProvider> {{
    MyBuilder()
    {{
        this.With(p => p.Member1.Member2);
    }}
}}
");

        var lambdaNode = root
            .DescendantNodesAndSelf()
            .OfType<LambdaExpressionSyntax>()
            .First();

        var expectedIdentifierNameSyntax = lambdaNode.DescendantNodesAndSelf()
            .OfType<IdentifierNameSyntax>()
            .Single(syntax => syntax.Identifier.Text == "p");

        var factory = new SimpleOperationFactory(root, semanticModel, CancellationToken.None);

        // act
        var result = sut.DiscoverMemberSelection(factory.Init())
            .ToSimpleOperation().Evaluate();

        // assert
        var (identifierNameSyntax, _, pathProviderInformation) = result.Single();

        identifierNameSyntax.Should().Be(expectedIdentifierNameSyntax);
        pathProviderInformation.FixtureItemSymbol.Name.Should().Be("Int32");
        pathProviderInformation.TypeName.Should().Be("MyPathProvider");
        pathProviderInformation.NameSpace.Should().Be("MyNamespace.Test");
    }


    [Test]
    public void Local_member_selection_gets_discovered_correctly()
    {
        // arrange
        var sut = new Discoverer();
        var (root, semanticModel) = GetTreeAndSemanticModel(
            @"
using Twizzar.Fixture;

var p = new MyPathProvider();
var p2 = p.Member1.Member2;

class MyPathProvider : PathProvider<int> {{ }}
");

        var expectedIdentifierNameSyntax = root
            .DescendantNodesAndSelf()
            .OfType<IdentifierNameSyntax>()
            .First(syntax => syntax.Identifier.Text == "p");

        var factory = new SimpleOperationFactory(root, semanticModel, CancellationToken.None);


        // act
        var result = sut.DiscoverMemberSelection(factory.Init())
            .ToSimpleOperation().Evaluate();

        // assert
        var (identifierNameSyntax, _, pathProviderInformation) = result.Single();

        identifierNameSyntax.Should().Be(expectedIdentifierNameSyntax);
        pathProviderInformation.FixtureItemSymbol.Name.Should().Be("Int32");
        pathProviderInformation.NameSpace.Should().Be("");
        pathProviderInformation.TypeName.Should().Be("MyPathProvider");
    }

    private static (SyntaxNode root, SemanticModel semanticModel) GetTreeAndSemanticModel(string source)
    {
        var compilation = CreateCompilation(source);
        foreach (var diagnostic in compilation.GetDiagnostics())
        {
            Console.WriteLine(diagnostic);
        }
        var tree = compilation.SyntaxTrees.First();
        var semanticModel = compilation.GetSemanticModel(tree);

        return (tree.GetRoot(), semanticModel);
    }

    private static Compilation CreateCompilation(string source)
        => CSharpCompilation.Create("compilation",
            new[] { CSharpSyntaxTree.ParseText(source) },
            new[]
            {
                MetadataReference.CreateFromFile(AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "netstandard").Location),
                MetadataReference.CreateFromFile(AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "System.Runtime").Location),
                MetadataReference.CreateFromFile(typeof(int).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Binder).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Fixture.ItemBuilder<>).Assembly.Location),
            },
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
}
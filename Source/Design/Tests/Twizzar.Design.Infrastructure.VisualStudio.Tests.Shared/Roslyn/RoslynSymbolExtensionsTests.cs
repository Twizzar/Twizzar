using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Moq;
using NUnit.Framework;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.Design.Shared.Infrastructure.Extension;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Roslyn;

[Category("TwizzarInternal")]
public class RoslynSymbolExtensionsTests
{
    private INamespaceSymbol _containingNameSpace;
    private const string TestNamespace = "Random.NameSpace";
    private const string ContainingAssembly = "containigAssembly";

    [SetUp]
    public void Setup()
    {
        this._containingNameSpace = Mock.Of<INamespaceSymbol>(symbol => symbol.ToString() == TestNamespace);
    }

    private ITypeSymbol CreateTypeSymbol(string typeName) =>
        new NamedTypeSymbolBuilder()
            .WithName(typeName)
            .WithContainingNamespace(this._containingNameSpace)
            .WithContainingAssembly(Mock.Of<IAssemblySymbol>(symbol => symbol.ToString() == ContainingAssembly))
            //.WithBasetype(Mock.Of<INamedTypeSymbol>(symbol => symbol.BaseType == Mock.Of<INamedTypeSymbol>()))
            .Build();

    [Test]
    public void Namespace_and_metadataName_are_combined_correctly()
    {
        // arrange
        var typeSymbol = this.CreateTypeSymbol("TestName");

        // act
        var fullTypeName = typeSymbol.GetTypeFullName();

        // assert
        fullTypeName.FullName.Should().Be(TestNamespace + ".TestName");
    }

    [Test]
    public void GetBaseTypesAndThis_resolves_base_types_correctly()
    {
        // arrange
        var typeSymbol = new NamedTypeSymbolBuilder()
            .WithBasetype(new NamedTypeSymbolBuilder()
                .WithBasetype(new NamedTypeSymbolBuilder()
                    .Build())
                .Build())
            .Build();

        var baseTypes = typeSymbol.GetBaseTypesAndThis();

        baseTypes.Count().Should().Be(3);
    }

    [Test]
    public void InheritsFromOrEquals_returns_true_if_type_inherits_from_basetype()
    {
        // arrange
        var typeSymbol = this.CreateTypeSymbol("TestName");
        var baseTypeSymbol = new NamedTypeSymbolBuilder().Build();

        var typeSymbolMock = Mock.Get(typeSymbol);
        var baseTypeSymbolMock = Mock.Get(baseTypeSymbol);
        typeSymbolMock.Setup(symbol => symbol.BaseType).Returns(baseTypeSymbol);
        baseTypeSymbolMock.Setup(symbol => symbol.Equals(baseTypeSymbol, It.IsAny<SymbolEqualityComparer>()))
            .Returns(true);

        // act
        var isInheritedFrom = typeSymbol.InheritsFromOrEquals(baseTypeSymbol);

        // assert
        isInheritedFrom.Should().BeTrue();
    }

    [Test]
    public void InheritsFromOrEquals_returns_false_if_type_does_not_inherit_from_basetype()
    {
        // arrange
        var typeSymbol = this.CreateTypeSymbol("TestName");
        var baseTypeSymbol = new NamedTypeSymbolBuilder().Build();

        // act
        var isInheritedFrom = typeSymbol.InheritsFromOrEquals(baseTypeSymbol);

        // assert
        isInheritedFrom.Should().BeFalse();
    }

    [Test]
    public void InheritsFromOrEquals_returns_false_if_basetype_is_null()
    {
        // arrange
        var typeSymbol = this.CreateTypeSymbol("TestName");

        // act
        var isInheritedFrom = typeSymbol.InheritsFromOrEquals((ITypeSymbol)null);

        // assert
        isInheritedFrom.Should().BeFalse();
    }

    [Test]
    public void TypeFullName_is_returned_correctly_for_generics()
    {
        var typeArgumentNames = Repeat(() => RandomString(), RandomInt(1, 6))
            .ToImmutableArray();

        var typeArguments = typeArgumentNames
            .Select(this.CreateTypeSymbol)
            .ToImmutableArray();

        var typeSymbol = Mock.Of<INamedTypeSymbol>(symbol =>
            symbol.Name == "TestName" &&
            symbol.ContainingNamespace == this._containingNameSpace &&
            symbol.TypeArguments == typeArguments);

        var expectedGenericPart =
            "[" + 
            string.Join(",", typeArgumentNames.Select(typeName => $"[{TestNamespace}.{typeName}, {ContainingAssembly}]")) + 
            "]";

        // act
        var fullTypeName = typeSymbol.GetTypeFullName();

        // assert
        fullTypeName.FullName.Should().Be(TestNamespace + $".TestName`{typeArgumentNames.Length}" + expectedGenericPart);
    }
}
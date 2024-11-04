using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Moq;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;

public class NamedTypeSymbolBuilder
{
    private readonly Mock<INamedTypeSymbol> _mock = new();

    private string _name;
    private string _nameSpace;

    public NamedTypeSymbolBuilder()
    {
        this._name = "tetName";
        this._nameSpace = "testNamespace";
    }

    public NamedTypeSymbolBuilder WithGlobalNamespace()
    {
        this._mock.Setup(symbol => symbol.ContainingNamespace)
            .Returns(Mock.Of<INamespaceSymbol>(symbol => symbol.IsGlobalNamespace == true));

        return this;
    }

    public NamedTypeSymbolBuilder WithNamespace(string ns)
    {
        this._nameSpace = ns;
        return this;
    }

    public NamedTypeSymbolBuilder WithName(string name)
    {
        this._name = name;

        return this;
    }

    public NamedTypeSymbolBuilder WithContainingNamespace(INamespaceSymbol containingNamespaceSymbol)
    {
        this._mock
            .Setup(symbol => symbol.ContainingNamespace)
            .Returns(containingNamespaceSymbol);
        return this;
    }


    public NamedTypeSymbolBuilder WithContainingAssembly(IAssemblySymbol assemblySymbol)
    {
        this._mock
            .Setup(symbol => symbol.ContainingAssembly)
            .Returns(assemblySymbol);
        return this;
    }

    public NamedTypeSymbolBuilder WithBasetype(INamedTypeSymbol symbol)
    {
        this._mock
            .Setup(symbol => symbol.BaseType)
            .Returns(symbol);

        return this;
    }

    public INamedTypeSymbol Build()
    {
        this._mock.Setup(symbol => symbol.Name)
            .Returns(this._name);

        this._mock.Setup(symbol => symbol.MetadataName)
            .Returns(this._name);

        this._mock.Setup(symbol => symbol.TypeArguments)
            .Returns(() => ImmutableArray<ITypeSymbol>.Empty);

        this._mock.Setup(symbol => symbol.AllInterfaces)
            .Returns(ImmutableArray<INamedTypeSymbol>.Empty);

        return this._mock.Object;
    }
}
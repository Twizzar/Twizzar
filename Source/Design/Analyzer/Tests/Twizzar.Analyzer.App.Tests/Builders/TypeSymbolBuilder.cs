using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Moq;

namespace Twizzar.Analyzer.Core.Tests.Builders
{
    public class TypeSymbolBuilder
    {
        private readonly Mock<ITypeSymbol> _mock = new Mock<ITypeSymbol>();
        private ImmutableArray<ISymbol> _members = ImmutableArray<ISymbol>.Empty;

        public TypeSymbolBuilder()
        {
            this._mock
                .Setup(symbol => symbol.GetMembers())
                .Returns(() => ImmutableArray<ISymbol>.Empty);

            this._mock.Setup(symbol => symbol.AllInterfaces)
                .Returns(() => ImmutableArray<INamedTypeSymbol>.Empty);
        }

        public TypeSymbolBuilder WithGlobalNamespace()
        {
            this._mock.Setup(symbol => symbol.ContainingNamespace)
                .Returns(Mock.Of<INamespaceSymbol>(symbol => symbol.IsGlobalNamespace == true));

            return this;
        }

        public TypeSymbolBuilder WithNamespace(string ns)
        {
            this._mock.Setup(symbol => symbol.ContainingNamespace)
                .Returns(Mock.Of<INamespaceSymbol>(symbol => symbol.IsGlobalNamespace == false &&
                                                             symbol.Name == ns &&
                                                             symbol.ToString() == ns &&
                                                             symbol.ToDisplayString(It.IsAny<SymbolDisplayFormat>()) == ns));

            return this;
        }

        public TypeSymbolBuilder WithMembers(params ISymbol[] symbols)
        {
            this._members = symbols.ToImmutableArray();
            return this;
        }

        public TypeSymbolBuilder WithName(string name)
        {
            this._mock.Setup(symbol => symbol.Name)
                .Returns(name);

            this._mock.Setup(symbol => symbol.MetadataName)
                .Returns(name);

            this._mock
                .Setup(symbol => symbol.GetMembers())
                .Returns(() => this._members);

            return this;
        }

        public ITypeSymbol Build() =>
            this._mock.Object;
    }
}
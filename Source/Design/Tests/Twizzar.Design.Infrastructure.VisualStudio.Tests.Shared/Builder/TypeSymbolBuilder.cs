using Microsoft.CodeAnalysis;

using Moq;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Builders
{
    public class NamedTypeSymbolBuilder
    {
        private readonly Mock<ITypeSymbol> _mock = new Mock<ITypeSymbol>();

        public NamedTypeSymbolBuilder WithGlobalNamespace()
        {
            _mock.Setup(symbol => symbol.ContainingNamespace)
                .Returns(Mock.Of<INamespaceSymbol>(symbol => symbol.IsGlobalNamespace == true));

            return this;
        }

        public NamedTypeSymbolBuilder WithNamespace(string ns)
        {
            _mock.Setup(symbol => symbol.ContainingNamespace)
                .Returns(Mock.Of<INamespaceSymbol>(symbol => symbol.IsGlobalNamespace == false &&
                                                             symbol.Name == ns &&
                                                             symbol.ToDisplayString(It.IsAny<SymbolDisplayFormat>()) == ns));

            return this;
        }

        public NamedTypeSymbolBuilder WithName(string name)
        {
            _mock.Setup(symbol => symbol.Name)
                .Returns(name);

            _mock.Setup(symbol => symbol.MetadataName)
                .Returns(name);

            return this;
        }

        public ITypeSymbol Build() =>
            _mock.Object;
    }
}
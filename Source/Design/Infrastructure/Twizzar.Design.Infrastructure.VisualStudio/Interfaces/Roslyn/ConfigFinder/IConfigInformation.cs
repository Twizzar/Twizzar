using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <summary>
    /// Information about the config class.
    /// </summary>
    public interface IBuilderInformation
    {
        /// <summary>
        /// Gets the config class symbol.
        /// </summary>
        public ITypeSymbol ConfigClassSymbol { get; }

        /// <summary>
        /// Gets the item config symbol.
        /// </summary>
        public INamedTypeSymbol ItemConfigSymbol { get; }

        /// <summary>
        /// Gets the fixture item symbols.
        /// </summary>
        public ITypeSymbol FixtureItemSymbol { get; }

        /// <summary>
        /// Gets the syntax node of the class declaration.
        /// </summary>
        public ClassDeclarationSyntax Syntax { get; }
    }
}
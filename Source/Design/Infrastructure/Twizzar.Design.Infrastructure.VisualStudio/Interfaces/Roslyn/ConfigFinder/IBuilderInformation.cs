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
        ITypeSymbol CustomItemBuilderSymbol { get; init; }

        /// <summary>
        /// Gets the item config symbol.
        /// </summary>
        INamedTypeSymbol ItemBuilderSymbol { get; init; }

        /// <summary>
        /// Gets the fixture item symbols.
        /// </summary>
        ITypeSymbol FixtureItemSymbol { get; init; }

        /// <summary>
        /// Gets the object creation syntax: like <c>new MyCustomBuilder()</c>.
        /// </summary>
        ObjectCreationExpressionSyntax ObjectCreationExpression { get; init; }

        /// <summary>
        /// Gets the syntax node of the class declaration.
        /// </summary>
        ClassDeclarationSyntax ClassDeclarationSyntax { get; init; }
    }
}
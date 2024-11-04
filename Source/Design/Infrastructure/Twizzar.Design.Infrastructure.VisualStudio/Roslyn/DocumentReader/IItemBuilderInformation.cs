using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn.DocumentReader
{
    /// <summary>
    /// Information about an ItemBuilder found in code. This can be of type ItemBuilder or a type derived form it.
    /// </summary>
    public interface IItemBuilderInformation
    {
        /// <summary>
        /// Gets the fixture item type.
        /// </summary>
        ITypeSymbol FixtureItemType { get; init; }

        /// <summary>
        /// Gets a value indicating whether the ItemBuilder is a custom builder.
        /// </summary>
        bool IsCustomBuilder { get; init; }

        /// <summary>
        /// Gets the object creation expression.
        /// </summary>
        ObjectCreationExpressionSyntax ObjectCreationExpression { get; init; }

        /// <summary>
        /// Deconstructor.
        /// </summary>
        /// <param name="ObjectCreationExpression"></param>
        /// <param name="FixtureItemType"></param>
        /// <param name="IsCustomBuilder"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "For record deconstruction.")]
        void Deconstruct(out ObjectCreationExpressionSyntax ObjectCreationExpression, out ITypeSymbol FixtureItemType, out bool IsCustomBuilder);

        /// <summary>
        /// Checks if two information are equal.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>Tre if equal.</returns>
        bool Equals(ItemBuilderInformation other);
    }
}
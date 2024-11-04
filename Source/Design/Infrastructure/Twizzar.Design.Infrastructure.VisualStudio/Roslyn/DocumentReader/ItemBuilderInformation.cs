using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn.DocumentReader
{
    /// <summary>
    /// Information about a found ItemBuilder creation like <c>new ItemBuilder&lt;MyType&gt;()</c> or <c>new MyCustomItemBuilder()</c>.
    /// </summary>
    /// <param name="ObjectCreationExpression">The object creation expression found.</param>
    /// <param name="FixtureItemType">The fixture item type of the ItemBuilder. This is the first type argument of the ItemBuilder.</param>
    /// <param name="IsCustomBuilder">If the builder is a custom builder and therefore not of the type ItemBuilder&lt;T&gt;.</param>
    [ExcludeFromCodeCoverage]
    public record ItemBuilderInformation(
        ObjectCreationExpressionSyntax ObjectCreationExpression,
        ITypeSymbol FixtureItemType,
        bool IsCustomBuilder) : IItemBuilderInformation;
}
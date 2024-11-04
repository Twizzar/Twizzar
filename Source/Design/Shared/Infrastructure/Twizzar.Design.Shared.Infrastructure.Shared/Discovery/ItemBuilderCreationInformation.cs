using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Twizzar.Design.Shared.Infrastructure.Discovery
{
    /// <summary>
    /// Information about an ItemBuilder creation like <c>new ItemBuilder&lt;MyType&gt;()</c>.
    /// </summary>
    /// <param name="FixtureItemFullName">The type full name of the fixture item (MyType).</param>
    /// <param name="ObjectCreationExpression">The object creation expression (<c>new ItemBuilder&lt;MyType&gt;()</c>).</param>
    /// <param name="Symbol">The symbol of the fixture item (MyType).</param>
    /// <param name="PathProviderName">The name of the path provider (MyTypePaths).</param>
    [ExcludeFromCodeCoverage]
    public record ItemBuilderCreationInformation(
        string FixtureItemFullName,
        ObjectCreationExpressionSyntax ObjectCreationExpression,
        ITypeSymbol Symbol,
        string PathProviderName);
}
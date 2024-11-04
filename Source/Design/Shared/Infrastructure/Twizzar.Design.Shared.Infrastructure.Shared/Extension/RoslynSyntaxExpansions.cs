using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Shared.Infrastructure.Shared.Extension
{
    /// <summary>
    /// Expansion method for types in the namespace Microsoft.CodeAnalysis.
    /// </summary>
    public static class RoslynSyntaxExpansions
    {
        /// <summary>
        /// Get the sibling nodes form a <see cref="SyntaxNode"/>.
        /// This is the same like: <c>node.Parent.ChildNodes()</c>.
        /// </summary>
        /// <param name="self"></param>
        /// <returns>All siblings returns a empty sequence when the parent is null.</returns>
        public static IEnumerable<SyntaxNode> Siblings(this SyntaxNode self) =>
            Maybe.ToMaybe(self.Parent)
                .Map(parent => parent.ChildNodes())
                .SomeOrProvided(Enumerable.Empty<SyntaxNode>);
    }
}

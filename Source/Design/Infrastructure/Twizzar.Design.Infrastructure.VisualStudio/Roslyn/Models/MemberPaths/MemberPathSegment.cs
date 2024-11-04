using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn.Models
{
    /// <summary>
    /// Represents all segment paths which inherit form <see cref="IPathSegment"/>.
    /// </summary>
    public class MemberPathSegment : IPathSegment
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberPathSegment"/> class.
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="parent"></param>
        /// <param name="symbol"></param>
        /// <param name="returnSymbol"></param>
        /// <param name="identifierNameSyntax"></param>
        public MemberPathSegment(
            string memberName,
            Maybe<IPathSegment> parent,
            INamedTypeSymbol symbol,
            ITypeSymbol returnSymbol,
            SimpleNameSyntax identifierNameSyntax)
        {
            this.MemberName = memberName;
            this.Parent = parent;
            this.Symbol = symbol;
            this.ReturnSymbol = returnSymbol;
            this.IdentifierNameSyntax = identifierNameSyntax;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public string MemberName { get; }

        /// <inheritdoc />
        public string Path => this.Parent.Map(item => $"{item.Path}.").SomeOrProvided(string.Empty) + this.MemberName;

        /// <inheritdoc />
        public Maybe<IPathSegment> Parent { get; }

        /// <summary>
        /// Gets the symbol of the path. Represents the path type. This type inherits form or is of the type <see cref="MemberPathSegment"/>.
        /// </summary>
        public INamedTypeSymbol Symbol { get; }

        /// <summary>
        /// Gets the return type (TReturnType) of the <see cref="IPathSegment"/>.
        /// </summary>
        public ITypeSymbol ReturnSymbol { get; }

        /// <inheritdoc />
        public SimpleNameSyntax IdentifierNameSyntax { get; }

        #endregion

        #region members

        /// <summary>
        /// Check if the path is of the type MethodPath or inherits form it.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="methodSymbol"></param>
        /// <returns>True if it assignable to MethodPath.</returns>
        public bool IsMethodPath(IRoslynContext context, ITypeSymbol methodSymbol) =>
            context.Compilation.ClassifyCommonConversion(this.Symbol, methodSymbol).IsImplicit;

        #endregion
    }
}
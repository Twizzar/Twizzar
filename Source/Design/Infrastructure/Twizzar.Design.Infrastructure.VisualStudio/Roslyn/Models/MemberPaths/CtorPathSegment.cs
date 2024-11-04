using Microsoft.CodeAnalysis.CSharp.Syntax;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn.Models
{
    /// <summary>
    /// Represent a constructor segment of the path.
    /// </summary>
    public class CtorPathSegment : IPathSegment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CtorPathSegment"/> class.
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="parent"></param>
        /// <param name="identifierNameSyntax"></param>
        public CtorPathSegment(string memberName, Maybe<IPathSegment> parent, SimpleNameSyntax identifierNameSyntax)
        {
            this.MemberName = memberName;
            this.Parent = parent;
            this.IdentifierNameSyntax = identifierNameSyntax;
        }

        #region Implementation of IPathItem

        /// <inheritdoc />
        public string MemberName { get; }

        /// <inheritdoc />
        public string Path =>
            this.Parent.Map(item => $"{item.Path}.").SomeOrProvided(string.Empty) + this.MemberName;

        /// <inheritdoc />
        public Maybe<IPathSegment> Parent { get; }

        /// <inheritdoc />
        public SimpleNameSyntax IdentifierNameSyntax { get; }

        #endregion
    }
}
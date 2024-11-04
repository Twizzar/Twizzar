using Microsoft.CodeAnalysis.CSharp.Syntax;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn.Models
{
    /// <summary>
    /// Represents a segment of a <see cref="RoslynMemberPath"/>.
    /// </summary>
    public interface IPathSegment
    {
        /// <summary>
        /// Gets the member name.
        /// </summary>
        public string MemberName { get; }

        /// <summary>
        /// Gets the full path to this item.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the parent if root returns None.
        /// </summary>
        public Maybe<IPathSegment> Parent { get; }

        /// <summary>
        /// Gets the identifier form the syntax tree that describes this path item in the config.
        /// </summary>
        public SimpleNameSyntax IdentifierNameSyntax { get; }
    }
}
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.Design.Shared.Infrastructure.PathTree;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Interfaces.Roslyn
{
    /// <summary>
    /// Finds member configurations of a ItemBuilder.
    /// </summary>
    public interface IRoslynMemberConfigurationFinder
    {
        /// <summary>
        /// Find the member configurations of a ItemBuilder and returns the selected path of the configuration
        /// as an tree.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="block">The block syntax which defines the scope to search in.</param>
        /// <param name="pathProviderName">The path provider name of the ItemPath, everything else will be filtered out.</param>
        /// <param name="objectCreationSyntax">
        /// Optional object creation syntax.
        /// This will be used to find the Invocation and only find configuration
        /// which are configured inside this invocation.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The root of the path tree.</returns>
        IPathNode FindMemberConfiguration(
            IRoslynContext context,
            SyntaxNode block,
            string pathProviderName,
            Maybe<ObjectCreationExpressionSyntax> objectCreationSyntax,
            CancellationToken cancellationToken);
    }
}
using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Twizzar.Design.Shared.Infrastructure.Discovery;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Analyzer.Core.Interfaces
{
    /// <summary>
    /// A cache for storing generated source code.
    /// </summary>
    public interface ISourceCodeCache
    {
        /// <summary>
        /// Get a cached value.
        /// </summary>
        /// <param name="pair">The lookup key.</param>
        /// <returns>Some when the value was found; else none.</returns>
        Maybe<SourceText> GetCached(KeyValuePair<PathProviderInformation, List<IdentifierNameSyntax>> pair);

        /// <summary>
        /// Update the cache.
        /// This will inserts a new value if a entry with the node does not exists; else it updates the entry.
        /// </summary>
        /// <param name="pair">The lookup key.</param>
        /// <param name="sourceText">The value to store.</param>
        void UpdateCache(KeyValuePair<PathProviderInformation, List<IdentifierNameSyntax>> pair, SourceText sourceText);
    }
}
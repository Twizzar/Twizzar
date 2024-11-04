using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Twizzar.Analyzer.Core.Interfaces;
using Twizzar.Design.Shared.Infrastructure.Discovery;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Analyzer.Core.SourceTextGenerators
{
    /// <inheritdoc cref="ISourceCodeCache"/>
    public class SourceCodeCache : ISourceCodeCache
    {
        #region static fields and constants

        private const int DefaultCapacity = 1000;

        #endregion

        #region fields

        private readonly int _capacity;

        private readonly Dictionary<int, SourceText> _cache =
            new();

        private readonly Queue<int> _queue;

        private readonly object _lock = new();

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceCodeCache"/> class.
        /// </summary>
        public SourceCodeCache()
            : this(DefaultCapacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceCodeCache"/> class.
        /// </summary>
        /// <param name="capacity"></param>
        public SourceCodeCache(int capacity)
        {
            EnsureHelper.GetDefault.Parameter(capacity, nameof(capacity))
                .IsGreaterEqualThan(1)
                .ThrowOnFailure();

            this._capacity = capacity;
            this._queue = new Queue<int>(this._capacity);
        }

        #endregion

        #region members

        /// <inheritdoc />
        public Maybe<SourceText> GetCached(KeyValuePair<PathProviderInformation, List<IdentifierNameSyntax>> pair) =>
            this._cache.GetMaybe(CalculateHash(pair));

        /// <inheritdoc />
        public void UpdateCache(
            KeyValuePair<PathProviderInformation, List<IdentifierNameSyntax>> pair,
            SourceText sourceText)
        {
            lock (this._lock)
            {
                var hash = CalculateHash(pair);

                if (this._cache.ContainsKey(hash))
                {
                    this._cache[hash] = sourceText;
                    return;
                }

                if (this._queue.Count >= this._capacity)
                {
                    this._cache.Remove(this._queue.Dequeue());
                }

                this._queue.Enqueue(hash);
                this._cache.Add(hash, sourceText);
            }
        }

        private static int CalculateHash(KeyValuePair<PathProviderInformation, List<IdentifierNameSyntax>> pair) =>
            pair.Key.GetHashCode() ^ pair.Value.GetHashCodeOfElements();

        #endregion
    }
}
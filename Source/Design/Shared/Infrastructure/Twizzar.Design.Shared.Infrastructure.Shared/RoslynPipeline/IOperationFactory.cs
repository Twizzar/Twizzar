using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline
{
    /// <summary>
    /// Factory for creating an operation in the RoslynPipeline.
    /// </summary>
    public interface IOperationFactory
    {
        #region members

        /// <summary>
        /// Creates a initial operation.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="transform"></param>
        /// <returns>.</returns>
        public IValuesOperation<TSource> Init<TSource>(
            Func<SyntaxNode, CancellationToken, bool> predicate,
            Func<(SyntaxNode Node, SemanticModel SemanticModel), CancellationToken, TSource> transform);

        /// <summary>
        /// Creates a initial operation.
        /// </summary>
        /// <returns>.</returns>
        public IValuesOperation<(SyntaxNode Node, SemanticModel SemanticModel)> Init();

        /// <summary>
        /// Create a operation which get the compilation.
        /// </summary>
        /// <returns></returns>
        public IValueOperation<Compilation> GetCompilation();

        /// <summary>
        /// Create a where operation.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="prev"></param>
        /// <param name="predicate"></param>
        /// <returns>.</returns>
        public IValuesOperation<TSource> Where<TSource>(
            IValuesOperation<TSource> prev,
            Func<TSource, bool> predicate);

        /// <summary>
        /// Creates a select operation.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="prev"></param>
        /// <param name="selector"></param>
        /// <returns>.</returns>
        public IValuesOperation<TResult> Select<TSource, TResult>(
            IValuesOperation<TSource> prev,
            Func<TSource, CancellationToken, TResult> selector);

        /// <summary>
        /// Creates a select operation.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="prev"></param>
        /// <param name="selector"></param>
        /// <returns>.</returns>
        public IValueOperation<TResult> Select<TSource, TResult>(
            IValueOperation<TSource> prev,
            Func<TSource, CancellationToken, TResult> selector);

        /// <summary>
        /// Creates SelectMany operation.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="prev"></param>
        /// <param name="selector"></param>
        /// <returns>.</returns>
        public IValuesOperation<TResult> SelectMany<TSource, TResult>(
            IValuesOperation<TSource> prev,
            Func<TSource, CancellationToken, ImmutableArray<TResult>> selector);

        /// <summary>
        /// Creates SelectMany operation.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="prev"></param>
        /// <param name="selector"></param>
        /// <returns>.</returns>
        public IValuesOperation<TResult> SelectMany<TSource, TResult>(
            IValueOperation<TSource> prev,
            Func<TSource, CancellationToken, ImmutableArray<TResult>> selector);

        /// <summary>
        /// Creates a combine operation.
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="operation1"></param>
        /// <param name="operation2"></param>
        /// <returns>.</returns>
        public IValueOperation<(TLeft Left, TRight Right)> Combine<TLeft, TRight>(
            IValueOperation<TLeft> operation1,
            IValueOperation<TRight> operation2);

        /// <summary>
        /// Creates a combine operation.
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="operation1"></param>
        /// <param name="operation2"></param>
        /// <returns>.</returns>
        public IValuesOperation<(TLeft Left, TRight Right)> Combine<TLeft, TRight>(
            IValuesOperation<TLeft> operation1,
            IValueOperation<TRight> operation2);

        /// <summary>
        /// Create a collect operation.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <param name="source"></param>
        /// <returns>.</returns>
        public IValueOperation<ImmutableArray<TSource>> Collect<TSource>(IValuesOperation<TSource> source);

        #endregion
    }
}
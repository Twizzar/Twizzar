using System.Collections.Generic;
using System.Threading;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations
{
    /// <summary>
    /// A simple values operation which can be evaluated.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public interface ISimpleValuesOperation<out TSource> : IValuesOperation<TSource>
    {
        /// <summary>
        /// Gets the cancellation token to canceling the operation.
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// Evaluate the operation chain.
        /// </summary>
        /// <returns>The resulting value.</returns>
        IEnumerable<TSource> Evaluate();
    }

    /// <inheritdoc cref="ISimpleValuesOperation{TSource}" />
    public abstract record SimpleValuesOperation<TSource>(
        IOperationFactory OperationFactory,
        CancellationToken CancellationToken) : ISimpleValuesOperation<TSource>
    {
        /// <summary>
        /// Gets the comparer.
        /// </summary>
        protected IEqualityComparer<TSource> Comparer { get; init; } = EqualityComparer<TSource>.Default;

        /// <inheritdoc />
        public abstract IEnumerable<TSource> Evaluate();

        /// <inheritdoc />
        public IValuesOperation<TSource> WithComparer(IEqualityComparer<TSource> comparer) =>
            this with { Comparer = comparer };
    }
}
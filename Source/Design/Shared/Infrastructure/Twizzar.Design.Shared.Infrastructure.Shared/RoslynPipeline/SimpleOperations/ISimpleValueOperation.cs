using System.Collections.Generic;
using System.Threading;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations
{
    /// <summary>
    /// A simple value operation which can be evaluated.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public interface ISimpleValueOperation<out TSource> : IValueOperation<TSource>
    {
        /// <summary>
        /// Gets the cancellation token to canceling the operation.
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// Evaluate the operation chain.
        /// </summary>
        /// <returns>The resulting value.</returns>
        TSource Evaluate();
    }

    /// <inheritdoc cref="ISimpleValueOperation{TSource}" />
    public abstract record SimpleValueOperation<TSource>(
        IOperationFactory OperationFactory,
        CancellationToken CancellationToken) : ISimpleValueOperation<TSource>
    {
        /// <summary>
        /// Gets the comparer.
        /// </summary>
        protected IEqualityComparer<TSource> Comparer { get; init; } = EqualityComparer<TSource>.Default;

        /// <inheritdoc />
        public abstract TSource Evaluate();

        /// <inheritdoc />
        public IValueOperation<TSource> WithComparer(IEqualityComparer<TSource> comparer) =>
            this with { Comparer = comparer };
    }
}
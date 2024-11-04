using System.Collections.Generic;

namespace Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline
{
    /// <summary>
    /// An generic operation for a multiples values and for a pipeline style usage.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public interface IValuesOperation<out TSource>
    {
        /// <summary>
        /// Gets the operation factory for creating new operations.
        /// </summary>
        IOperationFactory OperationFactory { get; }

        /// <summary>
        /// Get a new <see cref="IValuesOperation{TSource}"/> with a custom comparer.
        /// </summary>
        /// <param name="comparer"></param>
        /// <returns>.</returns>
        IValuesOperation<TSource> WithComparer(IEqualityComparer<TSource> comparer);
    }
}
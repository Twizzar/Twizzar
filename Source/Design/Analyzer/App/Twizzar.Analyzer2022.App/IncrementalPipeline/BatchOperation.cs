using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Analyzer2022.App.IncrementalPipeline
{
    /// <summary>
    /// Batch together a value with a Sequence.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <param name="Left"></param>
    /// <param name="Right"></param>
    /// <param name="ValueProvider"></param>
    /// <param name="OperationFactory"></param>
    [ExcludeFromCodeCoverage]
    public record BatchOperation<TLeft, TRight>(
            IIncrementalValuesOperation<TLeft> Left,
            IIncrementalValueOperation<TRight> Right,
            IncrementalValuesProvider<(TLeft, TRight)> ValueProvider,
            IOperationFactory OperationFactory)
        : IncrementalValuesOperation<(TLeft Left, TRight Right)>(ValueProvider, OperationFactory)
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchOperation{TLeft,TRight}"/> class.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public BatchOperation(IIncrementalValuesOperation<TLeft> left, IIncrementalValueOperation<TRight> right)
            : this(left, right, left.ValueProvider.Combine(right.ValueProvider), left.OperationFactory)
        {
        }

        #endregion
    }
}
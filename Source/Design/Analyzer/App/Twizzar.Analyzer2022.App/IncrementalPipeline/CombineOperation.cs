using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Analyzer2022.App.IncrementalPipeline
{
    /// <summary>
    /// <see cref="OperationExtensionMethods.Combine{TLeft,TRight}(IValueOperation{TLeft},IValueOperation{TRight})"/>.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <param name="Left"></param>
    /// <param name="Right"></param>
    /// <param name="OperationFactory"></param>
    /// <param name="ValueProvider"></param>
    [ExcludeFromCodeCoverage]
    public record CombineOperation<TLeft, TRight>(
        IIncrementalValueOperation<TLeft> Left,
        IIncrementalValueOperation<TRight> Right,
        IOperationFactory OperationFactory,
        IncrementalValueProvider<(TLeft Left, TRight Right)> ValueProvider)
        : IncrementalValueOperation<(TLeft Left, TRight Right)>(ValueProvider, OperationFactory)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CombineOperation{TLeft,TRight}"/> class.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public CombineOperation(IIncrementalValueOperation<TLeft> left, IIncrementalValueOperation<TRight> right)
            : this(left, right, left.OperationFactory, left.ValueProvider.Combine(right.ValueProvider))
        {
        }
    }
}
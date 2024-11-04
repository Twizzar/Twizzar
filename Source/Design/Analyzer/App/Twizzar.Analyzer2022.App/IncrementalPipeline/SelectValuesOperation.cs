using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Analyzer2022.App.IncrementalPipeline
{
    /// <summary>
    /// <see cref="OperationExtensionMethods.Select{TSource,TResult}(IValuesOperation{TSource},System.Func{TSource,System.Threading.CancellationToken,TResult})" />.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="Selector"></param>
    /// <param name="Prev"></param>
    /// <param name="ValuesProvider"></param>
    /// <param name="OperationFactory"></param>
    [ExcludeFromCodeCoverage]
    public record SelectValuesOperation<TSource, TResult>(
        Func<TSource, CancellationToken, TResult> Selector,
        IIncrementalValuesOperation<TSource> Prev,
        IncrementalValuesProvider<TResult> ValuesProvider,
        IOperationFactory OperationFactory)
        : IncrementalValuesOperation<TResult>(ValuesProvider, OperationFactory)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectValuesOperation{TSource,TResult}"/> class.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="prev"></param>
        public SelectValuesOperation(Func<TSource, CancellationToken, TResult> selector, IIncrementalValuesOperation<TSource> prev)
            : this(selector, prev, prev.ValueProvider.Select(selector), prev.OperationFactory)
        {
        }
    }
}
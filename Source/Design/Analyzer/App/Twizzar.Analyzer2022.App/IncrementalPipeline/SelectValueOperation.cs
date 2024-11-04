using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Analyzer2022.App.IncrementalPipeline
{
    /// <summary>
    /// <see cref="OperationExtensionMethods.Select{TSource,TResult}(IValueOperation{TSource},System.Func{TSource,System.Threading.CancellationToken,TResult})" />.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="Selector"></param>
    /// <param name="Prev"></param>
    /// <param name="ValueProvider"></param>
    /// <param name="OperationFactory"></param>
    [ExcludeFromCodeCoverage]
    public record SelectValueOperation<TSource, TResult>(
        Func<TSource, CancellationToken, TResult> Selector,
        IIncrementalValueOperation<TSource> Prev,
        IncrementalValueProvider<TResult> ValueProvider,
        IOperationFactory OperationFactory)
        : IncrementalValueOperation<TResult>(ValueProvider, OperationFactory)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectValueOperation{TSource,TResult}"/> class.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="prev"></param>
        public SelectValueOperation(
            Func<TSource, CancellationToken, TResult> selector,
            IIncrementalValueOperation<TSource> prev)
            : this(selector, prev, prev.ValueProvider.Select(selector), prev.OperationFactory)
        {
        }
    }
}
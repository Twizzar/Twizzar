using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Analyzer2022.App.IncrementalPipeline
{
    /// <summary>
    /// <see cref="OperationExtensionMethods.SelectMany{TSource,TResult}(IValueOperation{TSource},Func{TSource,CancellationToken,ImmutableArray{TResult}})"/>.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="Selector"></param>
    /// <param name="OperationFactory"></param>
    /// <param name="ValuesProvider"></param>
    [ExcludeFromCodeCoverage]
    public record SelectManyOperation<TSource, TResult>(
        Func<TSource, CancellationToken, ImmutableArray<TResult>> Selector,
        IOperationFactory OperationFactory,
        IncrementalValuesProvider<TResult> ValuesProvider)
        : IncrementalValuesOperation<TResult>(ValuesProvider, OperationFactory)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectManyOperation{TSource,TResult}"/> class.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="prev"></param>
        public SelectManyOperation(
            Func<TSource, CancellationToken, ImmutableArray<TResult>> selector,
            IIncrementalValuesOperation<TSource> prev)
            : this(
                selector,
                prev.OperationFactory,
                prev.ValueProvider.SelectMany(selector))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectManyOperation{TSource,TResult}"/> class.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="prev"></param>
        public SelectManyOperation(
            Func<TSource, CancellationToken, ImmutableArray<TResult>> selector,
            IIncrementalValueOperation<TSource> prev)
            : this(
                selector,
                prev.OperationFactory,
                prev.ValueProvider.SelectMany(selector))
        {
        }
    }
}
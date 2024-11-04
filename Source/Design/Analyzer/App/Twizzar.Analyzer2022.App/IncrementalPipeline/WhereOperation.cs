using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Analyzer2022.App.IncrementalPipeline
{
    /// <summary>
    /// <see cref="OperationExtensionMethods.Where{TSource}"/>.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="Predicate"></param>
    /// <param name="Prev"></param>
    /// <param name="ValuesProvider"></param>
    /// <param name="OperationFactory"></param>
    [ExcludeFromCodeCoverage]
    public record WhereOperation<TSource>(
        Func<TSource, bool> Predicate,
        IIncrementalValuesOperation<TSource> Prev,
        IncrementalValuesProvider<TSource> ValuesProvider,
        IOperationFactory OperationFactory)
        : IncrementalValuesOperation<TSource>(ValuesProvider, OperationFactory)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WhereOperation{TSource}"/> class.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="prev"></param>
        public WhereOperation(Func<TSource, bool> predicate, IIncrementalValuesOperation<TSource> prev)
            : this(predicate, prev, prev.ValueProvider.Where(predicate), prev.OperationFactory)
        {
        }
    }
}
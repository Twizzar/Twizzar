using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Analyzer2022.App.IncrementalPipeline
{
    /// <summary>
    /// <see cref="OperationExtensionMethods.Collect{TSource}"/>.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="Source"></param>
    /// <param name="OperationFactory"></param>
    /// <param name="ValueProvider"></param>
    [ExcludeFromCodeCoverage]
    public record CollectOperation<TSource>(
        IIncrementalValuesOperation<TSource> Source,
        IOperationFactory OperationFactory,
        IncrementalValueProvider<ImmutableArray<TSource>> ValueProvider)
        : IncrementalValueOperation<ImmutableArray<TSource>>(ValueProvider, OperationFactory)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectOperation{TSource}"/> class.
        /// </summary>
        /// <param name="source"></param>
        public CollectOperation(IIncrementalValuesOperation<TSource> source)
            : this(source, source.OperationFactory, source.ValueProvider.Collect())
        {
        }
    }
}
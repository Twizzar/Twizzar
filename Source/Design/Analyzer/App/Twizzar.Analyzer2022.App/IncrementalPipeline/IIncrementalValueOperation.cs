using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Analyzer2022.App.IncrementalPipeline
{
    /// <summary>
    /// Operation of a single value used for Incremental Generator.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public interface IIncrementalValueOperation<TSource> : IValueOperation<TSource>
    {
        /// <summary>
        /// Gets the value provider.
        /// </summary>
        IncrementalValueProvider<TSource> ValueProvider { get; }
    }

    /// <inheritdoc cref="IIncrementalValueOperation{TSource}"/>
    [ExcludeFromCodeCoverage]
    public record IncrementalValueOperation<TSource>(
        IncrementalValueProvider<TSource> ValueProvider,
        IOperationFactory OperationFactory) : IIncrementalValueOperation<TSource>
    {
        /// <inheritdoc />
        public IValueOperation<TSource> WithComparer(IEqualityComparer<TSource> comparer) =>
            this with { ValueProvider = this.ValueProvider.WithComparer(comparer) };
    }
}
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Analyzer2022.App.IncrementalPipeline
{
    /// <summary>
    /// Operation of values used for Incremental Generator.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public interface IIncrementalValuesOperation<TSource> : IValuesOperation<TSource>
    {
        /// <summary>
        /// Gets the value provider.
        /// </summary>
        IncrementalValuesProvider<TSource> ValueProvider { get; }
    }

    /// <inheritdoc cref="IIncrementalValuesOperation{TSource}"/>
    [ExcludeFromCodeCoverage]
    public record IncrementalValuesOperation<TSource>(
        IncrementalValuesProvider<TSource> ValueProvider,
        IOperationFactory OperationFactory) : IIncrementalValuesOperation<TSource>
    {
        /// <inheritdoc />
        public IValuesOperation<TSource> WithComparer(IEqualityComparer<TSource> comparer) =>
            this with { ValueProvider = this.ValueProvider.WithComparer(comparer) };
    }
}
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Analyzer2022.App.IncrementalPipeline
{
    /// <summary>
    /// An initial operation for chaining other operations.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="ValueProvider"></param>
    /// <param name="OperationFactory"></param>
    [ExcludeFromCodeCoverage]
    public record InitOperation<TSource>(
            IncrementalValuesProvider<TSource> ValueProvider,
            IOperationFactory OperationFactory)
        : IncrementalValuesOperation<TSource>(ValueProvider, OperationFactory);
}
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations
{
    /// <summary>
    /// Wraps a value in a <see cref="IValuesOperation{TSource}"/>.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="Input"></param>
    /// <param name="OperationFactory"></param>
    /// <param name="CancellationToken"></param>
    [ExcludeFromCodeCoverage]
    public record WrapValuesOperation<TSource>(
            IEnumerable<TSource> Input,
            IOperationFactory OperationFactory,
            CancellationToken CancellationToken)
        : SimpleValuesOperation<TSource>(OperationFactory, CancellationToken)
    {
        /// <inheritdoc />
        public override IEnumerable<TSource> Evaluate() =>
            this.Input;
    }
}
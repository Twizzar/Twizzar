using System.Collections.Immutable;
using System.Threading;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations
{
    /// <summary>
    /// <see cref="OperationExtensionMethods.Collect{TSource}"/>.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="Source"></param>
    /// <param name="OperationFactory"></param>
    /// <param name="CancellationToken"></param>
    public record CollectOperation<TSource>(
        ISimpleValuesOperation<TSource> Source,
        IOperationFactory OperationFactory,
        CancellationToken CancellationToken)
        : SimpleValueOperation<ImmutableArray<TSource>>(OperationFactory, CancellationToken)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectOperation{TSource}"/> class.
        /// </summary>
        /// <param name="source"></param>
        public CollectOperation(ISimpleValuesOperation<TSource> source)
            : this(source, source.OperationFactory, source.CancellationToken)
        {
        }

        /// <inheritdoc />
        public override ImmutableArray<TSource> Evaluate()
        {
            this.CancellationToken.ThrowIfCancellationRequested();

            return this.Source.Evaluate().ToImmutableArray();
        }
    }
}
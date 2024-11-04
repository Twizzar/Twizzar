using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations
{
    /// <summary>
    /// <see cref="OperationExtensionMethods.Where{TSource}"/>.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="Predicate"></param>
    /// <param name="Prev"></param>
    /// <param name="OperationFactory"></param>
    /// <param name="CancellationToken"></param>
    public record WhereOperation<TSource>(
        Func<TSource, bool> Predicate,
        ISimpleValuesOperation<TSource> Prev,
        IOperationFactory OperationFactory,
        CancellationToken CancellationToken)
        : SimpleValuesOperation<TSource>(OperationFactory, CancellationToken)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WhereOperation{TSource}"/> class.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="prev"></param>
        public WhereOperation(Func<TSource, bool> predicate, ISimpleValuesOperation<TSource> prev)
            : this(predicate, prev, prev.OperationFactory, prev.CancellationToken)
        {
        }

        /// <inheritdoc />
        public override IEnumerable<TSource> Evaluate()
        {
            this.CancellationToken.ThrowIfCancellationRequested();

            return this.Prev.Evaluate().Where(this.Predicate);
        }
    }
}
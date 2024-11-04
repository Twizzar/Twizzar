using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations
{
    /// <summary>
    /// <see cref="OperationExtensionMethods.Select{TSource,TResult}(IValuesOperation{TSource},Func{TSource,CancellationToken,TResult})"/>.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="Selector"></param>
    /// <param name="Prev"></param>
    /// <param name="OperationFactory"></param>
    /// <param name="CancellationToken"></param>
    public record SelectValuesOperation<TSource, TResult>(
        Func<TSource, CancellationToken, TResult> Selector,
        ISimpleValuesOperation<TSource> Prev,
        IOperationFactory OperationFactory,
        CancellationToken CancellationToken)
        : SimpleValuesOperation<TResult>(OperationFactory, CancellationToken)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectValuesOperation{TSource,TResult}"/> class.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="prev"></param>
        public SelectValuesOperation(Func<TSource, CancellationToken, TResult> selector, ISimpleValuesOperation<TSource> prev)
            : this(selector, prev, prev.OperationFactory, prev.CancellationToken)
        {
        }

        /// <inheritdoc />
        public override IEnumerable<TResult> Evaluate()
        {
            this.CancellationToken.ThrowIfCancellationRequested();

            return this.Prev.Evaluate().Select(source => this.Selector(source, this.CancellationToken));
        }
    }
}
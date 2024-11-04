using System;
using System.Threading;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations
{
    /// <summary>
    ///
    /// <code>
    ///  IValueOperation&lt;TSource&gt;                 IValueOperation&lt;TResult&gt;
    /// ┌─────────────┐                                 ┌─────────────┐
    /// │             │ Select&lt;TSource,TResult1&gt;  │             │
    /// │   TSource   ├───────────────────────────────► │   TResult   │
    /// │             │                                 │             │
    /// └─────────────┘                                 └─────────────┘
    /// </code>
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="Selector"></param>
    /// <param name="Prev"></param>
    /// <param name="OperationFactory"></param>
    /// <param name="CancellationToken"></param>
    public record SelectValueOperation<TSource, TResult>(
        Func<TSource, CancellationToken, TResult> Selector,
        ISimpleValueOperation<TSource> Prev,
        IOperationFactory OperationFactory,
        CancellationToken CancellationToken)
        : SimpleValueOperation<TResult>(OperationFactory, CancellationToken)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectValueOperation{TSource,TResult}"/> class.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="prev"></param>
        public SelectValueOperation(Func<TSource, CancellationToken, TResult> selector, ISimpleValueOperation<TSource> prev)
            : this(selector, prev, prev.OperationFactory, prev.CancellationToken)
        {
        }

        /// <inheritdoc />
        public override TResult Evaluate()
        {
            this.CancellationToken.ThrowIfCancellationRequested();

            return this.Selector(this.Prev.Evaluate(), this.CancellationToken);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations
{
    /// <summary>
    /// <see cref="OperationExtensionMethods.SelectMany{TSource,TResult}(IValueOperation{TSource},Func{TSource,CancellationToken,ImmutableArray{TResult}})"/>.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="Selector"></param>
    /// <param name="OperationFactory"></param>
    /// <param name="CancellationToken"></param>
    /// <param name="Operation"></param>
    public record SelectManyOperation<TSource, TResult>(
        Func<TSource, CancellationToken, ImmutableArray<TResult>> Selector,
        IOperationFactory OperationFactory,
        Func<IEnumerable<TResult>> Operation,
        CancellationToken CancellationToken)
        : SimpleValuesOperation<TResult>(OperationFactory, CancellationToken)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectManyOperation{TSource,TResult}"/> class.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="prev"></param>
        public SelectManyOperation(
            Func<TSource, CancellationToken, ImmutableArray<TResult>> selector,
            ISimpleValuesOperation<TSource> prev)
            : this(
                selector,
                prev.OperationFactory,
                () => prev.Evaluate().SelectMany(source => selector(source, prev.CancellationToken)),
                prev.CancellationToken)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectManyOperation{TSource,TResult}"/> class.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="prev"></param>
        public SelectManyOperation(
            Func<TSource, CancellationToken, ImmutableArray<TResult>> selector,
            ISimpleValueOperation<TSource> prev)
            : this(
                selector,
                prev.OperationFactory,
                () => selector(prev.Evaluate(), prev.CancellationToken),
                prev.CancellationToken)
        {
        }

        /// <inheritdoc />
        public override IEnumerable<TResult> Evaluate()
        {
            this.CancellationToken.ThrowIfCancellationRequested();

            return this.Operation();
        }
    }
}
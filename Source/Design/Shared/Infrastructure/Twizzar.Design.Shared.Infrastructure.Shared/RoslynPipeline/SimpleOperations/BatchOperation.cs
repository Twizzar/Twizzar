using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations
{
    /// <summary>
    /// Batch together a value with a Sequence.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <param name="Left"></param>
    /// <param name="Right"></param>
    /// <param name="OperationFactory"></param>
    /// <param name="CancellationToken"></param>
    public record BatchOperation<TLeft, TRight>(
        ISimpleValuesOperation<TLeft> Left,
        ISimpleValueOperation<TRight> Right,
        IOperationFactory OperationFactory,
        CancellationToken CancellationToken)
        : SimpleValuesOperation<(TLeft Left, TRight Right)>(OperationFactory, CancellationToken)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BatchOperation{TLeft,TRight}"/> class.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public BatchOperation(ISimpleValuesOperation<TLeft> left, ISimpleValueOperation<TRight> right)
            : this(left, right, left.OperationFactory, left.CancellationToken)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(left, nameof(left))
                .Parameter(right, nameof(right))
                .ThrowWhenNull();

            this.CancellationToken = left.CancellationToken;
        }

        /// <inheritdoc />
        public override IEnumerable<(TLeft Left, TRight Right)> Evaluate()
        {
            this.CancellationToken.ThrowIfCancellationRequested();

            var right = this.Right.Evaluate();
            return this.Left
                .Evaluate()
                .Select(left => (left, right));
        }
    }
}
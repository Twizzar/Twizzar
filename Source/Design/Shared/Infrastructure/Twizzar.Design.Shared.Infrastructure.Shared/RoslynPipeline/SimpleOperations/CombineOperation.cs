using System.Threading;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;

namespace Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations
{
    /// <summary>
    /// <see cref="OperationExtensionMethods.Combine{TLeft,TRight}(IValueOperation{TLeft},IValueOperation{TRight})"/>.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <param name="Left"></param>
    /// <param name="Right"></param>
    /// <param name="OperationFactory"></param>
    /// <param name="CancellationToken"></param>
    public record CombineOperation<TLeft, TRight>(
        ISimpleValueOperation<TLeft> Left,
        ISimpleValueOperation<TRight> Right,
        IOperationFactory OperationFactory,
        CancellationToken CancellationToken)
        : SimpleValueOperation<(TLeft Left, TRight Right)>(OperationFactory, CancellationToken)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CombineOperation{TLeft,TRight}"/> class.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public CombineOperation(ISimpleValueOperation<TLeft> left, ISimpleValueOperation<TRight> right)
            : this(left, right, left.OperationFactory, left.CancellationToken)
        {
        }

        /// <inheritdoc />
        public override (TLeft Left, TRight Right) Evaluate()
        {
            this.CancellationToken.ThrowIfCancellationRequested();

            return (this.Left.Evaluate(), this.Right.Evaluate());
        }
    }
}
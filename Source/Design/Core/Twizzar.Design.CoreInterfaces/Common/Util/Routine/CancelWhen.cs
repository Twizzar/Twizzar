using System;
using System.Diagnostics.CodeAnalysis;

namespace Twizzar.Design.CoreInterfaces.Common.Util.Routine
{
    /// <summary>
    /// Instruction to cancel when the predicate is true.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CancelWhen : ICancelInstruction
    {
        #region fields

        private readonly Predicate<RoutineContext> _predicate;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelWhen"/> class.
        /// </summary>
        /// <param name="predicate"></param>
        public CancelWhen(Predicate<RoutineContext> predicate)
        {
            this._predicate = predicate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelWhen"/> class.
        /// </summary>
        /// <param name="predicate"></param>
        public CancelWhen(Func<bool> predicate)
        {
            this._predicate = _ => predicate();
        }

        #endregion

        #region members

        /// <inheritdoc />
        public virtual bool Evaluate(RoutineContext context) =>
            this._predicate(context);

        #endregion
    }
}
using System;
using System.Diagnostics.CodeAnalysis;

namespace Twizzar.Design.CoreInterfaces.Common.Util.Routine
{
    /// <summary>
    /// Instruction to cancel the execution when in the same runner encounters two <see cref="PreventSpam"/> instruction under a certain TimeSpan.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class PreventSpam : ICancelInstruction
    {
        #region static fields and constants

        private const string LastCall = nameof(LastCall);

        #endregion

        #region fields

        private readonly TimeSpan _delta;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="PreventSpam"/> class.
        /// </summary>
        /// <param name="delta"></param>
        public PreventSpam(TimeSpan delta)
        {
            this._delta = delta;
        }

        #endregion

        #region members

        /// <inheritdoc />
        public bool Evaluate(RoutineContext context)
        {
            var isCanceled = context
                .Get<DateTime>(LastCall)
                .Map(time => DateTime.Now.Subtract(time) < this._delta)
                .SomeOrProvided(false);

            context.Set(LastCall, DateTime.Now);
            return isCanceled;
        }

        #endregion
    }
}
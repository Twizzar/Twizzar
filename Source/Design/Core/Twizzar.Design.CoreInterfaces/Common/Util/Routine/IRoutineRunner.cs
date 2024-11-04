using System.Collections.Generic;

namespace Twizzar.Design.CoreInterfaces.Common.Util.Routine
{
    /// <summary>
    /// Helper service to run some code and cancel execution without the need of nested code and state handling.
    /// </summary>
    public interface IRoutineRunner
    {
        /// <summary>
        /// Run a routine.
        /// </summary>
        /// <param name="instructions">
        /// A Method which has the return type <see cref="IEnumerable{T}"/> where T is <see cref="ICancelInstruction"/>.
        /// Use yield return [<see cref="ICancelInstruction"/> instance] to add some cancel instructions.
        /// </param>
        void Run(IEnumerable<ICancelInstruction> instructions);
    }
}
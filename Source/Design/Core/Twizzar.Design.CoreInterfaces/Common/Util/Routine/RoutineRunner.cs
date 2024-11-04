using System.Collections.Generic;
using System.Diagnostics;

namespace Twizzar.Design.CoreInterfaces.Common.Util.Routine
{
    /// <summary>
    /// Helper class to run some code and cancel execution without the need of nested code and state handling.
    /// </summary>
    public class RoutineRunner : IRoutineRunner
    {
        private readonly RoutineContext _context = new();

        /// <summary>
        /// Run a routine.
        /// </summary>
        /// <param name="instructions">
        /// A Method which has the return type <see cref="IEnumerable{T}"/> where T is <see cref="ICancelInstruction"/>.
        /// Use yield return [<see cref="ICancelInstruction"/> instance] to add some cancel instructions.
        /// </param>
        public void Run(IEnumerable<ICancelInstruction> instructions)
        {
            using var enumerator = instructions.GetEnumerator();
            var isCanceled = false;

            // order matters! First check if canceled if not execute the next code block.
            while (!isCanceled && enumerator.MoveNext())
            {
                Debug.Assert(enumerator.Current != null, "enumerator.Current != null");
                isCanceled = enumerator.Current.Evaluate(this._context);
            }
        }
    }
}
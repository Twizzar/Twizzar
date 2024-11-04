namespace Twizzar.Design.CoreInterfaces.Common.Util.Routine
{
    /// <summary>
    /// Instruction which will be evaluated in the <see cref="RoutineRunner"/> and when true the runner will cancel the execution.
    /// </summary>
    public interface ICancelInstruction
    {
        /// <summary>
        /// Evaluate the instruction.
        /// </summary>
        /// <param name="context">The runner context.</param>
        /// <returns>True when the execution should be canceled.</returns>
        bool Evaluate(RoutineContext context);
    }
}
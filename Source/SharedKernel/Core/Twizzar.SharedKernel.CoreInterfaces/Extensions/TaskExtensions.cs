using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Twizzar.SharedKernel.CoreInterfaces.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Task"/>.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Consumes a task and doesn't do anything with it.  Useful for fire-and-forget calls to async methods within async methods.
        /// </summary>
        /// <param name="task">The task whose result is to be ignored.</param>
        [ExcludeFromCodeCoverage]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "This is intended.")]
        public static void Forget(this Task task)
        {
            // do nothing
        }
    }
}
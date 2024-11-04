using System.Threading.Tasks;
using Twizzar.SharedKernel.CoreInterfaces.Failures;
using ViCommon.Functional;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.CoreInterfaces.Command.FixtureItem.Config
{
    /// <summary>
    /// Writes the given text to a file.
    /// </summary>
    public interface IFileWriter
    {
        /// <summary>
        /// Writes the text to the file.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>A task.</returns>
        Task<IResult<Unit, IoFailure>> WriteAsync(string text, string filePath);
    }
}

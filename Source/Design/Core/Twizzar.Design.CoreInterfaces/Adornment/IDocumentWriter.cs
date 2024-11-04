using System.Threading.Tasks;

namespace Twizzar.Design.CoreInterfaces.Adornment
{
    /// <summary>
    /// Manipulates text in a document.
    /// </summary>
    public interface IDocumentWriter
    {
        #region members

        /// <summary>
        /// Makes the class partial that contains the given span.
        /// </summary>
        /// <param name="adornmentInformation">The adornment information.</param>
        /// <param name="configLabel">The config class name.</param>
        /// <returns>Task to wait for completion.</returns>
        Task PrepareClassAsync(IAdornmentInformation adornmentInformation, string configLabel);

        /// <summary>
        /// Makes the class partial which contains the given span.
        /// </summary>
        /// <param name="adornmentInformation">The adornment information.</param>
        /// <param name="configName">The of the configuration.</param>
        /// <returns>Task to wait for completion.</returns>
        Task PrepareCodeBehindAsync(IAdornmentInformation adornmentInformation, string configName);

        #endregion
    }
}
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.CoreInterfaces.Common.Util
{
    /// <summary>
    /// Helper class for getting the versions of the AddIn.
    /// </summary>
    public interface IAddinVersionQuery : IQuery
    {
        /// <summary>
        /// Gets the vs adding version.
        /// </summary>
        /// <returns>The addin version.</returns>
        string GetVsAddinVersion();

        /// <summary>
        /// Gets the DLL version.
        /// </summary>
        /// <returns>The dll version.</returns>
        string GetDllVersion();

        /// <summary>
        /// Gets the product version.
        /// </summary>
        /// <returns>The product version.</returns>
        string GetProductVersion();
    }
}

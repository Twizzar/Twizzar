using System.Threading.Tasks;

namespace Twizzar.Design.CoreInterfaces.Common.Util
{
    /// <summary>
    /// Query for visual studio command information.
    /// </summary>
    public interface IVsCommandQuery
    {
        /// <summary>
        /// Gets the shortcut string of the open/close command.
        /// </summary>
        /// <returns>The shortcut of the open/close command as a string.</returns>
        Task<string> GetShortCutOfOpenCloseCommandAsync();
    }
}

namespace Twizzar.Design.CoreInterfaces.Common.VisualStudio
{
    /// <summary>
    /// Internal representation of a Visual Studio project.
    /// </summary>
    public interface IViProject
    {
        #region properties

        /// <summary>
        /// Gets the name of the object.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the project root directory.
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Gets the directory where the project files lies.
        /// </summary>
        string ProjectDirectory { get; }

        #endregion
    }
}
namespace Twizzar.Design.Ui.Interfaces.ViewModels
{
    /// <summary>
    /// The view model form the about window.
    /// </summary>
    public interface IAboutViewModel
    {
        /// <summary>
        /// Gets the addin version.
        /// </summary>
        string AddinVersion { get; }

        /// <summary>
        /// Gets the DLL version.
        /// </summary>
        string DllVersion { get; }

        /// <summary>
        /// Gets the product version.
        /// </summary>
        string ProductVersion { get; }
    }
}
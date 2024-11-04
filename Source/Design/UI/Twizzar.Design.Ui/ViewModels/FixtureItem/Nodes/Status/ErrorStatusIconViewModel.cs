using Twizzar.Design.Ui.Interfaces.Services;

namespace Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.Status
{
    /// <summary>
    /// Status icon for an error.
    /// </summary>
    public class ErrorStatusIconViewModel : SimpleStatusIconViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorStatusIconViewModel"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="imageProvider"></param>
        public ErrorStatusIconViewModel(string message, IImageProvider imageProvider)
            : base(message, "StatusCriticalError/StatusCriticalError_16x.png", imageProvider)
        {
        }
    }
}
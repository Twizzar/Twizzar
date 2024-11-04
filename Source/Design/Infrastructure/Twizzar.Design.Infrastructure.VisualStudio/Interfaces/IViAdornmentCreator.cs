using Microsoft.VisualStudio.Text.Editor;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Ui.Interfaces.VisualStudio;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.Infrastructure.VisualStudio.Interfaces
{
    /// <summary>
    /// The adornment creator for creating new <see cref="IViAdornment"/>.
    /// </summary>
    public interface IViAdornmentCreator : IService
    {
        #region members

        /// <summary>
        /// Create a new <see cref="IViAdornment"/>.
        /// </summary>
        /// <param name="adornmentInformation"></param>
        /// <param name="textView">The text view.</param>
        /// <param name="snapshotHistory">The snapshot history.</param>
        /// <param name="documentAdornmentController"></param>
        /// <returns>A new instance of <see cref="IViAdornment"/>.</returns>
        IViAdornment Create(
            IAdornmentInformation adornmentInformation,
            IWpfTextView textView,
            ISnapshotHistory snapshotHistory,
            IDocumentAdornmentController documentAdornmentController);

        #endregion
    }
}
using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Text;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.Enums;
using Twizzar.Design.Ui.Interfaces.VisualStudio;

namespace Twizzar.Design.Infrastructure.VisualStudio.Interfaces
{
    /// <summary>
    /// Service which knows how to open the peek view and how to provide the <see cref="IFixtureItemPeekResultContent"/>.
    /// The creation and closure of the peek view session is handled by the service.
    /// This service ensures that the peek session is close first before a new one is created to prevent some unintended scrolling in
    /// the text view.
    /// </summary>
    public interface IDocumentAdornmentController : IDisposable
    {
        /// <summary>
        /// Open the adornment.
        /// </summary>
        /// <param name="viAdornment">The adornment to open.</param>
        /// <param name="snapshotSpan">The snapshot span under which the peek view will be placed.</param>
        /// <returns>A task.</returns>
        Task OpenAdornmentAsync(IViAdornment viAdornment, SnapshotSpan snapshotSpan);

        /// <summary>
        /// Close the adornment.
        /// </summary>
        /// <param name="viAdornment">The adornment.</param>
        /// <returns>A task.</returns>
        Task CloseAdornmentAsync(IViAdornment viAdornment);

        /// <summary>
        /// Update the adornment information async.
        /// </summary>
        /// <param name="adornmentInformation"></param>
        /// <returns>A task.</returns>
        Task UpdateInformationAsync(IAdornmentInformation adornmentInformation);

        /// <summary>
        /// Focus the fixture panel. And move the focus in a desired direction.
        /// </summary>
        /// <param name="direction">The direction the focus should be moved.</param>
        void FocusFixturePanel(ViEnterFocusPosition direction);
    }
}
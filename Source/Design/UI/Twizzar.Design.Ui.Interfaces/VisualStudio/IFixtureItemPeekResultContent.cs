using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Infrastructure.VisualStudio.Enums;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status;

namespace Twizzar.Design.Ui.Interfaces.VisualStudio
{
    /// <summary>
    /// The fixture item panel shows the ui for showing and editing the FixtureItem configuration.
    /// </summary>
    public interface IFixtureItemPeekResultContent : IDisposable
    {
        #region properties

        /// <summary>
        /// Gets the Height of the FixtureItem UserControl.
        /// </summary>
        double ControlHeight { get; }

        /// <summary>
        /// Gets the ui element of the peek result content, which is a scroll viewer on root level.
        /// </summary>
        UIElement ScrollViewer { get; }

        /// <summary>
        /// Gets the content of the peek result, which is a user control.
        /// </summary>
        UIElement FixtureUserControl { get; }

        /// <summary>
        /// Gets or sets the zoom level.
        /// </summary>
        double Zoom { get; set; }

        #endregion

        #region members

        /// <summary>
        /// Initializes UI with viewmodel and other state.
        /// </summary>
        /// <param name="adornmentId"></param>
        /// <param name="adornmentInformation"></param>
        /// <param name="documentWriter"></param>
        /// <param name="statusPanelViewModel"></param>
        /// <param name="compilationTypeQuery"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task InitializeAsync(
            AdornmentId adornmentId,
            IAdornmentInformation adornmentInformation,
            IDocumentWriter documentWriter,
            IStatusPanelViewModel statusPanelViewModel,
            ICompilationTypeQuery compilationTypeQuery,
            CancellationToken cancellationToken);

        /// <summary>
        /// Update the adornment information.
        /// </summary>
        /// <param name="adornmentInformation"></param>
        /// <returns>A Task.</returns>
        Task UpdateAsync(IAdornmentInformation adornmentInformation);

        /// <summary>
        /// Move the focus away form this control in a specific direction.
        /// </summary>
        /// <param name="direction">The direction to move.</param>
        void MoveFocus(ViEnterFocusPosition direction);

        #endregion
    }
}
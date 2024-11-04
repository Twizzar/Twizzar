using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.Ui.Interfaces.VisualStudio
{
    /// <summary>
    /// The Factory to create the components needed by the UI.
    /// </summary>
    public interface IUiElementsFactory : IFactory
    {
        /// <summary>
        /// Create the adornment expander.
        /// </summary>
        /// <param name="adornmentId">The adornment id.</param>
        /// <param name="statusPanelViewModel">The status panel view model.</param>
        /// <returns>A new instance of <see cref="IAdornmentExpander"/>.</returns>
        IAdornmentExpander CreateAdornmentExpander(
            AdornmentId adornmentId,
            IStatusPanelViewModel statusPanelViewModel);
    }
}
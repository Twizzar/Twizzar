using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.Ui.Interfaces.Factories
{
    /// <summary>
    /// Factory for creating the status panel and its hosted icons.
    /// </summary>
    public interface IFixtureItemNodeStatusFactory : IFactory
    {
        /// <summary>
        /// Creates a new <see cref="IStatusPanelViewModel"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="IStatusPanelViewModel"/>.</returns>
        IStatusPanelViewModel CreateStatusPanelViewModel();

        /// <summary>
        /// Creates an error status icon.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <returns>A new instance of <see cref="IStatusIconViewModel"/>.</returns>
        IStatusIconViewModel CreateErrorStatusIconViewModel(string message);

        /// <summary>
        /// Creates a no configurable member status icon.
        /// </summary>
        /// <param name="panelViewModel">The panel hosting this item.</param>
        /// <returns>A new instance of <see cref="IStatusIconViewModel"/>.</returns>
        IStatusIconViewModel CreateNoConfigurableMemberStatusIconViewModel(
            IStatusPanelViewModel panelViewModel);

        /// <summary>
        /// Creates an array not configurable status icon.
        /// </summary>
        /// <param name="panelViewModel"></param>
        /// <returns>A new instance of <see cref="IStatusIconViewModel"/>.</returns>
        IStatusIconViewModel CreateArrayNotConfigurableStatusIconViewModel(IStatusPanelViewModel panelViewModel);

        /// <summary>
        /// Creates an base type are always unique status icon.
        /// </summary>
        /// <param name="panelViewModel"></param>
        /// <returns>A new instance of <see cref="IStatusIconViewModel"/>.</returns>
        IStatusIconViewModel CreateBaseTypeIsAlwaysUniqueStatusIconViewModel(IStatusPanelViewModel panelViewModel);
    }
}
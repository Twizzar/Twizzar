using System.Collections.ObjectModel;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status;

namespace Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.Status
{
    /// <summary>
    /// Status panel for the status icons.
    /// </summary>
    public class StatusPanelViewModel : IStatusPanelViewModel
    {
        #region properties

        /// <inheritdoc />
        public ObservableCollection<IStatusIconViewModel> Icons { get; } =
            new ObservableCollection<IStatusIconViewModel>();

        #endregion

        #region members

        /// <inheritdoc />
        public void Add(IStatusIconViewModel icon)
        {
            this.Icons.Add(icon);
        }

        /// <inheritdoc />
        public void Remove(IStatusIconViewModel icon)
        {
            this.Icons.Remove(icon);
        }

        #endregion
    }
}
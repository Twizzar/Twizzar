using System.Collections.ObjectModel;

namespace Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status
{
    /// <summary>
    /// ViewModel for a panel showing status icons.
    /// </summary>
    public interface IStatusPanelViewModel
    {
        #region properties

        /// <summary>
        /// Gets all icons.
        /// </summary>
        ObservableCollection<IStatusIconViewModel> Icons { get; }

        #endregion

        #region members

        /// <summary>
        /// Add an icon.
        /// </summary>
        /// <param name="icon"></param>
        void Add(IStatusIconViewModel icon);

        /// <summary>
        /// Remove an icon.
        /// </summary>
        /// <param name="icon"></param>
        void Remove(IStatusIconViewModel icon);

        #endregion
    }
}
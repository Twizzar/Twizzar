using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status;

namespace Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes
{
    /// <summary>
    /// Extends the <see cref="IFixtureItemNode"/> with the view model elements.
    /// </summary>
    public interface IFixtureItemNodeViewModel : IDisposable
    {
        #region properties

        /// <summary>
        /// Gets the fixture information.
        /// </summary>
        IFixtureItemInformation FixtureItemInformation { get; }

        /// <summary>
        /// Gets the Command executed when click on the expander button.
        /// This is used to Create the child view models.
        /// </summary>
        ICommand ExpandChildMemberDefinition { get; }

        /// <summary>
        /// Gets the status panel view model.
        /// </summary>
        IStatusPanelViewModel StatusPanelViewModel { get; }

        /// <summary>
        /// Gets the value. It could be either the real value of a link to a value definition.
        /// </summary>
        abstract IFixtureItemNodeValueViewModel Value { get; }

        /// <summary>
        /// Gets the children of this node.
        /// </summary>
        ObservableCollection<IFixtureItemNodeViewModel> Children { get; }

        /// <summary>
        /// Gets the ui event hub.
        /// </summary>
        IUiEventHub UiEventHub { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the item is expanded.
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        /// Gets a value indicating whether the view model is disposed.
        /// </summary>
        bool IsDisposed { get; }

        #endregion
    }
}
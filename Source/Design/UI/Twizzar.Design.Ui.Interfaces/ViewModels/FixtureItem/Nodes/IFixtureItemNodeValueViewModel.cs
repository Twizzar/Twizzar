using System.Collections.Generic;
using System.Windows.Input;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.Ui.Interfaces.ValueObjects;

namespace Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes
{
    /// <summary>
    /// The view model of the value of a fixture item.
    /// </summary>
    public interface IFixtureItemNodeValueViewModel
    {
        #region properties

        /// <summary>
        /// Gets or sets the total Text of the value.
        /// This will be set from the ui.
        /// </summary>
        string FullText { get; set; }

        /// <summary>
        /// Gets the default value of the node value item.
        /// </summary>
        string DefaultValue { get; }

        /// <summary>
        /// Gets the tooltip of the node value item.
        /// </summary>
        string Tooltip { get; }

        /// <summary>
        /// Gets the adorner text.
        /// </summary>
        string AdornerText { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the value controls has focus.
        /// </summary>
        bool HasFocus { get; set; }

        /// <summary>
        /// Gets all segments of the value of the fixture item.
        /// </summary>
        IEnumerable<ItemValueSegment> ItemValueSegments { get; }

        /// <summary>
        /// Gets all possible entries for the auto complete list.
        /// </summary>
        IEnumerable<AutoCompleteEntry> AutoCompleteEntries { get; }

        /// <summary>
        /// Gets a value indicating whether the value is read only or not.
        /// The value of the selected constructor is now for example read only.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Gets the commit command to apply the value changes to the domain.
        /// </summary>
        ICommand Commit { get; }

        /// <summary>
        /// Gets command to expand or collapse current node.
        /// </summary>
        ICommand ExpandCollapseCommand { get; }

        /// <summary>
        /// Gets or sets parent view model.
        /// </summary>
        IFixtureItemNodeViewModel FixtureNodeVM { get; set; }

        /// <summary>
        /// Gets the expand and collapse shortcut.
        /// </summary>
        string ExpandAndCollapseShortcut { get; }

        /// <summary>
        /// Gets the node id.
        /// </summary>
        NodeId Id { get; }

        /// <summary>
        /// Gets the service provider.
        /// </summary>
        IScopedServiceProvider ServiceProvider { get; }

        #endregion
    }
}
using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.Ui.Interfaces.Services;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status;

namespace Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.Status
{
    /// <summary>
    /// Status icon for a node with no configurable members.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ArrayNotConfigurableStatusIconViewModel : SingletonStatusIconViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayNotConfigurableStatusIconViewModel"/> class.
        /// </summary>
        /// <param name="panel">The panel of this icon.</param>
        /// <param name="imageProvider"></param>
        public ArrayNotConfigurableStatusIconViewModel(
            IStatusPanelViewModel panel,
            IImageProvider imageProvider)
            : base(panel, "Array not configurable", "StatusInformation/StatusInformation_16x.png", imageProvider)
        {
        }
    }
}
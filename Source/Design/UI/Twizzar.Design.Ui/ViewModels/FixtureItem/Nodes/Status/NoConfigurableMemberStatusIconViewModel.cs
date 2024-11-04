using Twizzar.Design.Ui.Interfaces.Services;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status;

namespace Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.Status
{
    /// <summary>
    /// Status icon for a node with no configurable members.
    /// </summary>
    public class NoConfigurableMemberStatusIconViewModel : SingletonStatusIconViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoConfigurableMemberStatusIconViewModel"/> class.
        /// </summary>
        /// <param name="panel">The panel of this icon.</param>
        /// <param name="imageProvider"></param>
        public NoConfigurableMemberStatusIconViewModel(
            IStatusPanelViewModel panel,
            IImageProvider imageProvider)
            : base(panel, "No configurable members", "StatusInformation/StatusInformation_16x.png", imageProvider)
        {
        }
    }
}
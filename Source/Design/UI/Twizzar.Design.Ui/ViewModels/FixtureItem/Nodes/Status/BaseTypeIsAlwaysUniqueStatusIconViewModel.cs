using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Resources;
using Twizzar.Design.Ui.Interfaces.Services;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status;

namespace Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.Status
{
    /// <summary>
    /// Status icon view model for the status when an adornment of a basetype fixtureItem is expanded.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class BaseTypeIsAlwaysUniqueStatusIconViewModel : SingletonStatusIconViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTypeIsAlwaysUniqueStatusIconViewModel"/> class.
        /// </summary>
        /// <param name="panel">The panel of this icon.</param>
        /// <param name="imageProvider"></param>
        public BaseTypeIsAlwaysUniqueStatusIconViewModel(
            IStatusPanelViewModel panel,
            IImageProvider imageProvider)
            : base(panel, MessagesDesign.Basetype_is_always_unique, "StatusInformation/StatusInformation_16x.png", imageProvider)
        {
        }
    }
}
using System.Linq;
using Twizzar.Design.Ui.Interfaces.Services;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.Status
{
    /// <summary>
    /// Status icon which only exists once per panel. When a new one will be added the old status icon will be removed.
    /// </summary>
    public abstract class SingletonStatusIconViewModel : SimpleStatusIconViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonStatusIconViewModel"/> class.
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="message"></param>
        /// <param name="imgPath"></param>
        /// <param name="imageProvider"></param>
        protected SingletonStatusIconViewModel(
            IStatusPanelViewModel panel,
            string message,
            string imgPath,
            IImageProvider imageProvider)
            : base(message, imgPath, imageProvider)
        {
            this.EnsureMany()
                .Parameter(panel, nameof(panel))
                .Parameter(imageProvider, nameof(imageProvider))
                .ThrowWhenNull();

            this.EnsureMany<string>()
                .Parameter(message, nameof(message))
                .Parameter(imgPath, nameof(imgPath))
                .IsNotNullAndNotEmpty()
                .ThrowOnFailure();

            var icon = panel.Icons?.FirstOrDefault(model => model.GetType() == this.GetType());

            if (icon != null)
            {
                panel.Remove(icon);
            }
        }
    }
}
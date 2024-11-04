using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.CommandWpf;
using Twizzar.Design.Ui.Interfaces.Services;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.Status
{
    /// <inheritdoc cref="IStatusIconViewModel" />
    public abstract class SimpleStatusIconViewModel : IStatusIconViewModel,
        IHasEnsureHelper,
        IHasLogger
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleStatusIconViewModel"/> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="imgPath"></param>
        /// <param name="imageProvider"></param>
        protected SimpleStatusIconViewModel(string message, string imgPath, IImageProvider imageProvider)
        {
            this.EnsureMany<string>()
                .Parameter(message, nameof(message))
                .Parameter(imgPath, nameof(imgPath))
                .IsNotNullAndNotEmpty()
                .ThrowOnFailure();

            this.EnsureParameter(imageProvider, nameof(imageProvider)).ThrowWhenNull();

            this.ToolTip = message;
            this.Image = imageProvider.GetBitmapSource(imgPath);
            this.ClickCommand = new RelayCommand(this.OnClick);
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public virtual ICommand ClickCommand { get; }

        /// <inheritdoc />
        public string ToolTip { get; }

        /// <inheritdoc />
        public BitmapSource Image { get; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region members

        /// <summary>
        /// Called when the icon is clicked.
        /// </summary>
        protected virtual void OnClick()
        {
            // intention left empty.
        }

        #endregion
    }
}
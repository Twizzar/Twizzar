using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.PlatformUI;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.Ui.Interfaces.Messaging.VsEvents;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Infrastructure.VisualStudio.Services
{
    /// <summary>
    /// Vs color theme event wrapper class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class VsColorThemeEventWrapper : IVsColorThemeEventWrapper
    {
        private readonly IUiEventHub _uiEventHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="VsColorThemeEventWrapper"/> class.
        /// </summary>
        /// <param name="uiEventHub"></param>
        public VsColorThemeEventWrapper(IUiEventHub uiEventHub)
        {
            this.EnsureMany()
                .Parameter(uiEventHub, nameof(uiEventHub))
                .IsNotNull();

            this._uiEventHub = uiEventHub;
        }

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        /// <inheritdoc />
        public void Initialize()
        {
            VSColorTheme.ThemeChanged += this.OnThemeChanged;
        }

        private void OnThemeChanged(ThemeChangedEventArgs e)
        {
            this._uiEventHub.Publish(new VsThemeChangedEvent());
        }
    }
}
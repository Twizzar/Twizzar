using System;
using System.Threading.Tasks;

using NLog;
using NLog.Targets;

using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

using ILogger = Twizzar.SharedKernel.NLog.Interfaces.ILogger;
using LogLevel = NLog.LogLevel;

namespace Twizzar.Design.Infrastructure.VisualStudio.Common.Util
{
    /// <summary>
    /// Nlog target for user notifications.
    /// </summary>
    public class ViNotificationTarget : TargetWithLayout, IHasEnsureHelper, IHasLogger
    {
        #region fields

#if DEBUG
        private static readonly LogLevel MinOutputLogLevel = LogLevel.Trace;
#else
        private static readonly LogLevel MinOutputLogLevel = LogLevel.Info;
#endif

        private readonly IViNotificationService _notificationService;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViNotificationTarget"/> class.
        /// </summary>
        /// <param name="notificationService"></param>
        public ViNotificationTarget(IViNotificationService notificationService)
        {
            this.EnsureParameter(notificationService, nameof(notificationService)).ThrowWhenNull();

            this._notificationService = notificationService;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override void Write(LogEventInfo logEvent)
        {
            base.Write(logEvent);
            this.WriteAsync(logEvent).Forget();
        }

        private async Task WriteAsync(LogEventInfo logEvent)
        {
            try
            {
                if (logEvent.Level >= LogLevel.Fatal)
                {
                    var msg = logEvent.Exception?.Message ?? logEvent.FormattedMessage;
                    await this._notificationService.SendToInfoBarAsync(msg);
                }

                if (logEvent.Level >= MinOutputLogLevel)
                {
                    await this._notificationService.SendToOutputAsync(this.Layout.Render(logEvent));
                }
            }
            catch (Exception exception)
            {
                this.Log(exception);
            }
        }

        #endregion
    }
}
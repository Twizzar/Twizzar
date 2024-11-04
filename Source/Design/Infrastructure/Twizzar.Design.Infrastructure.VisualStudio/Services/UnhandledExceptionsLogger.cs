using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;
using ViCommon.EnsureHelper;

namespace Twizzar.Design.Infrastructure.VisualStudio.Services
{
    /// <inheritdoc cref="IUnhandledExceptionsLogger"/>
    [ExcludeFromCodeCoverage]
    public class UnhandledExceptionsLogger : IUnhandledExceptionsLogger
    {
        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public void Initialize()
        {
            Application.Current.DispatcherUnhandledException += (sender, args) =>
            {
                if (args.Exception.Source.Contains("Twizzar"))
                {
                    args.Handled = true;
                    this.Log(args.Exception, LogLevel.Fatal);
                }
            };
        }

        #endregion
    }
}
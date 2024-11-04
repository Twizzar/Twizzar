using System.Collections.Generic;

using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.CoreInterfaces.Common.Roslyn;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.Design.Infrastructure.VisualStudio
{
    /// <inheritdoc />
    public class AddinEntryPoint : IAddinEntryPoint
    {
        #region fields

        private readonly IEnumerable<ISolutionEventsPublisher> _solutionEventsPublishers;
        private readonly IVsColorThemeEventWrapper _vsColorThemeEventWrapper;
        private readonly IUnhandledExceptionsLogger _unhandledExceptionsLogger;
        private readonly ICompilationTypeCache _compilationTypeCache;
        private readonly IVersionChecker _versionChecker;
        private readonly ISettingsQuery _settingsQuery;
        private readonly ICommandBus _commandBus;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddinEntryPoint"/> class.
        /// </summary>
        /// <param name="solutionEventsPublishers">The solution events publisher.</param>
        /// <param name="vsColorThemeEventWrapper">The vs color theme event wrapper.</param>
        /// <param name="unhandledExceptionsLogger">The unhandled exception logger.</param>
        /// <param name="commandBus">The command-bus.</param>
        /// <param name="settingsQuery"></param>
        /// <param name="versionChecker"></param>
        /// <param name="compilationTypeCache"></param>
        public AddinEntryPoint(
            IEnumerable<ISolutionEventsPublisher> solutionEventsPublishers,
            IVsColorThemeEventWrapper vsColorThemeEventWrapper,
            IUnhandledExceptionsLogger unhandledExceptionsLogger,
            ICommandBus commandBus,
            ISettingsQuery settingsQuery,
            IVersionChecker versionChecker,
            ICompilationTypeCache compilationTypeCache)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(solutionEventsPublishers, nameof(solutionEventsPublishers))
                .Parameter(vsColorThemeEventWrapper, nameof(vsColorThemeEventWrapper))
                .Parameter(unhandledExceptionsLogger, nameof(unhandledExceptionsLogger))
                .Parameter(commandBus, nameof(commandBus))
                .Parameter(settingsQuery, nameof(settingsQuery))
                .Parameter(versionChecker, nameof(versionChecker))
                .Parameter(compilationTypeCache, nameof(compilationTypeCache))
                .ThrowWhenNull();

            this._solutionEventsPublishers = solutionEventsPublishers;
            this._vsColorThemeEventWrapper = vsColorThemeEventWrapper;
            this._unhandledExceptionsLogger = unhandledExceptionsLogger;
            this._commandBus = commandBus;
            this._settingsQuery = settingsQuery;
            this._versionChecker = versionChecker;
            this._compilationTypeCache = compilationTypeCache;
        }

        #endregion

        #region members

        /// <inheritdoc />
        public void Start()
        {
            // this need to be initialized before the solutionEventsPublishers.
            this._versionChecker.Initialize();

            this.InitTheEventSystem();

            // register vs theme changed event
            this._vsColorThemeEventWrapper.Initialize();

            this._settingsQuery.Initialize();
            this._compilationTypeCache.Initialize();
        }

        private void InitTheEventSystem()
        {
            // Initialize the unhandled exception logger
            this._unhandledExceptionsLogger.Initialize();

            // Initialize the event publisher
            this._solutionEventsPublishers.ForEach(publisher => publisher.Initialize());
        }

        #endregion
    }
}
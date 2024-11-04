using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Autofac;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Text.Tagging;

using NLog.Targets;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.Infrastructure.VisualStudio.Common.Util;
using Twizzar.Design.Infrastructure.VisualStudio.Services.Notification;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.Infrastructure.IocInitializer;
using Twizzar.VsAddin.CompositionRoot.Registrant;
using Twizzar.VsAddin.Interfaces.CompositionRoot;

namespace Twizzar.VsAddin.CompositionRoot
{
    /// <summary>
    /// Does everything containing IoC.
    /// Combines MEF composition an AutoFac composition.
    /// </summary>
    [Export(typeof(IIocOrchestrator))]
    public class IocOrchestrator : IIocOrchestrator
    {
        #region MEF injection

#pragma warning disable // the fields are injected by MEF.

        [Import]
        private IBufferTagAggregatorFactoryService _bufferTagAggregatorFactoryService;

        [Import]
        private readonly IWpfKeyboardTrackingService _trackingService;

        [Import]
        private readonly VisualStudioWorkspace _visualStudioWorkspace;

#pragma warning restore

        #endregion

        #region fields

        private bool _isInitialized = false;

        #endregion

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="IocOrchestrator"/> class.
        /// </summary>
        protected IocOrchestrator()
        {
            CreationCount++;
            if (CreationCount > 1)
            {
                throw new InvalidOperationException("The IoC Orchestrator should only gets created once by MEF.");
            }
        }

        #endregion

        #region public static properties

        /// <summary>
        /// Gets the IoC container of the Add-in.
        /// This container lives next to MEF.
        /// </summary>
        public IContainer Container { get; private set; }

        /// <summary>
        /// Gets or sets how many instances of the <see cref="IocOrchestrator"/> where created.
        /// </summary>
        protected static int CreationCount { get; set; } = 0;

        #endregion

        /// <summary>
        /// Gets a list of all <see cref="IIocComponentRegistrant"/> on register this components will be called to register components.
        /// </summary>
        protected virtual List<IIocComponentRegistrant> Registrants => new()
        {
            new CommandHandlerRegistrant(),
            new ConfigurationRegistrant(),
            new EventSourcingRegistrant(),
            new FixtureItemDefinitionRegistrant(),
            new TypeDescriptionRegistrant(),
            new ReadModelRegistrant(),
            new MefComponentsRegistrant(this._bufferTagAggregatorFactoryService, this._trackingService, this._visualStudioWorkspace),
            new TaggerRegistrant(),
            new UiElementsRegistrant(),
            new UtilRegistrant(),
            new TestCreationRegistrant(),
        };

        #region public methods

        /// <summary>
        /// Resolves a Interface of T to generate a member of the registered base type.
        /// </summary>
        /// <typeparam name="T">The type of the instance that should be generated.</typeparam>
        /// <returns>A instance of the resolved type T.</returns>
        public T Resolve<T>()
        {
            this.Initialize();

            if (this.Container == null)
            {
                throw new InternalException("Could not resolve dependency");
            }

            return this.Container.Resolve<T>();
        }

        #endregion

        #region private static methods

        /// <summary>
        /// Registers the components.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private void RegisterComponents(ContainerBuilder builder)
        {
            foreach (var registrant in this.Registrants)
            {
                registrant.RegisterComponents(builder);
            }
        }

        #endregion

        #region pivate methods

        /// <summary>
        /// Initializes this instance if necessary.
        /// </summary>
        private void Initialize()
        {
            if (!this._isInitialized)
            {
                this.Register();

                this._isInitialized = true;
            }
        }

        /// <summary>
        /// Registers everything.
        /// </summary>
        private void Register()
        {
            if (this.Container != null)
            {
                return;
            }

            var builder = new ContainerBuilder();
            builder.RegisterInstance(this)
            .As<IIocOrchestrator>()
                .SingleInstance();

            var notificationService = new VsNotificationService();
            builder.RegisterInstance(notificationService)
                .As<IViNotificationService>()
                .SingleInstance();

            var loggerTargets = new List<Target>
            {
                new ViNotificationTarget(notificationService),
            };

#if !NCrunch && !DEBUG

            try
            {
                var settingsQuery = new SettingsFileService(new FileService());
                var monitorInstance = new AppInsightsMonitorInstance("Addin", settingsQuery.GetAnalyticsEnabled());
                ViMonitor.SetInstance(monitorInstance);
                loggerTargets.Add(new ViApplicationInsightsTarget(monitorInstance.Client));

                builder.RegisterInstance(monitorInstance)
                    .As<IEventListener<AnalyticsEnabledOrDisabledEvent>>()
                    .SingleInstance();
            }
            catch (Exception)
            {
                // ingored
            }
#endif

            LoggerInitializer.Init(builder, @"\vi-sit\twizzar\twizzarAddin.log", "Addin", loggerTargets.ToArray());
            EnsureHelperInitializer.Init(builder);
            this.RegisterComponents(builder);

            this.Container = builder.Build();
        }

        #endregion
    }
}

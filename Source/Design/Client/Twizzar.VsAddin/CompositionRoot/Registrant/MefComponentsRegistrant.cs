using System.Diagnostics.CodeAnalysis;
using Autofac;
using Community.VisualStudio.Toolkit;
using EnvDTE;
using EnvDTE80;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Tagging;
using Twizzar.VsAddin.Interfaces.CompositionRoot;

namespace Twizzar.VsAddin.CompositionRoot.Registrant
{
    /// <summary>
    /// Registers components provided by MEF to AutoFac.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MefComponentsRegistrant : IIocComponentRegistrant
    {
        #region fields

        private readonly IBufferTagAggregatorFactoryService _bufferTagAggregatorFactoryService;
        private readonly IWpfKeyboardTrackingService _trackingService;
        private readonly Workspace _workspace;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="MefComponentsRegistrant"/> class.
        /// </summary>
        /// <param name="bufferTagAggregatorFactoryService">The tag aggregator factory.</param>
        /// <param name="trackingService">The wpf keyboard tracking service.</param>
        /// <param name="workspace">The roslyn workspace.</param>
        public MefComponentsRegistrant(
            IBufferTagAggregatorFactoryService bufferTagAggregatorFactoryService,
            IWpfKeyboardTrackingService trackingService,
            Workspace workspace)
        {
            this._bufferTagAggregatorFactoryService = bufferTagAggregatorFactoryService;
            this._trackingService = trackingService;
            this._workspace = workspace;
        }

        #endregion

        #region members

        /// <inheritdoc />
        public void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterInstance(this._bufferTagAggregatorFactoryService)
                .As<IBufferTagAggregatorFactoryService>()
                .SingleInstance();

            builder.RegisterInstance(this._trackingService)
                .As<IWpfKeyboardTrackingService>()
                .SingleInstance();

            builder.RegisterInstance(this._workspace)
                .As<Workspace>()
                .SingleInstance();

            builder.Register(_ => VS.GetRequiredService<SDTE, DTE>())
                .As<DTE>()
                .SingleInstance();

            builder.Register(_ => VS.GetRequiredService<SDTE, DTE2>())
                .As<DTE2>()
                .SingleInstance();
        }

        #endregion
    }
}
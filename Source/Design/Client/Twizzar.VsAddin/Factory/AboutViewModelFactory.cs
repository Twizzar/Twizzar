using System.Diagnostics.CodeAnalysis;

using Twizzar.Design.Ui.Interfaces.Factories;
using Twizzar.Design.Ui.Interfaces.ViewModels;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.VsAddin.Interfaces.CompositionRoot;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.VsAddin.Factory
{
    /// <inheritdoc cref="IAboutViewModel"/>
    [ExcludeFromCodeCoverage]
    public class AboutViewModelFactory : IAboutViewModelFactory
    {
        private readonly IIocOrchestrator _iocOrchestrator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutViewModelFactory"/> class.
        /// </summary>
        /// <param name="iocOrchestrator">The ioc orchestrator1.</param>
        public AboutViewModelFactory(IIocOrchestrator iocOrchestrator)
        {
            this._iocOrchestrator = this.EnsureCtorParameterIsNotNull(iocOrchestrator, nameof(iocOrchestrator));
        }

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region Implementation of IAboutViewModelFactory

        /// <inheritdoc />
        public IAboutViewModel CreateAboutViewModel() =>
            this._iocOrchestrator.Resolve<IAboutViewModel>();

        #endregion
    }
}

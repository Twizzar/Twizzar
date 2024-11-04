using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.Ui.Interfaces.Factories;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.Status;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.VsAddin.Factory
{
    /// <inheritdoc cref="IFixtureItemNodeStatusFactory"/>
    [ExcludeFromCodeCoverage]
    public class FixtureItemNodeStatusFactory : IFixtureItemNodeStatusFactory
    {
        #region fields

        private readonly StatusPanelFactory _statusPanelFactory;
        private readonly ErrorStatusIconFactory _errorStatusIconFactory;
        private readonly NoConfigurableMemberStatusIconFactory _noConfigurableMemberStatusIconFactory;
        private readonly ArrayNotConfigurableStatusIconViewModelFactory _arrayNotConfigurableStatusIconViewModelFactory;
        private readonly BaseTypeIsAlwaysUniqueStatusIconViewModelFactory _baseTypeIsAlwaysUniqueStatusIconViewModelFactory;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemNodeStatusFactory"/> class.
        /// </summary>
        /// <param name="statusPanelFactory"></param>
        /// <param name="errorStatusIconFactory"></param>
        /// <param name="noConfigurableMemberStatusIconFactory"></param>
        /// <param name="arrayNotConfigurableStatusIconViewModelFactory"></param>
        /// <param name="baseTypeIsAlwaysUniqueStatusIconViewModelFactory"></param>
        public FixtureItemNodeStatusFactory(
            StatusPanelFactory statusPanelFactory,
            ErrorStatusIconFactory errorStatusIconFactory,
            NoConfigurableMemberStatusIconFactory noConfigurableMemberStatusIconFactory,
            ArrayNotConfigurableStatusIconViewModelFactory arrayNotConfigurableStatusIconViewModelFactory,
            BaseTypeIsAlwaysUniqueStatusIconViewModelFactory baseTypeIsAlwaysUniqueStatusIconViewModelFactory)
        {
            this.EnsureMany()
                .Parameter(statusPanelFactory, nameof(statusPanelFactory))
                .Parameter(errorStatusIconFactory, nameof(errorStatusIconFactory))
                .Parameter(noConfigurableMemberStatusIconFactory, nameof(noConfigurableMemberStatusIconFactory))
                .Parameter(arrayNotConfigurableStatusIconViewModelFactory, nameof(arrayNotConfigurableStatusIconViewModelFactory))
                .Parameter(baseTypeIsAlwaysUniqueStatusIconViewModelFactory, nameof(baseTypeIsAlwaysUniqueStatusIconViewModelFactory))
                .ThrowWhenNull();

            this._arrayNotConfigurableStatusIconViewModelFactory = arrayNotConfigurableStatusIconViewModelFactory;
            this._baseTypeIsAlwaysUniqueStatusIconViewModelFactory = baseTypeIsAlwaysUniqueStatusIconViewModelFactory;
            this._statusPanelFactory = statusPanelFactory;
            this._errorStatusIconFactory = errorStatusIconFactory;
            this._noConfigurableMemberStatusIconFactory = noConfigurableMemberStatusIconFactory;
        }

        #endregion

        #region delegates

        /// <summary>
        /// Factory for autofac.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>A new instance of <see cref="ErrorStatusIconViewModel"/>.</returns>
        public delegate ErrorStatusIconViewModel ErrorStatusIconFactory(string message);

        /// <summary>
        /// Factory for autofac.
        /// </summary>
        /// <param name="panel"></param>
        /// <returns>A new instance of <see cref="NoConfigurableMemberStatusIconViewModel"/>.</returns>
        public delegate NoConfigurableMemberStatusIconViewModel NoConfigurableMemberStatusIconFactory(
            IStatusPanelViewModel panel);

        /// <summary>
        /// Factory for autofac.
        /// </summary>
        /// <param name="panel"></param>
        /// <returns>A new instance of <see cref="ArrayNotConfigurableStatusIconViewModel"/>.</returns>
        public delegate ArrayNotConfigurableStatusIconViewModel ArrayNotConfigurableStatusIconViewModelFactory(
            IStatusPanelViewModel panel);

        /// <summary>
        /// Factory for autofac.
        /// </summary>
        /// <param name="panel"></param>
        /// <returns>A new instance of <see cref="BaseTypeIsAlwaysUniqueStatusIconViewModel"/>.</returns>
        public delegate BaseTypeIsAlwaysUniqueStatusIconViewModel BaseTypeIsAlwaysUniqueStatusIconViewModelFactory(
            IStatusPanelViewModel panel);

        /// <summary>
        /// Factory for autofac.
        /// </summary>
        /// <returns>A new instance of <see cref="StatusPanelViewModel"/>.</returns>
        public delegate StatusPanelViewModel StatusPanelFactory();

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public IStatusPanelViewModel CreateStatusPanelViewModel() =>
            this._statusPanelFactory();

        /// <inheritdoc />
        public IStatusIconViewModel CreateErrorStatusIconViewModel(string message) =>
            this._errorStatusIconFactory(message);

        /// <inheritdoc />
        public IStatusIconViewModel CreateNoConfigurableMemberStatusIconViewModel(
            IStatusPanelViewModel panelViewModel) =>
            this._noConfigurableMemberStatusIconFactory(panelViewModel);

        /// <inheritdoc />
        public IStatusIconViewModel CreateArrayNotConfigurableStatusIconViewModel(
            IStatusPanelViewModel panelViewModel) =>
            this._arrayNotConfigurableStatusIconViewModelFactory(panelViewModel);

        /// <inheritdoc />
        public IStatusIconViewModel CreateBaseTypeIsAlwaysUniqueStatusIconViewModel(IStatusPanelViewModel panelViewModel) =>
            this._baseTypeIsAlwaysUniqueStatusIconViewModelFactory(panelViewModel);

        #endregion
    }
}
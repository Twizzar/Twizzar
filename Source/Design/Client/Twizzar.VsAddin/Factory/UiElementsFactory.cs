using System.Diagnostics.CodeAnalysis;

using Autofac;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status;
using Twizzar.Design.Ui.Interfaces.VisualStudio;
using Twizzar.SharedKernel.Factories;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.VsAddin.Factory
{
    /// <summary>
    /// The Factory to create the components needed by the UI.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UiElementsFactory : FactoryBase, IUiElementsFactory
    {
        #region fields

        private readonly AdornmentExpanderFactory _adornmentExpanderFactory;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="UiElementsFactory"/> class.
        /// </summary>
        /// <param name="componentContext">The component context.</param>
        /// <param name="adornmentExpanderFactory">The adornment expander factory.</param>
        public UiElementsFactory(
            IComponentContext componentContext,
            AdornmentExpanderFactory adornmentExpanderFactory)
            : base(componentContext)
        {
            this.EnsureMany()
                .Parameter(adornmentExpanderFactory, nameof(adornmentExpanderFactory))
                .ThrowWhenNull();

            this._adornmentExpanderFactory = adornmentExpanderFactory;
        }

        #endregion

        #region delegates

        /// <summary>
        /// Autofac factory for creating AdornmentExpander.
        /// </summary>
        /// <param name="adornmentId">The adornment id.</param>
        /// <param name="statusPanelViewModel"></param>
        /// <returns>A new instance of <see cref="IAdornmentExpander"/>.</returns>
        public delegate IAdornmentExpander AdornmentExpanderFactory(
            AdornmentId adornmentId,
            IStatusPanelViewModel statusPanelViewModel);

        #endregion

        #region members

        /// <inheritdoc />
        public IAdornmentExpander CreateAdornmentExpander(
            AdornmentId adornmentId,
            IStatusPanelViewModel statusPanelViewModel) =>
            this._adornmentExpanderFactory(adornmentId, statusPanelViewModel);

        #endregion
    }
}
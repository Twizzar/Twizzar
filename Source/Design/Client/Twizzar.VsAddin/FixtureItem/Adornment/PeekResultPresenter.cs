using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Utilities;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment.Peek.PeekResult;
using Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment.Peek.ResultPresenter;
using Twizzar.VsAddin.Interfaces.CompositionRoot;

namespace Twizzar.VsAddin.FixtureItem.Adornment
{
    /// <inheritdoc />
    [Export(typeof(IPeekResultPresenter))]
    [Name("twizzar Peek Presenter")]
    [ExcludeFromCodeCoverage]
    public class PeekResultPresenter : IPeekResultPresenter
    {
        #region fields

        [Import]
#pragma warning disable CS0649
        private readonly IIocOrchestrator _iocOrchestrator;
#pragma warning restore CS0649

        #endregion

        #region members

        /// <inheritdoc />
        public IPeekResultPresentation TryCreatePeekResultPresentation(IPeekResult result)
        {
            if (result is PeekResult)
            {
                return new PeekResultPresentation(
                    this._iocOrchestrator.Resolve<IUiEventHub>());
            }

            return null;
        }

        #endregion
    }
}
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status;

namespace Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem
{
    /// <summary>
    /// View Model of the definition UI, the definition UI Adornment and definition UI expander.
    /// </summary>
    public interface IFixtureItemViewModel : IFixtureItemNode, IDisposable
    {
        #region properties

        /// <summary>
        /// Gets a value indicating whether this instance is loading.
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// Gets the children nodes.
        /// </summary>
        new ObservableCollection<IFixtureItemNodeViewModel> Children { get; }

        #endregion

        #region members

        /// <summary>
        /// Updates the information async.
        /// </summary>
        /// <param name="adornmentInformation">The adornment information.</param>
        /// <param name="adornmentId"></param>
        /// <param name="documentWriter"></param>
        /// <param name="statusPanelViewModel"></param>
        /// <param name="compilationTypeQuery"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task InitializeAsync(
            IAdornmentInformation adornmentInformation,
            AdornmentId adornmentId,
            IDocumentWriter documentWriter,
            IStatusPanelViewModel statusPanelViewModel,
            ICompilationTypeQuery compilationTypeQuery,
            CancellationToken cancellationToken);

        /// <summary>
        /// Update the adornment information async.
        /// </summary>
        /// <param name="adornmentInformation"></param>
        /// <returns>A task.</returns>
        Task UpdateAsync(IAdornmentInformation adornmentInformation);

        #endregion
    }
}
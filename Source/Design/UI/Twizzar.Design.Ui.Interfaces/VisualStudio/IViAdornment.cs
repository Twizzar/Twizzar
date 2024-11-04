using System;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.Ui.Interfaces.VisualStudio
{
    /// <summary>
    /// The adornment. Contains the expander to open the fixture item panel and the panel itself.
    /// </summary>
    public interface IViAdornment : IEntity, IDisposable
    {
        #region properties

        /// <summary>
        /// Gets the adornment Id.
        /// </summary>
        public AdornmentId Id { get; }

        /// <summary>
        /// Gets the adornment information.
        /// </summary>
        public IAdornmentInformation AdornmentInformation { get; }

        /// <summary>
        /// Gets the adornment expander.
        /// </summary>
        public IAdornmentExpander AdornmentExpander { get; }

        /// <summary>
        /// Gets the status view model.
        /// </summary>
        public IStatusPanelViewModel StatusPanelViewModel { get; }

        #endregion

        #region members

        /// <summary>
        /// Updates the adornment information.
        /// </summary>
        /// <param name="adornmentInformation"></param>
        public void Update(IAdornmentInformation adornmentInformation);

        /// <summary>
        /// Update the is expanded state.
        /// </summary>
        /// <param name="isExpanded">The new value.</param>
        public void UpdateIsExpanded(bool isExpanded);

        #endregion
    }
}
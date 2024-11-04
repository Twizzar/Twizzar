using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Ui.Interfaces.Messaging.VsEvents
{
    /// <summary>
    /// Event triggered when the peek view is collapsed.
    /// </summary>
    public class PeekCollapsedEvent : IUiEvent, IHasEnsureHelper
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="PeekCollapsedEvent"/> class.
        /// </summary>
        /// <param name="adornmentId"></param>
        public PeekCollapsedEvent(AdornmentId adornmentId)
        {
            this.EnsureCtorParameterIsNotNull(adornmentId, nameof(adornmentId));
            this.AdornmentId = adornmentId;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the adornment id.
        /// </summary>
        public AdornmentId AdornmentId { get; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion
    }
}
using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.Ui.Interfaces.ValueObjects;

namespace Twizzar.Design.Ui.Interfaces.Messaging.VsEvents
{
    /// <summary>
    /// Event triggered when the adornment size changed.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AdornmentSizeChangedEvent : IUiEvent
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdornmentSizeChangedEvent"/> class.
        /// </summary>
        /// <param name="adornmentId"></param>
        /// <param name="adornmentInformation"></param>
        public AdornmentSizeChangedEvent(AdornmentId adornmentId, IAdornmentInformation adornmentInformation)
        {
            this.AdornmentId = adornmentId;
            this.AdornmentInformation = adornmentInformation;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the adornment id.
        /// </summary>
        public AdornmentId AdornmentId { get; }

        /// <summary>
        /// Gets the adornment information.
        /// </summary>
        public IAdornmentInformation AdornmentInformation { get; }

        #endregion
    }
}
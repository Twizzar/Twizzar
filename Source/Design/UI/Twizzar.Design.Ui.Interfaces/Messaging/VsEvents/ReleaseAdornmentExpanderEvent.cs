using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.Ui.Interfaces.ValueObjects;

namespace Twizzar.Design.Ui.Interfaces.Messaging.VsEvents
{
    /// <summary>
    /// Event for releasing the adornment expander.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ReleaseAdornmentExpanderEvent : IUiEvent
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReleaseAdornmentExpanderEvent"/> class.
        /// </summary>
        /// <param name="adornmentId"></param>
        public ReleaseAdornmentExpanderEvent(AdornmentId adornmentId)
        {
            this.AdornmentId = adornmentId;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the id of the adornment.
        /// </summary>
        public AdornmentId AdornmentId { get; }

        #endregion
    }
}
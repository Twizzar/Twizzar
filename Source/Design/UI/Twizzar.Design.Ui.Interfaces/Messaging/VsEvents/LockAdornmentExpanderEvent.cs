using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using ViCommon.EnsureHelper;

namespace Twizzar.Design.Ui.Interfaces.Messaging.VsEvents
{
    /// <summary>
    /// Event for locking the adornment expander to prevent expanding the fixture item peek view.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class LockAdornmentExpanderEvent : IUiEvent
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="LockAdornmentExpanderEvent"/> class.
        /// </summary>
        /// <param name="adornmentId">The adornment id.</param>
        public LockAdornmentExpanderEvent(AdornmentId adornmentId)
        {
            EnsureHelper.GetDefault.Parameter(adornmentId, nameof(adornmentId)).ThrowOnFailure();
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.Ui.Interfaces.Messaging.VsEvents
{
    /// <summary>
    /// Event triggered when the adornment is expanded or collapsed.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AdornmentExpandedOrCollapsedEvent : ValueObject, IUiEvent
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdornmentExpandedOrCollapsedEvent"/> class.
        /// </summary>
        /// <param name="adornmentId">The adornment id.</param>
        /// <param name="isExpanded">True when the adornment is expanded.</param>
        /// <param name="sender">The sender.</param>
        public AdornmentExpandedOrCollapsedEvent(AdornmentId adornmentId, bool isExpanded, object sender)
        {
            this.AdornmentId = adornmentId;
            this.IsExpanded = isExpanded;
            this.Sender = sender;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the id of the adornment.
        /// </summary>
        public AdornmentId AdornmentId { get; }

        /// <summary>
        /// Gets a value indicating whether the adornment expanded.
        /// </summary>
        public bool IsExpanded { get; }

        /// <summary>
        /// Gets the Sender of the event.
        /// </summary>
        public object Sender { get; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.IsExpanded;
        }

        #endregion
    }
}
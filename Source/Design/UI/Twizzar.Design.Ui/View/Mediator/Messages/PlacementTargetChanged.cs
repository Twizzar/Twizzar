using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Twizzar.Design.Ui.View.Mediator.Messages
{
    /// <summary>
    /// The message notifies, that placement target has changed.
    /// </summary>
    public class PlacementTargetChanged : MediatorMessageBase
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="PlacementTargetChanged"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="placementRect">The placement rectangle.</param>
        public PlacementTargetChanged(object sender, Rect placementRect)
            : base(sender)
        {
            this.PlacementRect = placementRect;
        }

        #endregion

        #region properties

        /// <summary>
        /// gets the new placement rectangle.
        /// </summary>
        public Rect PlacementRect { get; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents() =>
            base.GetEqualityComponents().Append(this.PlacementRect);

        #endregion
    }
}
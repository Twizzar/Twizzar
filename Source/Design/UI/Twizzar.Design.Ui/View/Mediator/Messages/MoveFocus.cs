using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Twizzar.Design.Ui.View.Mediator.Messages
{
    /// <summary>
    /// This message notifies that the focus should be moved to another text box.
    /// </summary>
    public class MoveFocus : MediatorMessageBase
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveFocus"/> class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="direction"></param>
        public MoveFocus(object sender, FocusNavigationDirection direction)
            : base(sender)
        {
            this.Direction = direction;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the move direction.
        /// </summary>
        public FocusNavigationDirection Direction { get; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents() =>
            base.GetEqualityComponents().Append(this.Direction);

        #endregion
    }
}
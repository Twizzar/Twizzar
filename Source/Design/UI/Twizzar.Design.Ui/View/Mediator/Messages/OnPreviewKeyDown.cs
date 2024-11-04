using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Ui.View.Mediator.Messages
{
    /// <summary>
    /// The message to close the auto complete.
    /// </summary>
    public class OnPreviewKeyDown : MediatorMessageBase
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="OnPreviewKeyDown"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="keyEventArgs"><see cref="KeyEventArgs"/>.</param>
        public OnPreviewKeyDown(object sender, KeyEventArgs keyEventArgs)
            : base(sender)
        {
            this.EnsureParameter(keyEventArgs, nameof(keyEventArgs)).ThrowWhenNull();
            this.KeyEventArgs = keyEventArgs;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the key event arguments from the wpf PreviewKeyDown events.
        /// </summary>
        public KeyEventArgs KeyEventArgs { get; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents() =>
            base.GetEqualityComponents().Append(this.KeyEventArgs);

        #endregion
    }
}
using System.Collections.Generic;
using System.Linq;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Ui.View.Mediator.Messages
{
    /// <summary>
    /// The message to start the auto complete.
    /// </summary>
    public class RichTextBoxTextChanged : MediatorMessageBase
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextBoxTextChanged"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="caretSpan">The replacing span. Represents the text span from
        /// the text box begin to the current caret pos.</param>
        public RichTextBoxTextChanged(
            object sender,
            RichTextBoxSpan caretSpan)
            : base(sender)
        {
            this.EnsureParameter(caretSpan, nameof(caretSpan)).ThrowWhenNull();
            this.CaretSpan = caretSpan;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the caret span.
        /// </summary>
        public RichTextBoxSpan CaretSpan { get; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents() =>
            base.GetEqualityComponents().Append(this.CaretSpan);

        #endregion
    }
}
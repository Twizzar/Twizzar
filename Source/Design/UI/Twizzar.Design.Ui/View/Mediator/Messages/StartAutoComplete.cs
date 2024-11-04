using System.Collections.Generic;
using System.Linq;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Ui.View.Mediator.Messages
{
    /// <summary>
    /// The message to start the auto complete.
    /// </summary>
    public class StartAutoComplete : MediatorMessageBase
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="StartAutoComplete"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="caretSpan">The replacing span. Represents the text span from
        /// the text box begin to the current caret pos.</param>
        /// <param name="autoCompleteEntries">The auto complete entries.</param>
        public StartAutoComplete(
            object sender,
            RichTextBoxSpan caretSpan,
            IEnumerable<AutoCompleteEntry> autoCompleteEntries)
            : base(sender)
        {
            this.EnsureMany()
                .Parameter(caretSpan, nameof(caretSpan))
                .Parameter(autoCompleteEntries, nameof(autoCompleteEntries))
                .ThrowWhenNull();
            this.CaretSpan = caretSpan;
            this.AutoCompleteEntries = autoCompleteEntries;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the replacing span.
        /// </summary>
        public RichTextBoxSpan CaretSpan { get; }

        /// <summary>
        /// Gets the auto complete entries.
        /// </summary>
        public IEnumerable<AutoCompleteEntry> AutoCompleteEntries { get; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents() =>
            base.GetEqualityComponents().
                Append(this.CaretSpan).
                Append(this.AutoCompleteEntries);

        #endregion
    }
}
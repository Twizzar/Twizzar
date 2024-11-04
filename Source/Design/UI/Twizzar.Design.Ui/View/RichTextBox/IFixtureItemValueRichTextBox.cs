using System.Collections.Generic;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.View.Mediator;
using Twizzar.Design.Ui.View.Mediator;

namespace Twizzar.Design.Ui.View.RichTextBox
{
    /// <summary>
    /// Interaction logic for FixtureItemValueRichTextBox.
    /// </summary>
    public interface IFixtureItemValueRichTextBox : IAutoCompleteParticipant
    {
        /// <summary>
        /// Gets the entries of the auto complete list.
        /// </summary>
        IEnumerable<AutoCompleteEntry> AutoCompleteEntries { get; }

        /// <summary>
        /// Get the <see cref="RichTextBoxSpan"/> from document start to the caret position.
        /// </summary>
        /// <returns>The <see cref="RichTextBoxSpan"/> from document start to the caret position.</returns>
        RichTextBoxSpan GetCaretSpan();

        /// <summary>
        /// Gets the full text size.
        /// </summary>
        /// <returns>The width and the height of the content.</returns>
        (double Width, double Height) GetFullTextSize();
    }
}
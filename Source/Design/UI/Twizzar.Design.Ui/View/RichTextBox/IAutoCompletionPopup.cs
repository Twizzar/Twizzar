using System.Collections.Generic;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.View.Mediator;
using Twizzar.Design.Ui.View.Mediator;

namespace Twizzar.Design.Ui.View.RichTextBox
{
    /// <summary>
    /// Interaction logic for AutoCompletionPopup.
    /// </summary>
    public interface IAutoCompletionPopup : IAutoCompleteParticipant
    {
        #region properties

        /// <summary>
        /// Gets the selected auto complete entry.
        /// </summary>
        AutoCompleteEntry SelectedAutoCompleteEntry { get; }

        /// <summary>
        /// Gets a value indicating whether the popup is open.
        /// </summary>
        bool IsOpen { get; }

        #endregion

        #region members

        /// <summary>
        /// Filter the autocomplete list.
        /// </summary>
        /// <param name="autoCompleteEntries">The total autocomplete entries.</param>
        /// <param name="caretSpan">The caret span.</param>
        void FilterAutoCompletes(IEnumerable<AutoCompleteEntry> autoCompleteEntries, RichTextBoxSpan caretSpan);

        /// <summary>
        /// select the next item in the AutoCompleteListBox.
        /// </summary>
        void SelectNext();

        /// <summary>
        /// select the previous item in the AutoCompleteListBox.
        /// </summary>
        void SelectPrevious();

        #endregion
    }
}
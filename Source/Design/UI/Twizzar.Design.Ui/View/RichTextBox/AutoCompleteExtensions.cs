using System.Collections.Generic;
using System.Linq;
using Twizzar.Design.Ui.Interfaces.Enums;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.View.Mediator;

namespace Twizzar.Design.Ui.View.RichTextBox
{
    /// <summary>
    /// Class implements extensions for the auto complete classes.
    /// </summary>
    public static class AutoCompleteExtensions
    {
        #region members

        /// <summary>
        /// Filter an auto complete list according to the caret span.
        /// </summary>
        /// <param name="autoCompleteEntries">The original auto complete entries.</param>
        /// <param name="caretSpan">The Span from the start of the document until the caret position.</param>
        /// <returns>The filtered auto complete entries.</returns>
        public static IEnumerable<AutoCompleteEntry> Filter(
            this IEnumerable<AutoCompleteEntry> autoCompleteEntries,
            RichTextBoxSpan caretSpan)
        {
            var result = autoCompleteEntries
                .Where(
                    entry =>
                        entry.Text
                            .ToLowerInvariant()
                            .Contains(caretSpan.Text.Trim().ToLowerInvariant()))
                .ToList();

            if (!result.Any())
            {
                result.Add(new AutoCompleteEntry("No suggestions", AutoCompleteFormat.None));
            }

            return result;
        }

        #endregion
    }
}
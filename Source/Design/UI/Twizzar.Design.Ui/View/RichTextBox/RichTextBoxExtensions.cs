using System;
using System.Linq;
using System.Windows.Documents;
using Twizzar.Design.Ui.View.Mediator;

namespace Twizzar.Design.Ui.View.RichTextBox
{
    /// <summary>
    /// Class implement extensions for <see cref="RichTextBox"/>.
    /// </summary>
    public static class RichTextBoxExtensions
    {
        #region members

        /// <summary>
        /// Gets the full text of the document of the RichTextBox.
        /// </summary>
        /// <param name="richTextBox">The RichTextBox.</param>
        /// <returns>The full text of the document of the RichTextBox.</returns>
        public static string FullText(this System.Windows.Controls.RichTextBox richTextBox)
        {
            var fullText = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
            fullText = fullText.Replace(Environment.NewLine, string.Empty);
            return fullText;
        }

        /// <summary>
        /// Gets the run independent caret position in the text.
        /// </summary>
        /// <param name="richTextBox">The rich text box.</param>
        /// <returns>The caret index in the text.</returns>
        public static int GetCaretPosition(this System.Windows.Controls.RichTextBox richTextBox)
        {
            var text = GetCaretText(richTextBox);
            var indexInText = text.Length;
            return indexInText;
        }

        /// <summary>
        /// Get the <see cref="RichTextBoxSpan"/> from document start to the caret position.
        /// </summary>
        /// <param name="richTextBox">The <see cref="System.Windows.Controls.RichTextBox"/> to extend.</param>
        /// <returns>The <see cref="RichTextBoxSpan"/> from document start to the caret position.</returns>
        public static RichTextBoxSpan GetCaretSpan(this System.Windows.Controls.RichTextBox richTextBox)
        {
            var caretText = GetCaretText(richTextBox);
            var caretSpan = new RichTextBoxSpan(0, caretText.Length, caretText);
            return caretSpan;
        }

        /// <summary>
        /// Replace the text in the rich text box. This will convert all runs in just one run.
        /// </summary>
        /// <param name="richTextBox">The <see cref="System.Windows.Controls.RichTextBox"/> to extend.</param>
        /// <param name="richTextBoxSpan">The <see cref="RichTextBoxSpan"/> to replace.</param>
        /// <param name="newText">The new text, that replace the <see cref="RichTextBoxSpan"/> in the rich text box.</param>
        public static void ReplaceText(
            this System.Windows.Controls.RichTextBox richTextBox,
            RichTextBoxSpan richTextBoxSpan,
            string newText)
        {
            var paragraph = richTextBox?.Document?.Blocks.OfType<Paragraph>().FirstOrDefault();
            if (paragraph == null)
            {
                return;
            }

            var originalText = richTextBox.FullText();
            originalText = originalText.Remove(richTextBoxSpan.Start, richTextBoxSpan.End - richTextBoxSpan.Start);
            var replacedText = originalText.Insert(richTextBoxSpan.Start, newText);

            paragraph.Inlines.Clear();
            var inline = new Run(replacedText);
            paragraph.Inlines.Add(inline);

            richTextBox.CaretPosition =
                inline.ContentStart.GetPositionAtOffset(newText.Length, LogicalDirection.Forward);
        }

        /// <summary>
        /// Sets the caret index in the text and in the right run.
        /// This is necessary because of the hidden positions.
        /// </summary>
        /// <param name="richTextBox">The rich text box.</param>
        /// <param name="indexInText">The run independent caret position in the text.</param>
        public static void SetCaretPosition(this System.Windows.Controls.RichTextBox richTextBox, int indexInText)
        {
            if (indexInText < 0)
            {
                return;
            }

            var document = richTextBox?.Document;

            if (document == null)
            {
                return;
            }

            if (indexInText == 0)
            {
                richTextBox.CaretPosition = document.ContentStart;
            }

            var paragraph = document.Blocks.OfType<Paragraph>().FirstOrDefault();

            if (paragraph == null)
            {
                return;
            }

            var runs = paragraph.Inlines.OfType<Run>();

            // find position and run to which place caret
            var pos = 0;

            foreach (var run in runs)
            {
                pos += run.Text.Length;

                if (pos >= indexInText)
                {
                    // restore caret position
                    richTextBox.CaretPosition =
                        run.ContentEnd.GetPositionAtOffset(indexInText - pos, LogicalDirection.Forward);

                    return;
                }
            }
        }

        /// <summary>
        /// Gets the text from start to caret.
        /// </summary>
        /// <param name="richTextBox">The <see cref="System.Windows.Controls.RichTextBox"/> to extend.</param>
        /// <returns>The text from start to the caret position.</returns>
        private static string GetCaretText(this System.Windows.Controls.RichTextBox richTextBox)
        {
            var caretTextRange = GetCaretTextRange(richTextBox);
            var text = caretTextRange.Text.Replace(Environment.NewLine, string.Empty);
            return text;
        }

        /// <summary>
        /// Get the <see cref="TextRange"/> from document start to the caret position.
        /// </summary>
        /// <param name="richTextBox">The <see cref="System.Windows.Controls.RichTextBox"/> to extend.</param>
        /// <returns>The <see cref="TextRange"/> from document start to the caret position.</returns>
        private static TextRange GetCaretTextRange(this System.Windows.Controls.RichTextBox richTextBox)
        {
            var start = richTextBox.Document.ContentStart;
            var caret = richTextBox.CaretPosition;
            var range = new TextRange(start, caret);
            return range;
        }

        #endregion
    }
}
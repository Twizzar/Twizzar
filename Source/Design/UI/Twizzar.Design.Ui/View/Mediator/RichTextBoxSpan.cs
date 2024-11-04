using System.Collections.Generic;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.Ui.View.Mediator
{
    /// <summary>
    /// The RichTextBoxSpan class.
    /// </summary>
    public class RichTextBoxSpan : ValueObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextBoxSpan"/> class.
        /// </summary>
        /// <param name="start">The start of the span.</param>
        /// <param name="end">The end of the span.</param>
        /// <param name="text">The span text.</param>
        public RichTextBoxSpan(int start, int end, string text)
        {
            this.Start = start;
            this.End = end;
            this.Text = text;
        }

        /// <summary>
        /// Gets the start of the span.
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// Gets the end of the span.
        /// </summary>
        public int End { get; }

        /// <summary>
        /// Gets the text of the span.
        /// </summary>
        public string Text { get; }

        #region Overrides of ValueObject

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Start;
            yield return this.End;
            yield return this.Text;
        }

        #endregion
    }
}
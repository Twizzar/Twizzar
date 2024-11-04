using System.Collections.Generic;
using System.Linq;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Ui.View.Mediator.Messages
{
    /// <summary>
    /// The Apply Auto complete message.
    /// </summary>
    public class ApplyAutoComplete : MediatorMessageBase
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyAutoComplete"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="richTextBoxSpan">The replacing span.</param>
        /// <param name="selectedString">The selected string to apply.</param>
        public ApplyAutoComplete(object sender, RichTextBoxSpan richTextBoxSpan, string selectedString)
            : base(sender)
        {
            this.EnsureMany()
                .Parameter(richTextBoxSpan, nameof(richTextBoxSpan))
                .Parameter(selectedString, nameof(selectedString))
                .ThrowWhenNull();

            this.RichTextBoxSpan = richTextBoxSpan;
            this.SelectedString = selectedString;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets teh <see cref="RichTextBoxSpan"/>.
        /// </summary>
        public RichTextBoxSpan RichTextBoxSpan { get; }

        /// <summary>
        /// Gets the text of the selected <see cref="AutoCompleteEntry"/>.
        /// </summary>
        public string SelectedString { get; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents() =>
            base.GetEqualityComponents().Append(this.RichTextBoxSpan).Append(this.SelectedString);

        #endregion
    }
}
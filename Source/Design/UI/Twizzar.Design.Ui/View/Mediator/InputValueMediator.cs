using System.Windows.Input;
using Twizzar.Design.Ui.Interfaces.Enums;
using Twizzar.Design.Ui.Interfaces.View.Mediator;
using Twizzar.Design.Ui.View.Mediator.Messages;
using Twizzar.Design.Ui.View.RichTextBox;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Ui.View.Mediator
{
    /// <summary>
    /// Class <inheritdoc cref="IMediator"/>.
    /// </summary>
    public class InputValueMediator : IMediator, IHasEnsureHelper
    {
        #region fields

        private readonly IFixtureItemValueRichTextBox _valueRichTextBox;

        private readonly IAutoCompletionPopup _autocompleteControl;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="InputValueMediator"/> class.
        /// </summary>
        /// <param name="valueRichTextBox">The <see cref="IFixtureItemValueRichTextBox"/> as <see cref="IAutoCompleteParticipant"/>.</param>
        /// <param name="autocompleteControl">The <see cref="IAutoCompletionPopup"/> as <see cref="IAutoCompleteParticipant"/>.</param>
        public InputValueMediator(
            IFixtureItemValueRichTextBox valueRichTextBox,
            IAutoCompletionPopup autocompleteControl)
        {
            this.EnsureMany()
                .Parameter(valueRichTextBox, nameof(valueRichTextBox))
                .Parameter(autocompleteControl, nameof(autocompleteControl))
                .ThrowWhenNull();

            this._valueRichTextBox = valueRichTextBox;
            this._autocompleteControl = autocompleteControl;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public void Notify(IMediatorMessage mediatorMessage)
        {
            switch (mediatorMessage.Sender)
            {
                case IFixtureItemValueRichTextBox _:
                    this.ReactToRichTextBoxChanges(mediatorMessage);

                    break;
                case IAutoCompletionPopup _:
                    this._valueRichTextBox.RespondToMediator(mediatorMessage);
                    break;
            }
        }

        private void ReactToRichTextBoxChanges(IMediatorMessage mediatorMessage)
        {
            switch (mediatorMessage)
            {
                case OnPreviewKeyDown onPreviewKeyDown:
                    switch (onPreviewKeyDown.KeyEventArgs.Key)
                    {
                        case Key.Space when Keyboard.Modifiers == ModifierKeys.Control:
                        case Key.OemPeriod when Keyboard.Modifiers == ModifierKeys.Control:
                            this.StartAutoComplete();
                            break;
                        case Key.Down when this._autocompleteControl.IsOpen:
                            this._autocompleteControl.SelectNext();
                            break;
                        case Key.Up when this._autocompleteControl.IsOpen:
                            this._autocompleteControl.SelectPrevious();
                            break;
                        case Key.Up when !this._autocompleteControl.IsOpen:
                            this._valueRichTextBox.RespondToMediator(new MoveFocus(this, FocusNavigationDirection.Up));
                            break;
                        case Key.Down when !this._autocompleteControl.IsOpen:
                            this._valueRichTextBox.RespondToMediator(new MoveFocus(this, FocusNavigationDirection.Down));
                            break;
                        case Key.Enter:
                            this.HandleEnterKeyFromRichTextBox();
                            break;
                        case Key.Escape when this._autocompleteControl.IsOpen:
                            this.ClosePopUp();
                            onPreviewKeyDown.KeyEventArgs.Handled = true;
                            break;
                        case Key.Escape:
                            this.HandleEnterKeyFromRichTextBox();
                            break;
                    }

                    break;
                case RichTextBoxTextChanged richTextBoxTextChanged:

                    if (this._autocompleteControl.IsOpen)
                    {
                        this._autocompleteControl.FilterAutoCompletes(
                            this._valueRichTextBox.AutoCompleteEntries,
                            richTextBoxTextChanged.CaretSpan);
                    }

                    break;
            }

            this._autocompleteControl.RespondToMediator(mediatorMessage);
        }

        private void HandleEnterKeyFromRichTextBox()
        {
            if (this._autocompleteControl.IsOpen)
            {
                var selectedItem = this._autocompleteControl.SelectedAutoCompleteEntry;
                var autoCompleteText = selectedItem?.Text;
                var richTextBoxSpan = this._valueRichTextBox.GetCaretSpan();

                if (selectedItem != null && !string.IsNullOrEmpty(autoCompleteText) &&
                    selectedItem.Format != AutoCompleteFormat.None && richTextBoxSpan != null)
                {
                    var applyAutoComplete = new ApplyAutoComplete(this, richTextBoxSpan, autoCompleteText);
                    this._valueRichTextBox.RespondToMediator(applyAutoComplete);
                }

                this.ClosePopUp();
            }
            else
            {
                this._valueRichTextBox.RespondToMediator(new CommitTotalValue(this));
            }
        }

        private void StartAutoComplete()
        {
            var richTextBoxSpan = this._valueRichTextBox.GetCaretSpan();
            var autoCompleteEntries = this._valueRichTextBox.AutoCompleteEntries;

            if (richTextBoxSpan == null || autoCompleteEntries == null)
            {
                return;
            }

            var startAutoComplete = new StartAutoComplete(
                this,
                richTextBoxSpan,
                autoCompleteEntries);

            this._autocompleteControl.RespondToMediator(startAutoComplete);
        }

        private void ClosePopUp()
        {
            this._autocompleteControl.RespondToMediator(new CloseAutoComplete(this));
        }

        #endregion
    }
}
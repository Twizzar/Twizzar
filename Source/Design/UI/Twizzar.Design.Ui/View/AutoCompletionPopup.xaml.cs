using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Twizzar.Design.Ui.Interfaces.Enums;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.View.Mediator;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.View.Mediator;
using Twizzar.Design.Ui.View.Mediator.Messages;
using Twizzar.Design.Ui.View.RichTextBox;

namespace Twizzar.Design.Ui.View
{
    /// <summary>
    /// Interaction logic for AutoCompletionPopup.xaml.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class AutoCompletionPopup : IAutoCompletionPopup
    {
        #region fields

        private IMediator _mediator;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCompletionPopup"/> class.
        /// </summary>
        public AutoCompletionPopup()
        {
            this.InitializeComponent();
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public AutoCompleteEntry SelectedAutoCompleteEntry =>
            this.AutoCompleteListBox?.SelectedItem as AutoCompleteEntry;

        #endregion

        #region members

        /// <inheritdoc />
        public void InitializeMediator(IMediator mediator) =>
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        /// <inheritdoc />
        public void RespondToMediator(IMediatorMessage message)
        {
            if (this.DataContext is IFixtureItemNodeViewModel vm && vm.Value.IsReadOnly)
            {
                return;
            }

            switch (message)
            {
                case CloseAutoComplete _:
                    this.HandleCloseAutoComplete();
                    break;
                case StartAutoComplete startAutoComplete:
                    this.HandleStartAutoComplete(startAutoComplete);
                    break;
                case PlacementTargetChanged placementTargetChanged:
                    this.HandlePlacementTargetChanged(placementTargetChanged);
                    break;
            }
        }

        /// <inheritdoc/>
        public void FilterAutoCompletes(IEnumerable<AutoCompleteEntry> autoCompleteEntries, RichTextBoxSpan caretSpan)
        {
            if (autoCompleteEntries == null || caretSpan == null)
            {
                return;
            }

            this.AutoCompleteListBox.ItemsSource = autoCompleteEntries.Filter(caretSpan);
        }

        /// <inheritdoc />
        public void SelectNext()
        {
            if (!this.IsOpen)
            {
                return;
            }

            if (this.AutoCompleteListBox == null || this.AutoCompleteListBox.Items.IsEmpty)
            {
                return;
            }

            if (this.AutoCompleteListBox.SelectedIndex == this.AutoCompleteListBox.Items.Count - 1)
            {
                this.AutoCompleteListBox.SelectedIndex = 0;
            }
            else
            {
                this.AutoCompleteListBox.SelectedIndex += 1;
            }

            this.AutoCompleteListBox.ScrollIntoView(this.AutoCompleteListBox.SelectedItem);
        }

        /// <inheritdoc />
        public void SelectPrevious()
        {
            if (!this.IsOpen)
            {
                return;
            }

            if (this.AutoCompleteListBox == null || this.AutoCompleteListBox.Items.IsEmpty)
            {
                return;
            }

            if (this.AutoCompleteListBox.SelectedIndex <= 0)
            {
                this.AutoCompleteListBox.SelectedIndex = this.AutoCompleteListBox.Items.Count - 1;
            }
            else
            {
                this.AutoCompleteListBox.SelectedIndex -= 1;
            }

            this.AutoCompleteListBox.ScrollIntoView(this.AutoCompleteListBox.SelectedItem);
        }

        /// <inheritdoc />
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.OriginalSource is DependencyObject dependencyObject &&
                ItemsControl.ContainerFromElement(this.AutoCompleteListBox, dependencyObject) is ListBoxItem item)
            {
                var autoCompleteEntry = item.DataContext as AutoCompleteEntry;

                if (autoCompleteEntry != null && !string.IsNullOrEmpty(autoCompleteEntry.Text))
                {
                    if (autoCompleteEntry.Format != AutoCompleteFormat.None)
                    {
                        this._mediator.Notify(new ApplyAutoComplete(
                            this,
                            new RichTextBoxSpan(0, 0, string.Empty),
                            autoCompleteEntry.Text));
                    }

                    this.HandleCloseAutoComplete();
                    e.Handled = true;
                }
            }

            base.OnPreviewMouseLeftButtonDown(e);
        }

        private void HandlePlacementTargetChanged(PlacementTargetChanged placementTargetChanged)
        {
            if (this.IsOpen)
            {
                var offset = this.HorizontalOffset;
                this.HorizontalOffset = offset + 0.1;
                this.HorizontalOffset = offset;
                this.VerticalOffset = placementTargetChanged.PlacementRect.Height;
                this.HorizontalOffset = this.AutoCompleteListBox.ActualWidth;
            }
        }

        private void HandleStartAutoComplete(StartAutoComplete startAutoComplete)
        {
            this.FilterAutoCompletes(startAutoComplete.AutoCompleteEntries, startAutoComplete.CaretSpan);
            this.IsOpen = true;
        }

        private void HandleCloseAutoComplete()
        {
            this.IsOpen = false;
        }

        #endregion
    }
}
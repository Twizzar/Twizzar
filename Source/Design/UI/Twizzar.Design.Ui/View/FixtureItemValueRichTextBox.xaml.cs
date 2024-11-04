using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.CoreInterfaces.Common.Util.Routine;
using Twizzar.Design.Ui.Interfaces.Messaging.VsEvents;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.View;
using Twizzar.Design.Ui.Interfaces.View.Mediator;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.View.Mediator;
using Twizzar.Design.Ui.View.Mediator.Messages;
using Twizzar.Design.Ui.View.RichTextBox;
using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.Functional;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Ui.View
{
    /// <summary>
    /// Interaction logic for FixtureItemValueRichTextBox.xaml.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class FixtureItemValueRichTextBox : IFixtureItemValueRichTextBox
    {
        #region fields

        private readonly object _lock = new();
        private readonly RoutineRunner _moveFocusRunner = new();
        private Maybe<IMediator> _mediator = Maybe.None();
        private double _charWidth;
        private double _charHeight;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemValueRichTextBox"/> class.
        /// </summary>
        public FixtureItemValueRichTextBox()
        {
            this.InitializeComponent();

            this.DataContextChanged += this.OnDataContextChanged;
            this.LayoutUpdated += this.OnLayoutUpdated;
            this.ConnectTextChanged();
            (this._charWidth, this._charHeight) = this.GetCharSize();
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public IEnumerable<AutoCompleteEntry> AutoCompleteEntries { get; private set; }

        private Maybe<IFixtureItemNodeValueViewModel> FixtureItemNodeValueViewModel =>
            Maybe.ToMaybe(this.DataContext as IFixtureItemNodeValueViewModel);

        #endregion

        #region members

        /// <inheritdoc />
        public RichTextBoxSpan GetCaretSpan() => RichTextBoxExtensions.GetCaretSpan(this);

        /// <inheritdoc />
        public void InitializeMediator(IMediator mediator) =>
            this._mediator = Maybe.Some(mediator ?? throw new ArgumentNullException(nameof(mediator)));

        /// <inheritdoc />
        public void RespondToMediator(IMediatorMessage message)
        {
            try
            {
                switch (message)
                {
                    case ApplyAutoComplete applyAutoComplete:
                        this.DisconnectTextChanged();

                        try
                        {
                            this.ReplaceText(this.GetCaretSpan(), applyAutoComplete.SelectedString);
                        }
                        finally
                        {
                            this.ConnectTextChanged();
                        }

                        this.FixtureItemNodeValueViewModel.IfSome(model => model.FullText = this.FullText());
                        break;
                    case CommitTotalValue _:
                        this.CommitTotalValue();
                        break;
                    case MoveFocus moveFocusMessage:
                        this.MoveFocus(moveFocusMessage.Direction);
                        break;
                }
            }
            catch (Exception e)
            {
                this.Log(e);
            }
        }

        /// <inheritdoc />
        public (double Width, double Height) GetFullTextSize() => (this.GetFullTextWidth(), this._charHeight);

        /// <inheritdoc />
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            this.SendToMediator(new OnPreviewKeyDown(this, e));
        }

        /// <inheritdoc />
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            this.FixtureItemNodeValueViewModel.IfSome(model => model.HasFocus = false);
            this.CommitTotalValue();
            this._mediator.IfSome(mediator => mediator.Notify(new CloseAutoComplete(this)));
            base.OnLostFocus(e);
        }

        /// <inheritdoc />
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            this.FixtureItemNodeValueViewModel.IfSome(model => model.HasFocus = true);
        }

        /// <inheritdoc />
        protected override void OnDpiChanged(DpiScale oldDpiScaleInfo, DpiScale newDpiScaleInfo)
        {
            base.OnDpiChanged(oldDpiScaleInfo, newDpiScaleInfo);
            (this._charWidth, this._charHeight) = this.GetCharSize();
        }

        private Maybe<T> GetService<T>()
            where T : class =>
                this.FixtureItemNodeValueViewModel.Bind(model => model.ServiceProvider.GetService<T>());

        private void MoveFocus(FocusNavigationDirection direction)
        {
            try
            {
                this._moveFocusRunner.Run(this.MoveFocusRoutine(direction));
            }
            catch (Exception exp)
            {
                this.Log(exp);
            }
        }

        private IEnumerable<ICancelInstruction> MoveFocusRoutine(FocusNavigationDirection direction)
        {
            // preconditions
            yield return new CancelWhen(() => !this.HasEffectiveKeyboardFocus);
            yield return new PreventSpam(TimeSpan.FromMilliseconds(200));

            var next = this.PredictFocus(direction);

            // next will be null if the caret is at the first richTextBox or at the last.
            if (next is FixtureItemValueRichTextBox)
            {
                this.MoveFocus(new TraversalRequest(direction));
            }
            else
            {
                var leaveDirection = direction switch
                {
                    FocusNavigationDirection.Up => FixtureItemPanelLeaveRequestEvent.LeaveDirection.Up,
                    FocusNavigationDirection.Down => FixtureItemPanelLeaveRequestEvent.LeaveDirection.Down,
                    _ => throw new PatternErrorBuilder(nameof(direction))
                        .IsNotOneOf(nameof(FocusNavigationDirection.Up), nameof(FocusNavigationDirection.Down)),
                };

                this.GetService<IUiEventHub>()
                    .IfSome(
                        hub =>
                            hub.Publish(
                                new FixtureItemPanelLeaveRequestEvent(leaveDirection)));
            }
        }

        /// <summary>
        /// Notifies the mediator.
        /// </summary>
        /// <param name="mediatorMessage">The <see cref="IMediatorMessage"/>.</param>
        private void SendToMediator(IMediatorMessage mediatorMessage)
        {
            this._mediator.IfSome(mediator => mediator.Notify(mediatorMessage));
        }

        private void CommitTotalValue()
        {
            this.FixtureItemNodeValueViewModel.IfSome(
                model => model.Commit.Execute(null));
        }

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            var placementRect = LayoutInformation.GetLayoutSlot(this);
            this.SendToMediator(new PlacementTargetChanged(this, placementRect));
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            this.GetService<IUiEventHub>()
                .IfSome(
                    eventHub =>
                    {
                        this.SubscribeToPropertyChanges(
                            (INotifyPropertyChanged)args.OldValue,
                            (INotifyPropertyChanged)args.NewValue);

                        this.OnPropertyChanged(
                            this,
                            new PropertyChangedEventArgs(nameof(IFixtureItemNodeValueViewModel.ItemValueSegments)));

                        this.OnPropertyChanged(
                            this,
                            new PropertyChangedEventArgs(nameof(IFixtureItemNodeValueViewModel.AutoCompleteEntries)));

                        eventHub.Unsubscribe<VsThemeChangedEvent>(
                            this,
                            this.HandleVsColorThemeChanged);

                        eventHub.Subscribe<VsThemeChangedEvent>(
                            this,
                            this.HandleVsColorThemeChanged);

                        eventHub.Unsubscribe<FixtureItemNodeFocusedEvent>(
                            this,
                            this.HandleFocused);

                        eventHub.Subscribe<FixtureItemNodeFocusedEvent>(
                            this,
                            this.HandleFocused);
                    });
        }

        private void HandleFocused(FixtureItemNodeFocusedEvent e)
        {
            if (e.NodeId == this.FixtureItemNodeValueViewModel.Match(model => model.Id, () => null))
            {
                this.Focus();
                Keyboard.Focus(this);
            }
        }

        // subscriber
        private void SubscribeToPropertyChanges(
            INotifyPropertyChanged oldViewModel,
            INotifyPropertyChanged newViewModel)
        {
            if (oldViewModel != null)
            {
                oldViewModel.PropertyChanged -= this.OnPropertyChanged;
            }

            if (newViewModel != null)
            {
                newViewModel.PropertyChanged += this.OnPropertyChanged;
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (this.FixtureItemNodeValueViewModel.IsNone ||
                (args.PropertyName != nameof(IFixtureItemNodeValueViewModel.ItemValueSegments) &&
                 args.PropertyName != nameof(IFixtureItemNodeValueViewModel.AutoCompleteEntries)))
            {
                return;
            }

            var vm = this.FixtureItemNodeValueViewModel.GetValueUnsafe();

            try
            {
                this.DisconnectTextChanged();

                if (args.PropertyName == nameof(IFixtureItemNodeValueViewModel.ItemValueSegments))
                {
                    var itemValueSegments = vm.ItemValueSegments ?? new List<ItemValueSegment>();

                    this.OnItemValueSegmentsChanged(itemValueSegments.ToList(), vm);
                }

                if (args.PropertyName == nameof(vm.AutoCompleteEntries))
                {
                    var autoCompleteEntries = vm.AutoCompleteEntries ?? new List<AutoCompleteEntry>();

                    this.OnAutoCompleteEntriesChanged(autoCompleteEntries);
                }
            }
            finally
            {
                this.ConnectTextChanged();
            }
        }

        private void OnAutoCompleteEntriesChanged(IEnumerable<AutoCompleteEntry> autoCompleteEntries)
        {
            var completeEntries = autoCompleteEntries?.ToList() ?? new List<AutoCompleteEntry>();
            this.AutoCompleteEntries = completeEntries;
            this.SendToMediator(new AutoCompleteEntriesChanged(this, completeEntries));
        }

        private void ConnectTextChanged()
        {
            this.DisconnectTextChanged();
            this.TextChanged += this.HandleTextChanged;
        }

        private void HandleTextChanged(object sender, TextChangedEventArgs e)
        {
            var richTextBoxSpan = this.GetCaretSpan() ?? new RichTextBoxSpan(0, 0, string.Empty);

            this.SendToMediator(new RichTextBoxTextChanged(this, richTextBoxSpan));

            this.FixtureItemNodeValueViewModel.IfSome(
                model =>
                {
                    var fullText = this.FullText();
                    model.FullText = fullText;
                });
        }

        private void DisconnectTextChanged()
        {
            this.TextChanged -= this.HandleTextChanged;
        }

        private void HandleVsColorThemeChanged(VsThemeChangedEvent eventArgs)
        {
            this.FixtureItemNodeValueViewModel.IfSome(
                model =>
                {
                    if (model.ItemValueSegments.Any())
                    {
                        this.OnItemValueSegmentsChanged(model.ItemValueSegments.ToList(), model);
                    }
                });
        }

        private async void OnItemValueSegmentsChanged(
            IList<ItemValueSegment> itemValueSegments,
            IFixtureItemNodeValueViewModel fixtureItemNodeValueViewModel)
        {
            try
            {
                var itemValueSegmentToRunConverter = fixtureItemNodeValueViewModel.ServiceProvider
                    .GetService<IItemValueSegmentToRunConverter>();

                await itemValueSegmentToRunConverter.IfSomeAsync(async converter =>
                {
                    if (!itemValueSegments.Any() || string.IsNullOrEmpty(fixtureItemNodeValueViewModel.FullText))
                    {
                        this.Document.Blocks.Clear();
                        this.Document.Blocks.Add(new Paragraph());
                        return;
                    }

                    var runs = await converter.ConvertToRunsAsync(itemValueSegments);

                    var caretPosition = this.GetCaretPosition();
                    var paragraph = new Paragraph();
                    paragraph.Inlines.AddRange(runs);
                    this.Document.Blocks.Clear();
                    this.Document.Blocks.Add(paragraph);

                    // Set the page width to string length * char width. This only works with monospaced typeface font.
                    this.Document.PageWidth = Math.Max(this.GetFullTextWidth() * 2, 300);

                    this.SetCaretPosition(caretPosition);
                });
            }
            catch (Exception e)
            {
                this.Log(e);
            }
        }

        private (double Widht, double Height) GetCharSize(string text = "a")
        {
            // lock this part because the get dpi function is not thread safe.
            lock (this._lock)
            {
                var dpiInfo = VisualTreeHelper.GetDpi(this);

                var formattedText = new FormattedText(
                    text,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(
                        this.Document.FontFamily,
                        this.Document.FontStyle,
                        this.Document.FontWeight,
                        this.Document.FontStretch),
                    this.Document.FontSize,
                    Brushes.Black,
                    new NumberSubstitution(),
                    TextFormattingMode.Display,
                    dpiInfo.PixelsPerDip);

                return (formattedText.Width, formattedText.Height);
            }
        }

        private double GetFullTextWidth() => this.FullText().Length * this._charWidth;

        #endregion
    }
}
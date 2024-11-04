using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.Interfaces.VisualStudio;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Ui.View
{
    /// <summary>
    /// RichTextBox Adorner for displaying additional information like default value and fixture kind.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FixtureItemValueRichTextBoxAdorner : Adorner
    {
        private const int MarginX = 4;
        private const int MarginY = 0;
        private const int MarginYAdorner = (FontSizeDefault - FontSizeAdorner) / 2;
        private const int FontSizeDefault = 12;
        private const int FontSizeAdorner = 10;
        private readonly IFixtureItemNodeValueViewModel _fixtureItemNodeValueViewModel;
        private readonly WeakReference<FixtureItemValueRichTextBox> _richTextBox;
        private readonly Brush _adornerBackgroundBrush;
        private readonly Typeface _font = new("Consolas");
        private readonly Brush _adornerTextBrush;
        private readonly Brush _defaultTextBrush;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemValueRichTextBoxAdorner"/> class.
        /// </summary>
        /// <param name="richTextBox"></param>
        /// <param name="fixtureItemNodeValueViewModel"></param>
        public FixtureItemValueRichTextBoxAdorner(
            FixtureItemValueRichTextBox richTextBox,
            IFixtureItemNodeValueViewModel fixtureItemNodeValueViewModel)
            : base(richTextBox)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(richTextBox, nameof(richTextBox))
                .Parameter(fixtureItemNodeValueViewModel, nameof(fixtureItemNodeValueViewModel))
                .ThrowWhenNull();

            this._richTextBox = new WeakReference<FixtureItemValueRichTextBox>(richTextBox);
            this._fixtureItemNodeValueViewModel = fixtureItemNodeValueViewModel;

            if (this._fixtureItemNodeValueViewModel is INotifyPropertyChanged propertyChangedNotifier)
            {
                propertyChangedNotifier.PropertyChanged += this.OnPropertyChanged;
            }

            // allows to click through adorner.
            this.IsHitTestVisible = false;

            if (this._fixtureItemNodeValueViewModel.ServiceProvider.GetService<IVsColorService>().AsMaybeValue() is SomeValue<IVsColorService> someColorService)
            {
                var colorService = someColorService.Value;

                // initialize brushes:

                this._adornerBackgroundBrush = this.FindResource(colorService.ResolveVsColor(VsColors.BackgroundSecondaryBrush)) as Brush;
                this._adornerTextBrush = this.FindResource(colorService.ResolveVsColor(VsColors.TextSecondaryBrush)) as Brush;
                this._defaultTextBrush = this.FindResource(colorService.ResolveVsColor(VsColors.TextTertiaryBrush)) as Brush;
            }
        }

        #region Overrides of UIElement

        /// <inheritdoc />
        protected override void OnRender(DrawingContext drawingContext)
        {
            // do nothing when no default needs to be displayed (fullText not empty)
            // and no adornerText is set
            if (string.IsNullOrEmpty(this._fixtureItemNodeValueViewModel.AdornerText) &&
                !string.IsNullOrEmpty(this._fixtureItemNodeValueViewModel.FullText))
            {
                return;
            }

            // get richTextBox dimensions
            if (!this._richTextBox.TryGetTarget(out var richTextBox))
            {
                return;
            }

            var (width, height) = richTextBox.GetFullTextSize();
            var richTextBoxRect = new Rect(MarginX, MarginY, width, height);

            // get default text and draw it
            FormattedText defaultText = null;
            if (string.IsNullOrEmpty(this._fixtureItemNodeValueViewModel.FullText))
            {
                defaultText = this.CreateDefaultText();
                drawingContext.DrawText(defaultText, richTextBoxRect.TopLeft);
            }

            // get adorner text and draw it with background
            if (!string.IsNullOrEmpty(this._fixtureItemNodeValueViewModel.AdornerText))
            {
                var adornerText = this.CreateAdornerText();
                var adornerRect = GetAdornerPosition(richTextBoxRect, adornerText, defaultText);

                drawingContext.DrawRectangle(this._adornerBackgroundBrush, null, adornerRect);
                drawingContext.DrawText(adornerText, adornerRect.TopLeft);
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this._fixtureItemNodeValueViewModel.AdornerText)
                || e.PropertyName == nameof(this._fixtureItemNodeValueViewModel.FullText))
            {
                // forces adorner to re render
                this.InvalidateVisual();
            }
        }

        private static Rect GetAdornerPosition(Rect richTextBoxRect, FormattedText text, FormattedText defaultText)
        {
            var xPosition = richTextBoxRect.Right + MarginX;
            var yPosition = richTextBoxRect.Top + MarginYAdorner;

            if (defaultText != null)
            {
                xPosition += defaultText.Width;
            }

            return new Rect(
                xPosition,
                yPosition,
                text.Width + 2,
                text.Height);
        }

        private FormattedText CreateAdornerText() =>
            this.CreateFormattedText(
                this._fixtureItemNodeValueViewModel.AdornerText,
                this._adornerTextBrush,
                FontSizeAdorner);

        private FormattedText CreateDefaultText() =>
            this.CreateFormattedText(
                this._fixtureItemNodeValueViewModel.DefaultValue,
                this._defaultTextBrush,
                FontSizeDefault);

        private FormattedText CreateFormattedText(string content, Brush textBrush, int fontSize)
        {
            if (!this._richTextBox.TryGetTarget(out var richTextBox))
            {
                return default;
            }

            var dpiInfo = VisualTreeHelper.GetDpi(richTextBox);

            var txt = new FormattedText(
                content,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                this._font,
                fontSize,
                textBrush,
                new NumberSubstitution(),
                TextFormattingMode.Display,
                dpiInfo.PixelsPerDip);

            txt.SetFontStyle(FontStyles.Italic);

            return txt;
        }

        #endregion

    }
}

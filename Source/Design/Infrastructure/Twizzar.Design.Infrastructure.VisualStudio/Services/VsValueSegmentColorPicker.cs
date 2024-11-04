using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Twizzar.Design.Ui.Interfaces.Enums;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.View;
using Twizzar.Design.Ui.Interfaces.VisualStudio;
using Twizzar.SharedKernel.NLog.Interfaces;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.Extensions;

using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace Twizzar.Design.Infrastructure.VisualStudio.Services
{
    /// <summary>
    /// The visual studio segment color picker. Colors will be read from options and change according to selected theme.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class VsValueSegmentColorPicker : IValueSegmentColorPicker
    {
        private readonly IVsColorService _vsColorService;
        private IDictionary _resourceDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="VsValueSegmentColorPicker"/> class.
        /// </summary>
        /// <param name="vsColorService"></param>
        public VsValueSegmentColorPicker(IVsColorService vsColorService)
        {
            this.EnsureParameter(vsColorService, nameof(vsColorService));
            this._vsColorService = vsColorService;
        }

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public void InitializeFallback(IDictionary resource)
        {
            this._resourceDictionary = resource;
        }

        /// <inheritdoc />
        public async Task<Brush> GetSegmentColor(ItemValueSegment itemValueSegment)
        {
            var brush = itemValueSegment.Format switch
            {
                SegmentFormat.None => await this._vsColorService.GetVsOptionBrushAsync("Error"),
                SegmentFormat.Type => await this._vsColorService.GetVsOptionBrushAsync("Type"),
                SegmentFormat.Letter => await this._vsColorService.GetVsOptionBrushAsync("String"),
                SegmentFormat.Number => await this._vsColorService.GetVsOptionBrushAsync("Literal"),
                SegmentFormat.Boolean => await this._vsColorService.GetVsOptionBrushAsync("Keyword"),
                SegmentFormat.DefaultOrUndefined => await this._vsColorService.GetVsOptionBrushAsync("Keyword"),
                SegmentFormat.Keyword => await this._vsColorService.GetVsOptionBrushAsync("Keyword"),
                SegmentFormat.Id => await this._vsColorService.GetVsOptionBrushAsync("Line Number"),
                SegmentFormat.SelectedCtor => await this._vsColorService.GetVsOptionBrushAsync("Line Number"),
                SegmentFormat.ReadonlyCode => await this._vsColorService.GetVsOptionBrushAsync("Stale Code"),
                _ => throw new ArgumentOutOfRangeException(nameof(itemValueSegment)),
            };

            return brush.SomeOrProvided(() => this.GetFallbackBrush(itemValueSegment.Format));
        }

        private Brush GetFallbackBrush(SegmentFormat format)
        {
            return format switch
            {
                SegmentFormat.None => this.GetFallbackBrush("UnknownForeground"),
                SegmentFormat.Type => this.GetFallbackBrush("TypeForeground"),
                SegmentFormat.Letter => this.GetFallbackBrush("StringForeground"),
                SegmentFormat.Number => this.GetFallbackBrush("LiteralForeground"),
                SegmentFormat.Boolean => this.GetFallbackBrush("BooleanForeground"),
                SegmentFormat.DefaultOrUndefined => this.GetFallbackBrush("KeywordForeground"),
                SegmentFormat.Keyword => this.GetFallbackBrush("KeywordForeground"),
                SegmentFormat.Id => this.GetFallbackBrush("LinkForeground"),
                SegmentFormat.SelectedCtor => this.GetFallbackBrush("SelectedCtorForeground"),
                SegmentFormat.ReadonlyCode => this.GetFallbackBrush("ReadonlyCodeForeground"),
                _ => throw new ArgumentOutOfRangeException(nameof(format)),
            };
        }

        private Brush GetFallbackBrush(string brushKey) =>
            this._resourceDictionary != null && this._resourceDictionary.Contains(brushKey)
                ? (Brush)this._resourceDictionary[brushKey]
                : Brushes.Red;

        #endregion
    }
}

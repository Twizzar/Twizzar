using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Media;
using Twizzar.Design.Ui.Interfaces.Enums;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.View;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;

namespace Twizzar.Design.Ui.View.RichTextBox
{
    /// <summary>
    /// Dummy implementation for value segment color picker. Only for test and  DemoApp usage.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DummyValueSegmentColorPicker : IValueSegmentColorPicker
    {
        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region Implementation of IValueSegmentColorPicker

        /// <inheritdoc />
        public Task<Brush> GetSegmentColor(ItemValueSegment itemValueSegment) =>
            itemValueSegment.Format switch
            {
                SegmentFormat.None => Task.FromResult<Brush>(Brushes.Red),
                SegmentFormat.Type => Task.FromResult<Brush>(Brushes.DarkCyan),
                SegmentFormat.Letter => Task.FromResult<Brush>(Brushes.Maroon),
                SegmentFormat.Number => Task.FromResult<Brush>(Brushes.CadetBlue),
                SegmentFormat.Boolean => Task.FromResult<Brush>(Brushes.CornflowerBlue),
                SegmentFormat.DefaultOrUndefined => Task.FromResult<Brush>(Brushes.CornflowerBlue),
                SegmentFormat.Keyword => Task.FromResult<Brush>(Brushes.CornflowerBlue),
                SegmentFormat.Id => Task.FromResult<Brush>(Brushes.DarkCyan),
                SegmentFormat.SelectedCtor => Task.FromResult<Brush>(Brushes.DarkCyan),
                _ => throw new ArgumentOutOfRangeException(nameof(itemValueSegment)),
            };

        /// <inheritdoc />
        public void InitializeFallback(IDictionary resource)
        {
            // do nothing
        }

        #endregion
    }
}

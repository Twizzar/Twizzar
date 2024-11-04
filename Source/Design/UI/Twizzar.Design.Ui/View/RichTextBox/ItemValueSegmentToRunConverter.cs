using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Documents;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.View;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Ui.View.RichTextBox
{
    /// <summary>
    /// Convert segments of an input value to Runs with specific styling.
    /// </summary>
    public class ItemValueSegmentToRunConverter : IItemValueSegmentToRunConverter
    {
        #region fields

        private readonly IValueSegmentColorPicker _valueSegmentColorPicker;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemValueSegmentToRunConverter"/> class.
        /// </summary>
        /// <param name="valueSegmentColorPicker">The <see cref="IValueSegmentColorPicker"/>.</param>
        public ItemValueSegmentToRunConverter(IValueSegmentColorPicker valueSegmentColorPicker)
        {
            this.EnsureParameter(valueSegmentColorPicker, nameof(valueSegmentColorPicker)).ThrowWhenNull();
            this._valueSegmentColorPicker = valueSegmentColorPicker;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public async Task<IList<Run>> ConvertToRunsAsync(IEnumerable<ItemValueSegment> itemValueSegments)
        {
            var result = new List<Run>();

            foreach (var segment in itemValueSegments)
            {
                var run = await this.ConvertToRunAsync(segment);
                result.Add(run);
            }

            return result;
        }

        private async Task<Run> ConvertToRunAsync(ItemValueSegment itemValueSegment)
        {
            var brush = await this._valueSegmentColorPicker.GetSegmentColor(itemValueSegment);

            var run = new Run(itemValueSegment.Content)
            {
                FontSize = 12,
                Foreground = brush,
            };

            return run;
        }

        #endregion
    }
}
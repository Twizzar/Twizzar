using System.Collections;
using System.Threading.Tasks;
using System.Windows.Media;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.Ui.Interfaces.View
{
    /// <summary>
    /// Interface to get the color of a value segment according to its <see cref="ItemValueSegment.Format"/>.
    /// </summary>
    public interface IValueSegmentColorPicker : IService
    {
        /// <summary>
        /// Gets the segment color.
        /// </summary>
        /// <param name="itemValueSegment">The segment of the fixture item value.</param>
        /// <returns>The color. <see cref="System.Windows.Media.Brush"/>.</returns>
        Task<Brush> GetSegmentColor(ItemValueSegment itemValueSegment);

        /// <summary>
        /// Initialized fallback dictionary.
        /// </summary>
        /// <param name="resource"></param>
        void InitializeFallback(IDictionary resource);
    }
}
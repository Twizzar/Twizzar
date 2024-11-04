using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Documents;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.Ui.Interfaces.View
{
    /// <summary>
    /// Interface for converting item value segments to runs.
    /// </summary>
    public interface IItemValueSegmentToRunConverter : IService
    {
        /// <summary>
        /// Convert the parsed value segments to richTextBox runs.
        /// </summary>
        /// <param name="itemValueSegments">the item value segments. <see cref="ItemValueSegment"/>.</param>
        /// <returns>The converted list of runs.</returns>
        Task<IList<Run>> ConvertToRunsAsync(IEnumerable<ItemValueSegment> itemValueSegments);
    }
}
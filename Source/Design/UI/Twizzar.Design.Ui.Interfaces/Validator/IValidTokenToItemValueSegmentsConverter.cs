using System.Collections.Generic;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.Ui.Interfaces.Validator
{
    /// <summary>
    /// Converter for ItemValueSegment.
    /// </summary>
    public interface IValidTokenToItemValueSegmentsConverter : IService
    {
        /// <summary>
        /// Coverts the syntax tree to the segments.
        /// </summary>
        /// <param name="validatedToken">The validated token.</param>
        /// <returns>ItemValueSegments used by the ui.</returns>
        IEnumerable<ItemValueSegment> ToItemValueSegments(IViToken validatedToken);
    }
}
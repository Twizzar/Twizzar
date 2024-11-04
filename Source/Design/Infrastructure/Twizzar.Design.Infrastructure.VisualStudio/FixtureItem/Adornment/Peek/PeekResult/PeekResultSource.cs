using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.VisualStudio.Language.Intellisense;

namespace Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment.Peek.PeekResult
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class PeekResultSource : IPeekResultSource
    {
        #region Implementation of IPeekResultSource

        /// <inheritdoc />
        public void FindResults(
            string relationshipName,
            IPeekResultCollection resultCollection,
            CancellationToken cancellationToken,
            IFindPeekResultsCallback callback)
        {
            var result = new Peek.PeekResult.PeekResult();
            resultCollection.Add(result);
        }

        #endregion
    }
}
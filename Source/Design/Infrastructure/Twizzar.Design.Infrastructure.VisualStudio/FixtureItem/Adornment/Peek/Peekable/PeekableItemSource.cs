using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Language.Intellisense;
using Twizzar.Design.Ui.Interfaces.ValueObjects;

namespace Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment.Peek.Peekable
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public sealed class PeekableItemSource : IPeekableItemSource
    {
        #region Implementation of IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            // Do nothing
        }

        /// <inheritdoc />
        public void AugmentPeekSession(IPeekSession session, IList<IPeekableItem> peekableItems)
        {
            if (session.RelationshipName.StartsWith(AdornmentId.Prefix))
            {
                peekableItems.Add(new PeekableItem(session.RelationshipName));
            }
        }

        #endregion
    }
}
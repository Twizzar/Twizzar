using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.VisualStudio.Language.Intellisense;

using Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment.Peek.PeekResult;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;

namespace Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment.Peek.Peekable
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public sealed class PeekableItem : IPeekableItem
    {
        #region fields

        private readonly AdornmentId _adornmentId;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="PeekableItem"/> class.
        /// </summary>
        /// <param name="relationShip">The name of the relationship between <see cref="T:Microsoft.VisualStudio.Language.Intellisense.IPeekableItem" />s and <see cref="T:Microsoft.VisualStudio.Language.Intellisense.IPeekResult" />s.</param>
        public PeekableItem(string relationShip)
        {
            this._adornmentId = AdornmentId.Parse(relationShip)
                .Match(
                    id => id,
                    failure =>
                    {
                        this.Log(failure.Message, LogLevel.Error);
                        return AdornmentId.CreateNew("invalid");
                    });
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public string DisplayName => "TWIZZAR";

        /// <inheritdoc />
        public IEnumerable<IPeekRelationship> Relationships => new[] { new PeekRelationship(this._adornmentId) };

        #endregion

        #region members

        /// <inheritdoc />
        public IPeekResultSource GetOrCreateResultSource(string relationshipName) =>
            new PeekResultSource();

        #endregion
    }
}
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment.Peek.Peekable;

namespace Twizzar.VsAddin.FixtureItem.Adornment
{
    /// <inheritdoc />
    [Export(typeof(IPeekableItemSourceProvider))]
    [ContentType("text")]
    [Name("TWIZZAR")]
    [ExcludeFromCodeCoverage]
    public class PeekableItemSourceProvider : IPeekableItemSourceProvider
    {
        #region Implementation of IPeekableItemSourceProvider

        /// <inheritdoc />
        public IPeekableItemSource TryCreatePeekableItemSource(ITextBuffer textBuffer) =>
            new PeekableItemSource();

        #endregion
    }
}
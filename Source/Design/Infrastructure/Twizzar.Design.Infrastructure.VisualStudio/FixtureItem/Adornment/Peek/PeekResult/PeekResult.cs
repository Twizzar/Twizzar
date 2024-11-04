using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Language.Intellisense;

namespace Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment.Peek.PeekResult
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public sealed class PeekResult : IPeekResult
    {
        #region events

        /// <inheritdoc />
        public event EventHandler Disposed;

        #endregion

        #region properties

        /// <inheritdoc />
        public IPeekResultDisplayInfo DisplayInfo => new PeekResultDisplayInfo("TWIZZAR", null, "TWIZZAR", "TWIZZAR");

        /// <inheritdoc />
        public bool CanNavigateTo => false;

        /// <inheritdoc />
        public Action<IPeekResult, object, object> PostNavigationCallback => default;

        #endregion

        #region members

        /// <inheritdoc />
        public void Dispose()
        {
            this.Disposed?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public void NavigateTo(object data)
        {
            // do nothing
        }

        #endregion
    }
}
using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;

namespace Twizzar.Design.Infrastructure.VisualStudio.Common.FixtureItem.Adornment
{
    /// <inheritdoc cref="IAdornmentInformation"/>
    [ExcludeFromCodeCoverage]
    public record AdornmentInformation(
        IViSpan ObjectCreationSpan,
        IViSpan ObjectCreationTypeSpan,
        bool MultipleAdornmentsOnLine,
        FixtureItemId FixtureItemId,
        string ProjectName,
        string DocumentFilePath) : IAdornmentInformation
    {
        #region members

        /// <inheritdoc />
        public IAdornmentInformation UpdateVersion(IViSpanVersion viSpanVersion) =>
            this with
            {
                ObjectCreationSpan = this.ObjectCreationSpan.WithVersion(viSpanVersion),
                ObjectCreationTypeSpan = this.ObjectCreationTypeSpan.WithVersion(viSpanVersion),
            };

        /// <inheritdoc />
        public IAdornmentInformation UpdateSpans(IViSpan objectCreationSpan, IViSpan objectTypeCreationSpan) =>
            this with
            {
                ObjectCreationSpan = objectCreationSpan,
                ObjectCreationTypeSpan = objectTypeCreationSpan,
            };

        #endregion
    }
}
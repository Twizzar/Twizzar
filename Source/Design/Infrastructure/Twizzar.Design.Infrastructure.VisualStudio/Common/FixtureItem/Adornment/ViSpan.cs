using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.Infrastructure.VisualStudio.Common.FixtureItem.Adornment
{
    /// <inheritdoc cref="IViSpan" />
    [ExcludeFromCodeCoverage]
    public class ViSpan : ValueObject, IViSpan
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViSpan"/> class.
        /// </summary>
        /// <param name="start">The start of the span.</param>
        /// <param name="length">The length of the span.</param>
        /// <param name="version">The version of the span default is unknown version.</param>
        public ViSpan(int start, int length, IViSpanVersion version = null)
        {
            this.Start = start;
            this.Length = length;
            this.Version = version ?? ViSpanVersion.UnknownVersion;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public int Start { get; }

        /// <inheritdoc />
        public int Length { get; }

        /// <inheritdoc />
        public IViSpanVersion Version { get; }

        #endregion

        /// <inheritdoc />
        public IViSpan WithVersion(IViSpanVersion version) =>
            new ViSpan(this.Start, this.Length, version);

        #region Overrides of ValueObject

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Start;
            yield return this.Length;
            yield return this.Version;
        }

        #endregion
    }
}
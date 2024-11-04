using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.CoreInterfaces.Adornment
{
    /// <inheritdoc cref="IViSpanVersion" />
    [ExcludeFromCodeCoverage]
    public class ViSpanVersion : ValueObject, IViSpanVersion
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViSpanVersion"/> class.
        /// </summary>
        /// <param name="versionNumber">The version number.</param>
        public ViSpanVersion(int versionNumber)
        {
            this.VersionNumber = versionNumber;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets a new unknown version.
        /// </summary>
        public static ViSpanVersion UnknownVersion =>
            new(-1);

        /// <inheritdoc />
        public int VersionNumber { get; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.VersionNumber;
        }

        #endregion
    }
}
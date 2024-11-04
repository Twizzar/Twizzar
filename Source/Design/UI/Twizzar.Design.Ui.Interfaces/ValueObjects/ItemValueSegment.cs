using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.Ui.Interfaces.Enums;
using Twizzar.SharedKernel.CoreInterfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Ui.Interfaces.ValueObjects
{
    /// <summary>
    /// Value object class to implement the segment of the value of a fixture item.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ItemValueSegment : ValueObject, IHasEnsureHelper
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemValueSegment"/> class.
        /// </summary>
        /// <param name="content"><see cref="Content"/>.</param>
        /// <param name="format"><see cref="Format"/>.</param>
        /// <param name="isValid"><see cref="IsValid"/>.</param>
        public ItemValueSegment(string content, SegmentFormat format, bool isValid)
        {
            this.EnsureParameter(content, nameof(content))
                .ThrowWhenNull();

            this.IsValid = isValid;
            this.Content = content;
            this.Format = format;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the content of the value segment.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Gets the Format of the value segment.
        /// </summary>
        public SegmentFormat Format { get; }

        /// <summary>
        /// Gets a value indicating whether this fixture item value segment is valid or not.
        /// </summary>
        public bool IsValid { get; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Content;
            yield return this.Format;
            yield return this.IsValid;
        }

        #endregion
    }
}
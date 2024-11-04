using System.Collections.Generic;
using Twizzar.Design.Ui.Interfaces.Enums;
using Twizzar.SharedKernel.CoreInterfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Ui.Interfaces.ValueObjects
{
    /// <summary>
    /// An Implementation of an AutoComplete entry.
    /// </summary>
    public class AutoCompleteEntry : ValueObject, IHasEnsureHelper
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCompleteEntry"/> class.
        /// </summary>
        /// <param name="text"><see cref="Text"/>.</param>
        /// <param name="format"><see cref="Format"/>.</param>
        public AutoCompleteEntry(string text, AutoCompleteFormat format)
        {
            this.EnsureParameter(text, nameof(text)).
                ThrowWhenNull();
            this.Text = text;
            this.Format = format;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the text of an Autocomplete entry.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the format of an Autocomplete entry.
        /// Used for example to choose which Icon to use in the AutoComplete ui element.
        /// </summary>
        public AutoCompleteFormat Format { get; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Text;
            yield return this.Format;
        }

        #endregion
    }
}
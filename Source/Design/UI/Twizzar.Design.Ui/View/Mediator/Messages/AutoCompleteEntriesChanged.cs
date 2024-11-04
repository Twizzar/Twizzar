using System.Collections.Generic;
using System.Linq;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Ui.View.Mediator.Messages
{
    /// <summary>
    /// Message to tell, that the original auto complete list has been changed.
    /// </summary>
    public class AutoCompleteEntriesChanged : MediatorMessageBase
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCompleteEntriesChanged"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="autoCompleteEntries">The new auto complete entries.</param>
        public AutoCompleteEntriesChanged(object sender, IEnumerable<AutoCompleteEntry> autoCompleteEntries)
            : base(sender)
        {
            this.EnsureParameter(autoCompleteEntries, nameof(autoCompleteEntries)).ThrowWhenNull();
            this.AutoCompleteEntries = autoCompleteEntries;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the new auto complete entries.
        /// </summary>
        public IEnumerable<AutoCompleteEntry> AutoCompleteEntries { get; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents() =>
            base.GetEqualityComponents().Append(this.AutoCompleteEntries);

        #endregion
    }
}
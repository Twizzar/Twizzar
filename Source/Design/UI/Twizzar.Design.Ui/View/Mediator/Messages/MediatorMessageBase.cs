using System.Collections.Generic;
using Twizzar.Design.Ui.Interfaces.View.Mediator;
using Twizzar.SharedKernel.CoreInterfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Ui.View.Mediator.Messages
{
    /// <summary>
    /// The MediatorMessageBase class.
    /// </summary>
    public abstract class MediatorMessageBase : ValueObject, IMediatorMessage, IHasEnsureHelper
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="MediatorMessageBase"/> class.
        /// </summary>
        /// <param name="sender">The sender object of this message.</param>
        protected MediatorMessageBase(object sender)
        {
            this.EnsureParameter(sender, nameof(sender)).ThrowWhenNull();
            this.Sender = sender;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public object Sender { get; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Sender;
        }

        #endregion
    }
}
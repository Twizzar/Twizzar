namespace Twizzar.Design.Ui.Interfaces.View.Mediator
{
    /// <summary>
    /// The IMediatorMessage interface.
    /// </summary>
    public interface IMediatorMessage
    {
        #region properties

        /// <summary>
        /// Gets the sender of the message.
        /// </summary>
        object Sender { get; }

        #endregion
    }
}
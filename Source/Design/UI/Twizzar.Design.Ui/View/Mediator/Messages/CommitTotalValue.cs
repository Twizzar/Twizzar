namespace Twizzar.Design.Ui.View.Mediator.Messages
{
    /// <summary>
    /// The message to commit the total value.
    /// </summary>
    public class CommitTotalValue : MediatorMessageBase
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommitTotalValue"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public CommitTotalValue(object sender)
            : base(sender)
        {
        }

        #endregion
    }
}
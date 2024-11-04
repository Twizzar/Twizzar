namespace Twizzar.Design.Ui.View.Mediator.Messages
{
    /// <summary>
    /// The message to close the auto complete.
    /// </summary>
    public class CloseAutoComplete : MediatorMessageBase
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseAutoComplete"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public CloseAutoComplete(object sender)
            : base(sender)
        {
        }

        #endregion
    }
}
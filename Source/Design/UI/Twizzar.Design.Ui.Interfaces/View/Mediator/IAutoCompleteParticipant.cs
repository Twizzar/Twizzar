namespace Twizzar.Design.Ui.Interfaces.View.Mediator
{
    /// <summary>
    /// The auto complete participant in. This ist used to implement tha mediator pattern.
    /// </summary>
    public interface IAutoCompleteParticipant
    {
        #region members

        /// <summary>
        /// initializes the mediator.
        /// </summary>
        /// <param name="mediator">the auto complete mediator.</param>
        void InitializeMediator(IMediator mediator);

        /// <summary>
        /// Responds to a message from the auto complete mediator.
        /// </summary>
        /// <param name="message">The message from the mediator.</param>
        void RespondToMediator(IMediatorMessage message);

        #endregion
    }
}
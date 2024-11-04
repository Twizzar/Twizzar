namespace Twizzar.Design.Ui.Interfaces.View.Mediator
{
    /// <summary>
    /// The mediator interface.
    /// </summary>
    public interface IMediator
    {
        #region members

        /// <summary>
        /// Notify the mediator to communicate with other participant.
        /// </summary>
        /// <param name="mediatorMessage">The <see cref="IMediatorMessage"/>.</param>
        void Notify(IMediatorMessage mediatorMessage);

        #endregion
    }
}
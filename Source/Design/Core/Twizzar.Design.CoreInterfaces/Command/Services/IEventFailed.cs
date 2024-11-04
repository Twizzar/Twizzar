namespace Twizzar.Design.CoreInterfaces.Command.Services
{
    /// <summary>
    /// A event representing a failure.
    /// </summary>
    public interface IEventFailed
    {
        /// <summary>
        /// Gets a text explaining why it failed.
        /// </summary>
        public string Reason { get; }
    }
}

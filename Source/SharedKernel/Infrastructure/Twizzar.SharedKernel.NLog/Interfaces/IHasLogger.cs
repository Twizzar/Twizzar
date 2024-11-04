namespace Twizzar.SharedKernel.NLog.Interfaces
{
    /// <summary>
    /// Implements Logger functionality.
    /// </summary>
    public interface IHasLogger
    {
        /// <summary>
        /// Gets or sets the ILogger for logging purpose.
        /// </summary>
        ILogger Logger { get; set; }
    }
}

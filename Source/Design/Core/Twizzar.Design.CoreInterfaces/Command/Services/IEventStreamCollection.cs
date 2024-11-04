using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.Command.Services
{
    /// <summary>
    /// Collection of event streams for in memory storage of event messages.
    /// </summary>
    public interface IEventStreamCollection : IHasEnsureHelper, IHasLogger
    {
        /// <summary>
        /// Adds the message to a dedicated stream.
        /// </summary>
        /// <param name="eventMessage">The event Message to store.</param>
        void Add(EventMessage eventMessage);

        /// <summary>
        /// Gets the requested stream.
        /// </summary>
        /// <returns>The stream.</returns>
        Maybe<IEventStream> GetStream();

        /// <summary>
        /// Gets the requested stream.
        /// </summary>
        /// <param name="key">The key that identifies the stream.</param>
        /// <returns>The stream.</returns>
        Maybe<IEventStream> GetStream(string key);

        /// <summary>
        /// Clears the stream and removes all stored event messages.
        /// </summary>
        /// <param name="key">Key identifying the stream dedicated for a root item.</param>
        void ClearStream(string key);
    }
}
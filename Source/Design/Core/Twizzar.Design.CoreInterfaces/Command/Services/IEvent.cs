// ReSharper disable once UnusedTypeParameter

using Twizzar.SharedKernel.CoreInterfaces;
#pragma warning disable S2326 // Unused type parameters should be removed

namespace Twizzar.Design.CoreInterfaces.Command.Services
{
    /// <summary>
    /// Represents a passed event.
    /// </summary>
    /// <typeparam name="TSelf">The type of it self. Is used for getting the correct type when resolving the <see cref="IEventListener{TEvent}"/>.</typeparam>
    public interface IEvent<out TSelf> : IEvent
        where TSelf : IEvent
    {
    }

    /// <summary>
    /// Represents a passed event.
    /// </summary>
    public interface IEvent : IValueObject
    {
        /// <summary>
        /// Create a string for logging. This method should anonymize data.
        /// </summary>
        /// <returns></returns>
        internal string ToLogString();
    }
}
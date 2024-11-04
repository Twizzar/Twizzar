using Twizzar.SharedKernel.CoreInterfaces;

#pragma warning disable SA1600 // Elements should be documented

// ReSharper disable once UnusedTypeParameter

namespace Twizzar.Design.CoreInterfaces.Command.Services
{
    /// <summary>
    /// Command interface for commands.
    /// </summary>
    /// <typeparam name="TSelf">The type of it self. Is used for getting the correct type when resolving the <see cref="IEventListener{TEvent}"/>.</typeparam>
    public interface ICommand<in TSelf> : ICommand, IValueObject
        where TSelf : ICommand
    {
    }

    /// <summary>
    /// Command interface for commands, when implementing comments use the <see cref="ICommand{TSelf}"/>.
    /// </summary>
    public interface ICommand
    {
    }
}

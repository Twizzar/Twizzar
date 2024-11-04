using Autofac;

namespace Twizzar.Design.Infrastructure.VisualStudio
{
    /// <summary>
    /// The entry point of the adding.
    /// This class start all services needed at the start of the AddIn. And knows the order in which they need to be started.
    /// </summary>
    public interface IAddinEntryPoint : IStartable
    {
    }
}
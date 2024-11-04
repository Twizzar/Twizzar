using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.CoreInterfaces;

// ReSharper disable PossibleInterfaceMemberAmbiguity

namespace Twizzar.Design.CoreInterfaces.Command.FixtureItem.Config
{
    /// <summary>
    /// Service for listening to event changes and save the changes to a config class.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3444:Interfaces should not simply inherit from base interfaces with colliding members", Justification = "Works as intended.")]
    public interface IEventWriter :
        IEventListener<FixtureItemMemberChangedEvent>,
        IEventListener<FixtureItemConfigurationStartedEvent>,
        IEventListener<FixtureItemConfigurationEndedEvent>,
        IService
    {
    }
}

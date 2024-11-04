using System.Collections.Generic;
using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using IResult = ViCommon.Functional.Monads.ResultMonad.IResult<ViCommon.Functional.Unit, ViCommon.Functional.Monads.ResultMonad.Failure>;

namespace Twizzar.Design.CoreInterfaces.Command.FixtureItem.Definition
{
    /// <summary>
    /// Definition node which combines the type description with the configuration.
    /// </summary>
    public interface IFixtureItemDefinitionNode : IEntity
    {
        /// <summary>
        /// Gets the fixture item id.
        /// </summary>
        public FixtureItemId FixtureItemId { get; }

        /// <summary>
        /// Gets the type description.
        /// </summary>
        public ITypeDescription TypeDescription { get; }

        /// <summary>
        /// Gets the configuration item.
        /// </summary>
        public IConfigurationItem ConfigurationItem { get; }

        /// <summary>
        /// Replays an event.
        /// </summary>
        /// <param name="e">The event to replay.</param>
        /// <returns>Success if the replay was successful else failure.</returns>
        public IResult Replay(IFixtureItemEvent e);

        /// <summary>
        /// Replays all events in the sequence.
        /// </summary>
        /// <param name="es">The events to replay.</param>
        /// <returns>Success if the replay was successful else failure.</returns>
        public IResult Replay(IEnumerable<IFixtureItemEvent> es);

        /// <summary>
        /// Create this fixture item. A fixture item with this.FixtureItemId should not already exists in the event store.
        /// </summary>
        /// <returns>A task.</returns>
        public Task CreateNamedFixtureItem();

        /// <summary>
        /// Change the member configuration.
        /// </summary>
        /// <param name="memberConfiguration">The new member configuration.</param>
        /// <returns>A task.</returns>
        public Task ChangeMemberConfiguration(IMemberConfiguration memberConfiguration);
    }
}
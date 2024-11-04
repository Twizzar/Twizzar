using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition
{
    /// <summary>
    /// Definition node which contains all information for creating an instance of the corresponding fixture item.
    /// The node only contains the type and his members and not the complete hierarchy.
    /// Is uses links to other definition nodes to describe his grandchildren.
    /// </summary>
    public interface IFixtureItemDefinitionNode : IEntity
    {
        /// <summary>
        /// Gets the type description for this definition node.
        /// </summary>
        public IRuntimeTypeDescription TypeDescription { get; }

        /// <summary>
        /// Gets the fixtureItemId of this definition node.
        /// </summary>
        public FixtureItemId FixtureItemId { get; }

        /// <summary>
        /// Get the creator type.
        /// </summary>
        /// <returns>Enum which creator should be used.</returns>
        public CreatorType GetCreatorType();
    }
}

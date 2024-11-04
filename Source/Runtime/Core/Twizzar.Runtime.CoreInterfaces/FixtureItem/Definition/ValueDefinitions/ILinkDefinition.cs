using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions
{
    /// <summary>
    /// The value is defined in another FixtureItemDefinitionNode.
    /// </summary>
    public interface ILinkDefinition : IValueDefinition
    {
        /// <summary>
        /// Gets the link to another Fixture Item.
        /// </summary>
        public FixtureItemId Link { get; }

        /// <summary>
        /// Gets a new <see cref="ILinkDefinition"/> with a new link.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ILinkDefinition WithLink(FixtureItemId id);
    }
}

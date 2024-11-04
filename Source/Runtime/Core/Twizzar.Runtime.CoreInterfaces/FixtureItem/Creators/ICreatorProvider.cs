using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators
{
    /// <summary>
    /// Service which provides the right <see cref="ICreator"/>.
    /// </summary>
    public interface ICreatorProvider : IService
    {
        /// <summary>
        /// Get the right creator for a <see cref="IFixtureItemDefinitionNode"/>.
        /// </summary>
        /// <param name="fixtureItemDefinition">The fixture item node definition.</param>
        /// <returns>A <see cref="ICreator"/> for creating a instance.</returns>
        public ICreator GetCreator(IFixtureItemDefinitionNode fixtureItemDefinition);
    }
}
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators
{
    /// <summary>
    /// Interface for building up fixtures.
    /// </summary>
    public interface ICreator : IService
    {
        /// <summary>
        /// Creates the fixture instance based on the creator info.
        /// </summary>
        /// <param name="definition">The fixture item definition.</param>
        /// <returns>The created instance.</returns>
        object CreateInstance(IFixtureItemDefinitionNode definition);
    }
}

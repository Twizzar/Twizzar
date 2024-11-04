using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.CoreInterfaces.Command.FixtureItem.Definition
{
    /// <summary>
    /// Factory for creating <see cref="IFixtureItemDefinitionNode"/>.
    /// </summary>
    public interface IFixtureDefinitionNodeFactory : IFactory
    {
        /// <summary>
        /// Create a new <see cref="IFixtureItemDefinitionNode"/>.
        /// </summary>
        /// <param name="id">The fixture item id.</param>
        /// <param name="typeDescription">The type description.</param>
        /// <returns>A new <see cref="IFixtureItemDefinitionNode"/>.</returns>
        IFixtureItemDefinitionNode Create(FixtureItemId id, ITypeDescription typeDescription);
    }
}

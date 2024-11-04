using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators
{
    /// <summary>
    /// Marker Interface for base type creator.
    /// </summary>
    public interface IBaseTypeCreator : ICreator
    {
        /// <summary>
        /// Create an instance for a base type.
        /// </summary>
        /// <param name="valueDefinition">Should be of type <see cref="IRawValueDefinition"/>, <see cref="IUniqueDefinition"/> or <see cref="INullValueDefinition"/>.</param>
        /// <param name="description">The description of the type.</param>
        /// <returns>An new instance.</returns>
        public object CreateInstance(IValueDefinition valueDefinition, IBaseDescription description);
    }
}
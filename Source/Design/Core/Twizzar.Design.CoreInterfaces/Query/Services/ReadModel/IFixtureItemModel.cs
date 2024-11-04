using System.Collections.Immutable;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.FixtureConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.CoreInterfaces.Query.Services.ReadModel
{
    /// <summary>
    /// The fixture item read model.
    /// </summary>
    public interface IFixtureItemModel
    {
        /// <summary>
        /// Gets the fixture item id.
        /// </summary>
        public FixtureItemId Id { get; }

        /// <summary>
        /// Gets the <see cref="IFixtureConfiguration"/> used for general fixture item configurations.
        /// </summary>
        public IImmutableDictionary<string, IFixtureConfiguration> FixtureConfigurations { get; }

        /// <summary>
        /// Gets the type description.
        /// </summary>
        public ITypeDescription Description { get; }
    }
}

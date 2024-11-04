using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration
{
    /// <summary>
    /// Container for adding instances or getting instances know by the fixture.
    /// </summary>
    public interface IRegisteredCodeInstanceContainer : IService
    {
        /// <summary>
        /// Add a new instance.
        /// </summary>
        /// <param name="id">The id of the instance.</param>
        /// <param name="instance">The instance.</param>
        void Add(FixtureItemId id, object instance);

        /// <summary>
        /// Get the instance.
        /// </summary>
        /// <param name="id">The id of the instance.</param>
        /// <returns>The save instance.</returns>
        Maybe<object> Get(FixtureItemId id);
    }
}
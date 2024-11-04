using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Services
{
    /// <summary>
    /// Get an instance cached during the fixture item creation.
    /// </summary>
    public interface IInstanceCacheQuery
    {
        /// <summary>
        /// Get the instance by a specific path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Some when the instance was registered or else None.</returns>
        Maybe<object> GetInstance(string path);
    }
}
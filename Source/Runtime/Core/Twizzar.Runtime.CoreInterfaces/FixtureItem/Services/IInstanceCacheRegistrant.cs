namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Services
{
    /// <summary>
    /// Registrant for register instances used for a fixture item.
    /// </summary>
    public interface IInstanceCacheRegistrant
    {
        /// <summary>
        /// Register an instance.
        /// </summary>
        /// <param name="path">The path to the instances.</param>
        /// <param name="instance">The instance.</param>
        void Register(string path, object instance);
    }
}
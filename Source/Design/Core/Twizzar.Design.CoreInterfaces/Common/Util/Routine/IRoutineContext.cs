using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.Common.Util.Routine
{
    /// <summary>
    /// Context shared for one <see cref="RoutineRunner"/>.
    /// </summary>
    public interface IRoutineContext
    {
        /// <summary>
        /// Get a value by its key.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="key">The Unique key name.</param>
        /// <returns>Some when the key was found else none.</returns>
        Maybe<T> Get<T>(string key);

        /// <summary>
        /// Set a value. This will add a new value if it not already exists else it updates the value.
        /// </summary>
        /// <param name="key">The Unique key name.</param>
        /// <param name="value">The new value to set.</param>
        void Set(string key, object value);
    }
}
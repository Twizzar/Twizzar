using System.Collections.Generic;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.Common.Util.Routine
{
    /// <inheritdoc cref="IRoutineContext" />
    public class RoutineContext : IRoutineContext
    {
        private readonly Dictionary<string, object> _dictionary = new();

        /// <inheritdoc />
        public void Set(string key, object value)
        {
            if (this._dictionary.ContainsKey(key))
            {
                this._dictionary[key] = value;
            }
            else
            {
                this._dictionary.Add(key, value);
            }
        }

        /// <inheritdoc />
        public Maybe<T> Get<T>(string key) =>
            this._dictionary.GetMaybe(key)
                .Map(o => (T)o);
    }
}
using System.Collections.Generic;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Services;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.Core.FixtureItem.Services
{
    /// <summary>
    /// Service which stores instances resolved by a <see cref="ICreator"/>.
    /// </summary>
    public class InstanceCacheRepository : IInstanceCacheRegistrant, IInstanceCacheQuery
    {
        private readonly Dictionary<string, object> _instances = new();

        /// <inheritdoc />
        public Maybe<object> GetInstance(string path) =>
            this._instances.GetMaybe(path);

        /// <inheritdoc />
        public void Register(string path, object instance)
        {
            this._instances.AddOrUpdate(path, instance);
        }
    }
}
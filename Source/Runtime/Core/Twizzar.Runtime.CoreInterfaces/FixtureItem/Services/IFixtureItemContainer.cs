using System.Collections.Generic;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Services
{
    /// <summary>
    /// The test container providing SUT and DOC.
    /// </summary>
    public interface IFixtureItemContainer : IService
    {
        /// <summary>
        /// Gets a concrete instance of type t from a given definition id.
        /// If t is a twizzar base-type the default value of the given type will be returned (with some special cases)
        /// If t is an interface, it will be a test double of Type T.
        /// If t is a class, the type itself will be created via reflection.
        /// </summary>
        /// <param name="config">The item configuration.</param>
        /// <typeparam name="T">The type if the asked instance.</typeparam>
        /// <returns>A resolved instance of type T with given definition.</returns>
        T GetInstance<T>(IItemConfig<T> config);

        /// <summary>
        /// Gets a concrete instance of type t from the default scope.
        /// If t is a twizzar base-type the default value of the given type will be returned (with some special cases)
        /// If t is an interface, it will be a test double of Type T.
        /// If t is a class, the type itself will be created via reflection.
        /// </summary>
        /// <typeparam name="T">The type if the asked instance.</typeparam>
        /// <returns>A resolved instance of type T with default definition.</returns>
        T GetInstance<T>();

        /// <summary>
        /// Gets a list of concrete type t from the default scope.
        /// If t is a twizzar base-type the default value of the given type will be returned (with some special cases)
        /// If t is an interface, it will be a test double of Type T.
        /// If t is a class, the type itself will be created via reflection.
        /// </summary>
        /// <param name="count">The number of instances to return.</param>
        /// <typeparam name="T">The type if the asked instance.</typeparam>
        /// <returns>A resolved instance of type IEnumerable&lt;T&gt; with default definition.</returns>
        IEnumerable<T> GetInstances<T>(int count);

        /// <summary>
        /// Gets a list of concrete type t from a given definition id.
        /// If t is a twizzar base-type the default value of the given type will be returned (with some special cases)
        /// If t is an interface, it will be a test double of Type T.
        /// If t is a class, the type itself will be created via reflection.
        /// </summary>
        /// <param name="count">The number of instances to return.</param>
        /// <param name="config">The item configuration.</param>
        /// <typeparam name="T">The type if the asked instance.</typeparam>
        /// <returns>A resolved instance of type IEnumerable&lt;T&gt; with given definition.</returns>
        IEnumerable<T> GetInstances<T>(int count, IItemConfig<T> config);

        /// <summary>
        /// Sets a concrete instance to the default scope.
        /// When a item which has no definition item set, is requested the set instance is returned.
        /// This is the case on a constructor parameter or a call to <see cref="GetInstance{T}()"/>.
        /// </summary>
        /// <typeparam name="T">The type to configure.</typeparam>
        /// <param name="instance">The instance to set.</param>
        void SetInstance<T>(T instance);

        /// <summary>
        /// Sets a concrete instance to a given definition id.
        /// When a instance of type T and the definition id is requested the given instance is returned.
        /// </summary>
        /// <typeparam name="T">The type to configure.</typeparam>
        /// <param name="instance">The instance to set.</param>
        /// <param name="config">The item configuration.</param>
        void SetInstance<T>(T instance, IItemConfig<T> config);
    }
}

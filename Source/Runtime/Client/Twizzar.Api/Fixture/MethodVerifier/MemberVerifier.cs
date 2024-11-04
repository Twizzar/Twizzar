using System;
using Moq;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Services;
using Twizzar.Runtime.CoreInterfaces.Helpers;
using Twizzar.Runtime.CoreInterfaces.Resources;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Fixture.MethodVerifier
{
    /// <summary>
    /// Baste class for member verification.
    /// </summary>
    /// <typeparam name="TFixtureItem">The fixture item type.</typeparam>
    internal abstract class MemberVerifier<TFixtureItem>
    {
        #region fields

        private readonly IInstanceCacheQuery _instanceCacheQuery;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberVerifier{TFixtureItem}"/> class.
        /// </summary>
        /// <param name="methodPath"></param>
        /// <param name="instanceCacheQuery"></param>
        protected MemberVerifier(IMemberPath<TFixtureItem> methodPath, IInstanceCacheQuery instanceCacheQuery)
        {
            this._instanceCacheQuery = instanceCacheQuery;

            var parentPath = methodPath.Parent.SomeOrProvided(() =>
                throw new ArgumentException("Member path should have a parent"));

            if (!parentPath.ReturnType.IsInterface)
            {
                throw new ArgumentException(
                    $"We can only validate calls on interfaces. The type {parentPath.ReturnType.Name} is not an interface.");
            }

            var parentObject = this.Get(parentPath)
                .SomeOrProvided(() => throw new ArgumentException($"{parentPath} object not found."));

            this.MockType = parentPath.ReturnType;
            this.Mock = GetMock(parentObject, this.MockType);
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the mock.
        /// </summary>
        protected object Mock { get; }

        /// <summary>
        /// Gets the mock inner type.
        /// </summary>
        protected Type MockType { get; }

        #endregion

        #region members

        /// <summary>
        /// Get an dependency over the path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">When no instance was found for the selected path.</exception>
        protected Maybe<object> Get(IMemberPath<TFixtureItem> path) =>
            this._instanceCacheQuery.GetInstance(path.ToString())
                .SomeOrProvided(
                    () => throw new InvalidOperationException(
                        string.Format(
                            ErrorMessagesRuntime.ItemScope_Get_A_instance_for_the_path__0__cannot_be_found_,
                            path)));

        private static object GetMock(object instance, Type type) =>
            ReflectionGenericMethodBuilder.Create(GetMock<object>)
                .WithGenericTypes(type)
                .WithParameters(instance)
                .Invoke();

        private static Mock<T> GetMock<T>(T instance)
            where T : class =>
            Moq.Mock.Get(instance);

        #endregion
    }
}
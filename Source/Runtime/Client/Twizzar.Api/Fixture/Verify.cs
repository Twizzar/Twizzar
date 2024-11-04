using Twizzar.CompositionRoot;
using Twizzar.Fixture.Verifier;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Services;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Fixture
{
    /// <summary>
    /// Static twizzar class for verifying code.
    /// </summary>
    public static class Verify
    {
        #region members

        /// <summary>
        /// Get a constructor verifier for verifying constructor parameters.
        /// </summary>
        /// <typeparam name="T">The type the constructor will return.</typeparam>
        /// <returns>A constructor verifier.</returns>
        public static ICtorVerifier<T> Ctor<T>() =>
                new CtorVerifier<T>(ResolveFixtureItemContainer<T>());

        private static IFixtureItemContainer ResolveFixtureItemContainer<T>()
        {
            var iocOrchestrator = new IocOrchestrator();
            iocOrchestrator.Register<T>(Maybe.None());

            return iocOrchestrator.Resolve<IFixtureItemContainer>();
        }

        #endregion
    }
}
using System;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Interfaces
{
    /// <summary>
    /// Does everything containing IoC.
    /// </summary>
    internal interface IIocOrchestrator : IHasEnsureHelper, IHasLogger, IDisposable
    {
        /// <summary>
        /// Registers everything.
        /// </summary>
        /// <typeparam name="TFixtureItemType">The fixture item type.</typeparam>
        /// <param name="itemConfig">The item config.</param>
        void Register<TFixtureItemType>(Maybe<IItemConfig<TFixtureItemType>> itemConfig);

        /// <summary>
        /// Resolves a Interface of T to generate a member of the registered base type.
        /// </summary>
        /// <typeparam name="T">The type of the instance that should be generated.</typeparam>
        /// <returns>A instance of the resolved type T.</returns>
        T Resolve<T>();
    }
}
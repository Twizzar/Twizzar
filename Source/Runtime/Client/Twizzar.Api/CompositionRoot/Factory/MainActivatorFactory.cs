using System;
using Autofac.Core;
using JetBrains.Annotations;
using Twizzar.Runtime.Infrastructure.AutofacServices.Activator;
using Twizzar.Runtime.Infrastructure.AutofacServices.Factory;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.CompositionRoot.Factory
{
    /// <summary>
    /// Implements the <see cref="IMainActivatorFactory"/> interface.
    /// Factory for creating the autofac MainActivator instances.
    /// </summary>
    internal class MainActivatorFactory : IMainActivatorFactory
    {
        private readonly FactoryDelegate _factoryDelegate;

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="MainActivatorFactory"/> class.
        /// </summary>
        /// <param name="factoryDelegate">The autofac component context.</param>
        public MainActivatorFactory(FactoryDelegate factoryDelegate)
        {
            this._factoryDelegate = factoryDelegate;
        }
        #endregion

#pragma warning disable SA1600 // Elements should be documented
        public delegate MainActivator FactoryDelegate(Type type, Maybe<string> definitionId);
#pragma warning restore SA1600 // Elements should be documented

        #region Implementation of IMainActivatorFactory

        /// <inheritdoc />
        public IInstanceActivator Create(Type type, [CanBeNull] string scope) =>
            this._factoryDelegate(type, Maybe.ToMaybe(scope));

        #endregion
    }
}

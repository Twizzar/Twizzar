using System;
using Autofac;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.Services
{
    /// <inheritdoc cref="IScopedServiceProvider" />
    public sealed class ScopedServiceProvider : IScopedServiceProvider, IService
    {
        #region fields

        private readonly ILifetimeScope _lifetimeScope;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedServiceProvider"/> class.
        /// </summary>
        /// <param name="lifetimeScope"></param>
        public ScopedServiceProvider(ILifetimeScope lifetimeScope)
        {
            this.EnsureParameter(lifetimeScope, nameof(lifetimeScope)).ThrowWhenNull();

            this._lifetimeScope = lifetimeScope;
        }

        #endregion

        #region properties

        /// <inheritdoc/>
        public ILogger Logger { get; set; }

        /// <inheritdoc/>
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc/>
        public void Dispose()
        {
            this._lifetimeScope.Dispose();
        }

        /// <inheritdoc />
        public Maybe<T> GetService<T>()
            where T : class
        {
            try
            {
                return this._lifetimeScope.TryResolve<T>(out var service)
                    ? service
                    : Maybe.None<T>();
            }
            catch (ObjectDisposedException)
            {
                return Maybe.None<T>();
            }
        }

        #endregion
    }
}
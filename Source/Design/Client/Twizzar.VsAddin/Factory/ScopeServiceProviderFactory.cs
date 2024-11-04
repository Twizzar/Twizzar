using System.Diagnostics.CodeAnalysis;
using Autofac;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.VsAddin.Factory
{
    /// <inheritdoc cref="IScopeServiceProviderFactory" />
    [ExcludeFromCodeCoverage]
    public class ScopeServiceProviderFactory : IScopeServiceProviderFactory, IFactory
    {
        #region fields

        private readonly ILifetimeScope _lifetimeScope;
        private readonly Factory _factory;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeServiceProviderFactory"/> class.
        /// </summary>
        /// <param name="lifetimeScope"></param>
        /// <param name="factory"></param>
        public ScopeServiceProviderFactory(ILifetimeScope lifetimeScope, Factory factory)
        {
            this.EnsureMany()
                .Parameter(lifetimeScope, nameof(lifetimeScope))
                .Parameter(factory, nameof(factory))
                .ThrowWhenNull();

            this._lifetimeScope = lifetimeScope;
            this._factory = factory;
        }

        #endregion

        #region delegates

        /// <summary>
        /// Delegate for autofac.
        /// </summary>
        /// <param name="lifetimeScope"></param>
        /// <returns>A new <see cref="IScopedServiceProvider"/>.</returns>
        public delegate IScopedServiceProvider Factory(ILifetimeScope lifetimeScope);

        #endregion

        #region properties

        /// <inheritdoc/>
        public ILogger Logger { get; set; }

        /// <inheritdoc/>
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc/>
        public IScopedServiceProvider CreateNew() =>
            this._factory(this._lifetimeScope.BeginLifetimeScope());

        #endregion
    }
}
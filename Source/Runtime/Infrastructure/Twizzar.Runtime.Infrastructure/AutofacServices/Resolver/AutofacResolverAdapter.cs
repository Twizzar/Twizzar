using System;
using System.Diagnostics.CodeAnalysis;
using Autofac;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.SharedKernel.Infrastructure.Factory;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using IContainer = Autofac.IContainer;

namespace Twizzar.Runtime.Infrastructure.AutofacServices.Resolver
{
    /// <summary>
    /// Implementation of the <see cref="IResolver"/> for the autofac IContainer.
    /// Adapts the IContainer Extension Method to the <see cref="IResolver"/> interface.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AutofacResolverAdapter : IResolver
    {
        private readonly IContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacResolverAdapter"/> class.
        /// </summary>
        /// <param name="autofacContainerFactory">The autofac container factory to create a new instance of <see cref="IContainer"/>.</param>
        public AutofacResolverAdapter(IAutofacContainerFactory autofacContainerFactory)
        {
            this.EnsureParameter(autofacContainerFactory, nameof(autofacContainerFactory))
                .ThrowWhenNull();
            this._container = autofacContainerFactory.Create();
        }

        #region Implementation of IHasLogger and IHasEnsureHelper

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region implementation of IResolver

        /// <inheritdoc />
        public T Resolve<T>() =>
            ConvertNullValue<T>(this._container.Resolve(typeof(T)));

        /// <inheritdoc />
        public object Resolve(Type type) =>
            ConvertNullValue(this._container.Resolve(type));

        /// <inheritdoc />
        public T ResolveNamed<T>(string typeName)
        {
            this.EnsureParameter(typeName, nameof(typeName))
                .IsNotNullAndNotEmpty()
                .ThrowOnFailure();

            var instance = this._container.ResolveNamed(typeName, typeof(T));
            return ConvertNullValue<T>(instance);
        }

        private static T ConvertNullValue<T>(object instance) =>
            (instance is INullValue marker)
                ? marker.Convert<T>()
                : (T)instance;

        private static object ConvertNullValue(object instance) =>
            (instance is INullValue marker)
                ? marker.Convert<object>()
                : instance;

        #endregion
    }
}

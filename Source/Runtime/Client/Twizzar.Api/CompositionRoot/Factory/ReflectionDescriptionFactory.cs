using System;
using Autofac;
using Twizzar.Runtime.Core.FixtureItem.Description;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.SharedKernel.Factories;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.CompositionRoot.Factory
{
    /// <inheritdoc cref="IReflectionDescriptionFactory"/>
    internal sealed class ReflectionDescriptionFactory : FactoryBase, IReflectionDescriptionFactory
    {
        #region fields

        private readonly Factory _factory;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionDescriptionFactory"/> class.
        /// </summary>
        /// <param name="componentContext"></param>
        /// <param name="factory"></param>
        public ReflectionDescriptionFactory(IComponentContext componentContext, Factory factory)
            : base(componentContext)
        {
            this.EnsureParameter(factory, nameof(factory)).ThrowWhenNull();

            this._factory = factory;
        }

        #endregion

        #region delegates

        /// <summary>
        /// Autofac factory.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>A new instance of <see cref="IRuntimeTypeDescription"/>.</returns>
        internal delegate ReflectionTypeDescription Factory(Type type);

        #endregion

        #region members

        /// <inheritdoc />
        public IRuntimeTypeDescription Create(Type type) => this._factory(type);

        #endregion
    }
}
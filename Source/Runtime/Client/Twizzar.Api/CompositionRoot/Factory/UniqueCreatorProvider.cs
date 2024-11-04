using System;
using System.Data;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Name;
using Twizzar.Runtime.CoreInterfaces.Resources;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.CompositionRoot.Factory
{
    /// <summary>
    /// Class which implements <see cref="IUniqueCreatorProvider"/> interface.
    /// </summary>
    internal class UniqueCreatorProvider : IUniqueCreatorProvider
    {
        private readonly IBaseTypeService _baseTypeService;
        private readonly ILifetimeScope _lifetimeScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueCreatorProvider"/> class.
        /// </summary>
        /// <param name="baseTypeService">The base type service.</param>
        /// <param name="lifetimeScope">The ioc lifetime scope.</param>
        public UniqueCreatorProvider(IBaseTypeService baseTypeService, ILifetimeScope lifetimeScope)
        {
            this.EnsureMany()
                .Parameter(baseTypeService, nameof(baseTypeService))
                .Parameter(lifetimeScope, nameof(lifetimeScope))
                .ThrowWhenNull();

            this._baseTypeService = baseTypeService;
            this._lifetimeScope = lifetimeScope;
        }

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region Implementation of IUniqueCreatorProvider

        /// <inheritdoc />
        public IUniqueCreator<T> GetUniqueCreator<T>()
        {
            var type = typeof(T);

            if (type.IsEnum)
            {
                return this._lifetimeScope.Resolve<IUniqueEnumCreator<T>>();
            }

            if (!this._baseTypeService.IsBaseType(type.ToTypeFullName()))
            {
                this.Logger?.Log(LogLevel.Error, ErrorMessagesRuntime.BaseTypesService_AskedTypeIsNotSupported);
                throw new InvalidConstraintException(ErrorMessagesRuntime.BaseTypesService_AskedTypeIsNotSupported);
            }

            try
            {
                return this._lifetimeScope.Resolve<IUniqueCreator<T>>();
            }
            catch (ComponentNotRegisteredException)
            {
                throw new NotSupportedException(ErrorMessagesRuntime.BaseTypesService_AskedTypeIsNotSupported);
            }
            catch (DependencyResolutionException)
            {
                throw new NotSupportedException(ErrorMessagesRuntime.BaseTypesService_AskedTypeIsNotSupported);
            }
        }

        #endregion
    }
}

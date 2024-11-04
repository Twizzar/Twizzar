using System;
using System.Runtime.CompilerServices;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Twizzar.CompositionRoot.Factory;
using Twizzar.Fixture.Verifier;
using Twizzar.Interfaces;
using Twizzar.Runtime.Core.FixtureItem.Configuration;
using Twizzar.Runtime.Core.FixtureItem.Configuration.Services;
using Twizzar.Runtime.Core.FixtureItem.Creators;
using Twizzar.Runtime.Core.FixtureItem.Creators.BaseTypeUniqueCreators;
using Twizzar.Runtime.Core.FixtureItem.Creators.BaseTypeUniqueCreators.BitSequenceBased;
using Twizzar.Runtime.Core.FixtureItem.Creators.BaseTypeUniqueCreators.FloatingPointNumbers;
using Twizzar.Runtime.Core.FixtureItem.Definition;
using Twizzar.Runtime.Core.FixtureItem.Definition.MemberDefinitions;
using Twizzar.Runtime.Core.FixtureItem.Definition.Services;
using Twizzar.Runtime.Core.FixtureItem.Description;
using Twizzar.Runtime.Core.FixtureItem.Services;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.MemberDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.Services;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Services;
using Twizzar.Runtime.Infrastructure.ApplicationService;
using Twizzar.Runtime.Infrastructure.AutofacServices.Activator;
using Twizzar.Runtime.Infrastructure.AutofacServices.Factory;
using Twizzar.Runtime.Infrastructure.AutofacServices.Registration;
using Twizzar.Runtime.Infrastructure.AutofacServices.Resolver;
using Twizzar.Runtime.Infrastructure.DomainService;
using Twizzar.SharedKernel.Core.FixtureItem.Configuration;
using Twizzar.SharedKernel.Core.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.Core.FixtureItem.Description.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.SharedKernel.Factories;
using Twizzar.SharedKernel.Infrastructure.Factory;
using Twizzar.SharedKernel.Infrastructure.IocInitializer;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using IRegistrationSource = Autofac.Core.IRegistrationSource;

[assembly: InternalsVisibleTo("Twizzar.Runtime.Test")]
[assembly: InternalsVisibleTo("Twizzar.SharedKernel.Test")]

namespace Twizzar.CompositionRoot
{
    /// <summary>
    /// Does everything containing IoC.
    /// </summary>
    internal class IocOrchestrator : IIocOrchestrator
    {
        #region fields

        /// <summary>
        /// The IoC container of the API.
        /// Containing all the dependencies of the components.
        /// </summary>
        private Autofac.IContainer _container;

        private bool _isDisposed;
        #endregion

        #region Properties

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region public methods

        /// <inheritdoc/>
        public void Register<TFixtureItemType>(Maybe<IItemConfig<TFixtureItemType>> itemConfig)
        {
            if (this._container != null)
            {
                return;
            }

            var builder = new ContainerBuilder();
            LoggerInitializer.Init(builder, @"\vi-sit\twizzar\twizzarApi.log", "Api");
            EnsureHelperInitializer.Init(builder);

            RegisterServices(builder);
            RegisterAutofacServices(builder);
            RegisterCreators(builder);
            RegisterUniqueCreators(builder);
            RegisterAggregates(builder);

            builder
                .Register(c =>
                    new UserConfigurationQuery<TFixtureItemType>(itemConfig, c.Resolve<IConfigurationItemFactory>()))
                .As<IUserConfigurationQuery>();

            this._container = builder.Build();
        }

        /// <summary>
        /// Resolves a Interface of T to generate a member of the registered base type.
        /// </summary>
        /// <typeparam name="T">The type of the instance that should be generated.</typeparam>
        /// <returns>A instance of the resolved type T.</returns>
        public T Resolve<T>()
        {
            this.EnsureParameter(this._isDisposed, nameof(this._isDisposed))
                .IsFalse(b => b, paramName => new ObjectDisposedException(paramName))
                .ThrowOnFailure();

            this.EnsureParameter(this._container, nameof(this._container))
                .IsNotNull(_ => new Exception("Could not resolve dependency"))
                .ThrowOnFailure();

            return this._container.Resolve<T>();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes elements of this class.
        /// Mainly the IoC _container and its components.
        /// </summary>
        /// <param name="disposing">Bool whether this class should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || this._isDisposed)
            {
                return;
            }

            this._isDisposed = true;
            this._container.Disposer.Dispose();
            this._container.Dispose();
        }

        #endregion

        #region pivate methods

        /// <summary>
        /// Registers all adapters to their ports.
        /// </summary>
        /// <param name="builder">A <see cref="ContainerBuilder"/> to register the adapter modules on.</param>
        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<SharedKernel.Core.FixtureItem.Configuration.Services.CtorSelector>()
                .As<SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services.ICtorSelector>();

            builder.RegisterType<CreatorProvider>()
                .As<ICreatorProvider>()
                .SingleInstance();

            builder.RegisterType<BaseTypeService>()
                .As<IBaseTypeService>()
                .SingleInstance();

            builder.RegisterType<FixtureItemDefinitionNodeCreationService>()
                .As<IFixtureItemDefinitionNodeCreationService>()
                .SingleInstance();

            builder.RegisterType<FixtureDefinitionFactory>()
                .As<IFixtureDefinitionFactory>()
                .SingleInstance();

            builder.RegisterType<FixtureItemDefinitionQuery>()
                .As<IFixtureItemDefinitionQuery>()
                .SingleInstance();

            builder.RegisterType<UniqueCreatorProvider>()
                .As<IUniqueCreatorProvider>()
                .SingleInstance();

            builder.RegisterType<BaseTypeUniqueCreator>()
                .As<IBaseTypeUniqueCreator>()
                .SingleInstance();

            builder.RegisterType<FixtureItemContainer>()
                .As<IFixtureItemContainer>()
                .SingleInstance();

            builder.RegisterType<ReflectionDescriptionFactory>()
                .As<IReflectionDescriptionFactory>()
                .SingleInstance();

            builder.RegisterType<ConfigurationItemFactory>()
                .As<IConfigurationItemFactory>()
                .SingleInstance();

            builder.RegisterType<ConfigurationItemQuery>()
                .As<IConfigurationItemQuery>()
                .SingleInstance();

            builder.RegisterType<SystemDefaultService>()
                .As<ISystemDefaultService>()
                .SingleInstance();

            builder.RegisterType<ReflectionTypeDescription>();

            builder.RegisterType<ReflectionTypeDescriptionProvider>()
                .As<ITypeDescriptionQuery>()
                .SingleInstance();

            builder.RegisterType<InstanceCacheRepository>()
                .As<IInstanceCacheRegistrant>()
                .As<IInstanceCacheQuery>()
                .SingleInstance();
        }

        private static void RegisterAggregates(ContainerBuilder builder)
        {
            builder.RegisterType<ClassNode>().As<IClassNode>();
            builder.RegisterType<BaseTypeNode>().As<IBaseTypeNode>();
            builder.RegisterType<MockNode>().As<IMockNode>();
            builder.RegisterType<PropertyDefinition>().As<IPropertyDefinition>();
            builder.RegisterType<FieldDefinition>().As<IFieldDefinition>();
            builder.RegisterType<ParameterDefinition>().As<IParameterDefinition>();
            builder.RegisterType<MethodDefinition>().As<IMethodDefinition>();
            builder.RegisterType<ConfigurationItem>().As<IConfigurationItem>();
        }

        private static void RegisterCreators(ContainerBuilder builder)
        {
            builder.RegisterType<BaseTypeCreator>()
                .As<IBaseTypeCreator>()
                .SingleInstance();

            builder.RegisterType<MoqCreator>()
                .As<IMoqCreator>()
                .SingleInstance();

            builder.RegisterType<ConcreteTypeCreator>()
                .As<IConcreteTypeCreator>()
                .SingleInstance();
        }

        private static void RegisterAutofacServices(ContainerBuilder builder)
        {
            builder.RegisterType<RegistrationSource>()
                .As<IRegistrationSource>()
                .SingleInstance();

            builder.RegisterType<ComponentRegistrationFactory>()
                .As<IComponentRegistrationFactory>()
                .SingleInstance();

            builder.RegisterType<MainActivatorFactory>()
                .As<IMainActivatorFactory>()
                .SingleInstance();

            builder.RegisterType<MainActivator>()
                .AsSelf();

            builder.RegisterType<AutofacContainerFactory>()
                .As<IAutofacContainerFactory>()
                .SingleInstance();

            builder.RegisterType<AutofacResolverAdapter>()
                .As<IResolver>()
                .SingleInstance();

            builder.RegisterType<ComponentRegistration>()
                .As<IComponentRegistration>();
        }

        private static void RegisterUniqueCreators(ContainerBuilder builder)
        {
            builder.RegisterType<IntUniqueCreator>()
                .As<IUniqueCreator<int>>()
                .SingleInstance();

            builder.RegisterType<UintUniqueCreator>()
                .As<IUniqueCreator<uint>>()
                .SingleInstance();

            builder.RegisterType<LongUniqueCreator>()
                .As<IUniqueCreator<long>>()
                .SingleInstance();

            builder.RegisterType<UlongUniqueCreator>()
                .As<IUniqueCreator<ulong>>()
                .SingleInstance();

            builder.RegisterType<ShortUniqueCreator>()
                .As<IUniqueCreator<short>>()
                .SingleInstance();

            builder.RegisterType<UshortUniqueCreator>()
                .As<IUniqueCreator<ushort>>()
                .SingleInstance();

            builder.RegisterType<ByteUniqueCreator>()
                .As<IUniqueCreator<byte>>()
                .SingleInstance();

            builder.RegisterType<SbyteUniqueCreator>()
                .As<IUniqueCreator<sbyte>>()
                .SingleInstance();

            builder.RegisterType<DecimalUniqueCreator>()
                .As<IUniqueCreator<decimal>>()
                .SingleInstance();

            builder.RegisterType<FloatUniqueCreator>()
                .As<IUniqueCreator<float>>()
                .SingleInstance();

            builder.RegisterType<DoubleUniqueCreator>()
                .As<IUniqueCreator<double>>()
                .SingleInstance();

            builder.RegisterType<CharUniqueCreator>()
                .As<IUniqueCreator<char>>()
                .SingleInstance();

            builder.RegisterType<BoolCreator>()
                .As<IUniqueCreator<bool>>()
                .SingleInstance();

            builder.RegisterType<StringUniqueCreator>()
                .As<IUniqueCreator<string>>()
                .SingleInstance();

            builder.RegisterGeneric(typeof(UniqueEnumCreator<>))
                .As(typeof(IUniqueEnumCreator<>))
                .SingleInstance();

            builder.RegisterType<RegisteredCodeInstanceContainer>()
                .As<IRegisteredCodeInstanceContainer>()
                .SingleInstance();

            builder.RegisterGeneric(typeof(CtorVerifier<>))
                .As(typeof(ICtorVerifier<>));
        }

        #endregion
    }
}

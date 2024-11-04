using System;
using Autofac;
using Twizzar.Analyzer.Core.CompositionRoot.Factory;
using Twizzar.Analyzer.Core.Interfaces;
using Twizzar.Analyzer.Core.SourceTextGenerators;
using Twizzar.Analyzer.SourceTextGenerators;
using Twizzar.Analyzer2022.App.Interfaces;
using Twizzar.Analyzer2022.App.SourceTextGenerators;
using Twizzar.Design.Shared.Infrastructure.Description;
using Twizzar.Design.Shared.Infrastructure.Discovery;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.SharedKernel.Core.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.Core.FixtureItem.Description.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.SharedKernel.Infrastructure.IocInitializer;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Analyzer.Core.CompositionRoot
{
    /// <summary>
    /// Does everything containing IoC.
    /// </summary>
    public class IocOrchestrator : IIocOrchestrator
    {
        #region fields

        /// <summary>
        /// The IoC container of the API.
        /// Containing all the dependencies of the components.
        /// </summary>
        private Autofac.IContainer _container;

        private bool _isDisposed;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="IocOrchestrator"/> class.
        /// </summary>
        public IocOrchestrator()
        {
            this.Register();
        }

        #region Properties

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region public methods

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

        private void Register()
        {
            if (this._container != null)
            {
                return;
            }

            var builder = new ContainerBuilder();

            LoggerInitializer.Init(builder, @"\vi-sit\twizzar\twizzarAnalyzer.log", "Analyzer");
            EnsureHelperInitializer.Init(builder);

            RegisterServices(builder);

            this._container = builder.Build();
        }

        /// <summary>
        /// Registers all adapters to their ports.
        /// </summary>
        /// <param name="builder">A <see cref="ContainerBuilder"/> to register the adapter modules on.</param>
        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<BaseTypeService>()
                .As<IBaseTypeService>()
                .SingleInstance();

            builder.RegisterType<RoslynDescriptionFactory>()
                .As<IRoslynDescriptionFactory>()
                .SingleInstance();

            builder.RegisterType<RoslynTypeDescription>()
                .As<ITypeDescription>();

            builder.RegisterType<PathSourceTextGenerator>()
                .As<IPathSourceTextGenerator>()
                .SingleInstance();

            builder.RegisterType<PathSourceTextMemberGenerator>()
                .As<IPathSourceTextMemberGenerator>()
                .SingleInstance();

            builder.RegisterType<BuilderExtensionMethodSourceTextGenerator>()
                .As<IBuilderExtensionMethodSourceTextGenerator>()
                .SingleInstance();

            builder.RegisterType<CtorSelector>()
                .As<ICtorSelector>()
                .SingleInstance();

            builder.RegisterType<SourceCodeCache>()
                .As<ISourceCodeCache>()
                .SingleInstance();

            builder.RegisterType<Discoverer>()
                .As<IDiscoverer>()
                .SingleInstance();

            builder.RegisterType<SymbolService>()
                .As<ISymbolService>()
                .SingleInstance();
        }

        #endregion
    }
}

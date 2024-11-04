using System.Diagnostics.CodeAnalysis;
using Autofac;
using Twizzar.Design.Core.Query.Services;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.FixtureItem.Config;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.CoreInterfaces.Common.FixtureItem.Config;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Infrastructure.Command.FixtureItem.Config;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn.ConfigWriter;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn.DocumentReader;
using Twizzar.Design.Infrastructure.VisualStudio.Services;
using Twizzar.Design.Shared.Infrastructure.Discovery;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.SharedKernel.Core.FixtureItem.Configuration;
using Twizzar.SharedKernel.Core.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.Factories;
using Twizzar.VsAddin.Factory;
using Twizzar.VsAddin.Interfaces.CompositionRoot;

namespace Twizzar.VsAddin.CompositionRoot.Registrant
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class ConfigurationRegistrant : IIocComponentRegistrant
    {
        #region members

        /// <inheritdoc />
        public void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterType<CtorSelector>()
                .As<ICtorSelector>()
                .SingleInstance();

            builder.RegisterType<ConfigurationItemQuery>()
                .As<IConfigurationItemQuery>()
                .SingleInstance();

            builder.RegisterType<SystemDefaultService>()
                .As<ISystemDefaultService>()
                .SingleInstance();

            builder.RegisterType<ConfigurationItemFactory>()
                .As<IConfigurationItemFactory>()
                .SingleInstance();

            builder.RegisterType<RoslynConfigReader>()
                .As<IUserConfigurationQuery>()
                .SingleInstance();

            builder.RegisterType<RoslynConfigEventWriter>()
                .As<IEventListener<FixtureItemMemberChangedEvent>>()
                .As<IEventListener<FixtureItemConfigurationStartedEvent>>()
                .As<IEventListener<FixtureItemConfigurationEndedEvent>>()
                .As<IEventWriter>()
                .SingleInstance();

            builder.RegisterType<RoslynConfigWriter>()
                .As<IRoslynConfigWriter>()
                .SingleInstance();

            builder.RegisterType<ItemBuilderFinderFactory>()
                .As<IItemBuilderFinderFactory>()
                .SingleInstance();

            builder.RegisterType<ItemBuilderFinder>()
                .As<IItemBuilderFinder>();

            builder.RegisterType<FileWriter>()
                .As<IFileWriter>()
                .SingleInstance();

            builder.RegisterType<FilePathProvider>()
                .As<IFilePathProvider>()
                .As<IViProjectFilePathProvider>()
                .SingleInstance();

            builder.RegisterType<ConfigurationItemCacheQuery>()
                .As<IConfigurationItemCacheQuery>()
                .As<IEventStoreToQueryCacheSynchronizer>()
                .As<IEventListener<FixtureItemConfigurationEndedEvent>>()
                .SingleInstance();

            builder.RegisterType<ConfigurationItem>().As<IConfigurationItem>();

            builder.RegisterType<VsProjectManager>()
                .As<IVsProjectManager>()
                .SingleInstance();

            builder.RegisterType<RoslynContextQuery>()
                .As<IRoslynContextQuery>()
                .SingleInstance();

            builder.RegisterType<RoslynConfigurationItemReader>()
                .As<IRoslynConfigurationItemReader>()
                .SingleInstance();

            builder.RegisterType<RoslynConfigFinder>()
                .As<IRoslynConfigFinder>()
                .SingleInstance();

            builder.RegisterType<Discoverer>()
                .As<IDiscoverer>()
                .SingleInstance();

            builder.RegisterType<RoslynMemberConfigurationFinder>()
                .As<IRoslynMemberConfigurationFinder>()
                .SingleInstance();

            builder.RegisterType<RoslynBuildInvocationQuery>()
                .As<IBuildInvocationQuery>()
                .SingleInstance();
        }

        #endregion
    }
}
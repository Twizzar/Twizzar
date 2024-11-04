using System.Diagnostics.CodeAnalysis;
using Autofac;
using Twizzar.Design.Core.Query.Services;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.CoreInterfaces.Common.Roslyn;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Infrastructure.Query;
using Twizzar.Design.Infrastructure.Services;
using Twizzar.Design.Infrastructure.VisualStudio.Common.Util;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.Design.Infrastructure.VisualStudio.Services;
using Twizzar.Design.Infrastructure.VisualStudio2019.Interfaces.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio2019.Roslyn;
using Twizzar.VsAddin.Factory;
using Twizzar.VsAddin.Interfaces.CompositionRoot;

namespace Twizzar.VsAddin.CompositionRoot.Registrant
{
    /// <summary>
    /// Registrant for util classes.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UtilRegistrant : IIocComponentRegistrant
    {
        /// <inheritdoc />
        public void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterType<AddinVersionQuery>()
                .As<IAddinVersionQuery>()
                .SingleInstance();

            builder.RegisterType<VersionChecker>()
                .As<IVersionChecker>()
                .SingleInstance();

            builder.RegisterType<ScopedServiceProvider>()
                .As<IScopedServiceProvider>();

            builder.RegisterType<ScopeServiceProviderFactory>()
                .As<IScopeServiceProviderFactory>()
                .SingleInstance();

            builder.RegisterType<FileService>()
                .As<IFileService>()
                .SingleInstance();

            builder.RegisterType<SettingsFileService>()
                .As<ISettingsWriter, ISettingsQuery>()
                .SingleInstance();

            builder.RegisterType<CompilationTypeQueryFactory>()
                .As<ICompilationTypeQueryFactory>()
                .SingleInstance();

            builder.RegisterType<CompilationTypeCache>()
                .As<ICompilationTypeCache>()
                .SingleInstance();

            builder.RegisterType<CompilationQuery>()
                .As<ICompilationQuery>()
                .SingleInstance();

            builder.RegisterType<ShortTypesConverter>()
                .As<IShortTypesConverter>()
                .SingleInstance();

            builder.RegisterType<ShortcutService>()
                .As<IShortcutService>()
                .SingleInstance();
        }
    }
}
using System.Diagnostics.CodeAnalysis;
using Autofac;
using TestCreation.ProjectCreation;
using TestCreation.Services;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.Design.CoreInterfaces.TestCreation.BaseConfig;
using Twizzar.Design.CoreInterfaces.TestCreation.Config;
using Twizzar.Design.CoreInterfaces.TestCreation.Mapping;
using Twizzar.Design.CoreInterfaces.TestCreation.ProjectCreation;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.Design.Infrastructure.VisualStudio.Services;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.BaseConfig;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.DocumentContent;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Mapping;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;
using Twizzar.VsAddin.Interfaces.CompositionRoot;

namespace Twizzar.VsAddin.CompositionRoot.Registrant;

/// <summary>
/// Registrant for register services used for the unit test creation.
/// </summary>
[ExcludeFromCodeCoverage]
public class TestCreationRegistrant : IIocComponentRegistrant
{
    #region members

    /// <inheritdoc />
    public void RegisterComponents(ContainerBuilder builder)
    {
        builder.RegisterType<DocumentContentCreationService>()
            .As<IDocumentContentCreationService>()
            .SingleInstance();
        builder.RegisterType<DocumentQuery>()
            .As<IDocumentQuery>()
            .SingleInstance();
        builder.RegisterType<LocationService>()
            .As<ILocationService>()
            .SingleInstance();
        builder.RegisterType<MappingService>()
            .As<IMappingService>()
            .SingleInstance();
        builder.RegisterType<TemplateService>()
            .As<ITemplateService>()
            .SingleInstance();
        builder.RegisterType<TestCreationProgressFactory>()
            .As<ITestCreationProgressFactory>()
            .SingleInstance();
        builder.RegisterType<UserFeedbackService>()
            .As<IUserFeedbackService>()
            .SingleInstance();

        // Project Query
        builder.RegisterType<ProjectQuery>()
            .As<IProjectQuery>()
            .SingleInstance();

        builder.RegisterType<VsProjectQuery>()
            .As<IVsProjectQuery>()
            .SingleInstance();

        builder.RegisterType<VsProjectFactory>()
            .As<IVsProjectFactory>()
            .SingleInstance();

        builder.RegisterType<TargetFrameworkMonikerQuery>()
            .As<ITargetFrameworkMonikerQuery>()
            .SingleInstance();

        // Project Query
        builder.RegisterType<ProjectQuery>()
            .As<IProjectQuery>()
            .SingleInstance();

        builder.RegisterType<VsProjectQuery>()
            .As<IVsProjectQuery>()
            .SingleInstance();

        builder.RegisterType<VsProjectFactory>()
            .As<IVsProjectFactory>()
            .SingleInstance();

        builder.RegisterType<TargetFrameworkMonikerQuery>()
            .As<ITargetFrameworkMonikerQuery>()
            .SingleInstance();

        // base config
        builder.RegisterType<BaseConfigParser>()
            .As<IBaseConfigParser>()
            .SingleInstance();

        builder.RegisterType<BaseConfigQuery>()
            .As<IBaseConfigQuery>()
            .SingleInstance();

        builder.RegisterType<ConfigFileService>()
            .As<IConfigFileService>()
            .SingleInstance();

        // Mapping
        builder.RegisterType<ConfigQuery>()
            .As<IConfigQuery>()
            .SingleInstance();

        builder.RegisterType<MappingReplacementService>()
            .As<IMappingReplacementService>()
            .SingleInstance();

        builder.RegisterType<WildcardPatternMatcher>()
            .As<IWildcardPatternMatcher>()
            .SingleInstance();

        // Template
        builder.RegisterType<TemplateFileQuery>()
            .As<ITemplateFileQuery>()
            .SingleInstance();

        builder.RegisterType<SnippetNodeFactory>()
            .As<ISnippetNodeFactory>()
            .SingleInstance();

        builder.RegisterType<TemplateCodeProvider>()
            .As<ITemplateCodeProvider>()
            .SingleInstance();

        builder.RegisterType<TemplateSnippetFactory>()
            .As<ITemplateSnippetFactory>()
            .SingleInstance();

        builder.RegisterType<TemplateFileService>()
            .As<ITemplateFileService>()
            .SingleInstance();

        builder.RegisterType<VariableReplacer>()
            .As<IVariableReplacer>()
            .SingleInstance();

        builder.RegisterType<VariablesProviderFactory>()
            .As<IVariablesProviderFactory>()
            .SingleInstance();

        builder.RegisterType<TemplateVisitor>()
            .As<ITemplateVisitor>()
            .SingleInstance();

        builder.RegisterType<TemplateFileService>()
            .As<ITemplateFileService>()
            .SingleInstance();

        // Document Content creation
        builder.RegisterType<UpdateUsingsService>()
            .As<IUpdateUsingService>()
            .SingleInstance();

        // Navigation
        builder.RegisterType<NavigationService>()
            .As<INavigationService>()
            .SingleInstance();
    }

    #endregion
}
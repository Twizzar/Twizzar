using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

using Autofac;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Infrastructure.VisualStudio;
using Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Services;
using Twizzar.Design.Infrastructure.VisualStudio.Services.VsEvents;
using Twizzar.Design.Infrastructure.VisualStudio.Ui;
using Twizzar.Design.Infrastructure.VisualStudio.Ui.View.About;
using Twizzar.Design.Infrastructure.VisualStudio2019.Roslyn;
using Twizzar.Design.Ui.Interfaces.Factories;
using Twizzar.Design.Ui.Interfaces.Queries;
using Twizzar.Design.Ui.Interfaces.Services;
using Twizzar.Design.Ui.Interfaces.Validator;
using Twizzar.Design.Ui.Interfaces.View;
using Twizzar.Design.Ui.Interfaces.ViewModels;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.Interfaces.VisualStudio;
using Twizzar.Design.Ui.Messaging;
using Twizzar.Design.Ui.Queries;
using Twizzar.Design.Ui.Service;
using Twizzar.Design.Ui.Validator;
using Twizzar.Design.Ui.View;
using Twizzar.Design.Ui.View.RichTextBox;
using Twizzar.Design.Ui.ViewModels.FixtureItem;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.Status;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Value;
using Twizzar.VsAddin.CompositionRoot.Autofac;
using Twizzar.VsAddin.Factory;
using Twizzar.VsAddin.Interfaces.CompositionRoot;

namespace Twizzar.VsAddin.CompositionRoot.Registrant
{
    /// <summary>
    /// Registrant for ui elements.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UiElementsRegistrant : IIocComponentRegistrant
    {
        /// <inheritdoc />
        public void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterType<UiElementsFactory>()
                .As<IUiElementsFactory>()
                .SingleInstance();

            builder.RegisterType<FixtureItemViewModel>()
                .As<IFixtureItemViewModel>();

            builder.RegisterType<FixtureItemNodeFactory>()
                .As<IFixtureItemNodeFactory>()
                .SingleInstance();

            builder.RegisterType<AdornmentExpandButton>()
                .FindConstructorsWith(new AllCtorFinder())
                .As<IAdornmentExpander>();

            builder.RegisterType<FixtureItemNodeViewModel>();
            builder.RegisterType<FixtureItemNodeSender>();
            builder.RegisterType<ParameterNodeSender>();
            builder.RegisterType<MethodNodeSender>();
            builder.RegisterType<FixtureItemNodeReceiver>();
            builder.RegisterType<CtorFixtureItemNodeReceiver>();

            builder.RegisterType<FixtureItemNodeViewModelQuery>()
                .As<IFixtureItemNodeViewModelQuery>()
                .SingleInstance();

            builder.RegisterInstance(
                    new DispatcherSynchronizationContext(Application.Current.Dispatcher))
                .As<SynchronizationContext>()
                .SingleInstance();

            builder.RegisterType<UiEventHub>()
                .As<IUiEventHub>()
                .SingleInstance();

            builder.RegisterType<VsSolutionEventsPublisher>()
                .As<ISolutionEventsPublisher>()
                .SingleInstance();

            builder.RegisterType<RoslynSolutionEvents>()
                .As<ISolutionEventsPublisher>()
                .SingleInstance();

            builder.RegisterType<UnhandledExceptionsLogger>()
                .As<IUnhandledExceptionsLogger>()
                .SingleInstance();

            builder.RegisterType<VsProjectEventPublisherFactory>()
                .As<IVsProjectEventPublisherFactory>()
                .SingleInstance();

            builder.RegisterType<VsProjectEventsPublisher>()
                .As<IVsProjectEventsPublisher>();

            builder.RegisterType<VsColorService>()
                .As<IVsColorService>()
                .SingleInstance();

            builder.RegisterType<VsColorThemeEventWrapper>()
                .As<IVsColorThemeEventWrapper>()
                .SingleInstance();

            builder.RegisterType<AddinEntryPoint>()
                .As<IStartable, IAddinEntryPoint>()
                .SingleInstance();

            builder.RegisterType<VsEventCache>()
                .As<IVsEventCache, IVsEventCacheRegistrant>()
                .SingleInstance();

            builder.RegisterType<AboutWindow>();

            builder.RegisterType<AboutViewModel>()
                .As<IAboutViewModel>();

            builder.RegisterType<VsValueSegmentColorPicker>()
                .As<IValueSegmentColorPicker>()
                .SingleInstance();

            builder.RegisterType<ItemValueSegmentToRunConverter>()
                .As<IItemValueSegmentToRunConverter>()
                .SingleInstance();

            builder.RegisterType<ValidTokenToItemValueSegmentsConverter>()
                .As<IValidTokenToItemValueSegmentsConverter>();

            builder.RegisterType<AboutViewModelFactory>()
                .As<IAboutViewModelFactory>()
                .SingleInstance();

            builder.RegisterType<FixtureItemValueViewModelFactory>()
                .As<IFixtureItemValueViewModelFactory>()
                .SingleInstance();

            builder.RegisterType<FixtureItemNodeValueViewModel>()
                .As<IFixtureItemNodeValueViewModel>();

            builder.RegisterType<FixtureItemPeekResultContent>()
                .As<IFixtureItemPeekResultContent>();

            builder.RegisterType<ImageProvider>()
                .As<IImageProvider>()
                .SingleInstance();

            builder.RegisterType<ImagePathResolver>()
                .As<IImagePathResolver>()
                .SingleInstance();

            builder.RegisterType<FixtureItemNodeStatusFactory>()
                .As<IFixtureItemNodeStatusFactory>()
                .SingleInstance();

            builder.RegisterType<ViKeyboardTrackingService>()
                .As<IViKeyboardTrackingService>()
                .SingleInstance();

            builder.RegisterType<ErrorStatusIconViewModel>();
            builder.RegisterType<NoConfigurableMemberStatusIconViewModel>();
            builder.RegisterType<ArrayNotConfigurableStatusIconViewModel>();
            builder.RegisterType<StatusPanelViewModel>();
            builder.RegisterType<BaseTypeIsAlwaysUniqueStatusIconViewModel>();

            builder.RegisterType<AdornmentSession>()
                .As<IAdornmentSession>();

            builder.RegisterType<AdornmentSessionFactory>()
                .As<IAdornmentSessionFactory>()
                .SingleInstance();

            builder.RegisterType<VsCommandQuery>()
                .As<IVsCommandQuery>()
                .SingleInstance();

            builder.RegisterType<CompilationTypeQuery>()
                .As<ICompilationTypeQuery>();
        }
    }
}
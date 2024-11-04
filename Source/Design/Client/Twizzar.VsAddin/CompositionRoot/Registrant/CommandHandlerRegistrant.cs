using System.Diagnostics.CodeAnalysis;
using Autofac;
using Twizzar.Design.Core.Command;
using Twizzar.Design.Core.Command.FixtureItem;
using Twizzar.Design.CoreInterfaces.Command.Commands;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.VsAddin.Interfaces.CompositionRoot;

namespace Twizzar.VsAddin.CompositionRoot.Registrant
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class CommandHandlerRegistrant : IIocComponentRegistrant
    {
        #region members

        /// <inheritdoc />
        public void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterType<FixtureItemCommandHandler>()
                .As<ICommandHandler<ChangeMemberConfigurationCommand>>()
                .As<ICommandHandler<CreateFixtureItemCommand>>()
                .As<ICommandHandler<CreateCustomBuilderCommand>>()
                .SingleInstance();

            builder.RegisterType<EndFixtureItemConfigurationCommandHandler>()
                .As<ICommandHandler<EndFixtureItemConfigurationCommand>>()
                .SingleInstance();

            builder.RegisterType<StartFixtureItemConfigurationCommandHandler>()
                .As<ICommandHandler<StartFixtureItemConfigurationCommand>>()
                .SingleInstance();

            builder.RegisterType<AnalyticsCommandHandler>()
                .As<ICommandHandler<EnableOrDisableAnalyticsCommand>>()
                .SingleInstance();

            builder.RegisterType<SetDefaultShortcutsCommandHandler>()
                .As<ICommandHandler<SetDefaultShortcutsCommand>>()
                .SingleInstance();

            builder.RegisterType<CreateUnitTestCommandHandler>()
                .As<ICommandHandler<CreateUnitTestCommand>>()
                .SingleInstance();

            builder.RegisterType<UnitTestNavigationCommandHandler>()
                .As<ICommandHandler<UnitTestNavigationCommand>>()
                .SingleInstance();
        }

        #endregion
    }
}
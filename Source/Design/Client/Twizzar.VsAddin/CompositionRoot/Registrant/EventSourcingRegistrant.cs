using System.Diagnostics.CodeAnalysis;
using Autofac;
using Twizzar.Design.Core.Command.Services;
using Twizzar.Design.Core.Query.Services;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Infrastructure.Command.Services;
using Twizzar.VsAddin.Interfaces.CompositionRoot;

namespace Twizzar.VsAddin.CompositionRoot.Registrant
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class EventSourcingRegistrant : IIocComponentRegistrant
    {
        #region Implementation of IIocComponentRegistrant

        /// <inheritdoc />
        public void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterType<CommandBus>()
                .As<ICommandBus>()
                .SingleInstance();

            builder.RegisterType<EventBus>()
                .As<IEventBus>()
                .SingleInstance();

            builder.RegisterType<MemoryEventStore>()
                .As<IEventStore>()
                .SingleInstance();

            builder.RegisterType<EventStreamCollection>()
                .As<IEventStreamCollection>()
                .SingleInstance();

            builder.RegisterType<AutofacEventSourcingContainer>()
                .As<IEventSourcingContainer>()
                .As<IEventSourcingRegisterService>()
                .SingleInstance();

            builder.RegisterType<ProjectNameQuery>()
                .As<IProjectNameQuery>()
                .SingleInstance();

            builder.RegisterType<DocumentFileNameQuery>()
                .As<IDocumentFileNameQuery>()
                .SingleInstance();

            builder.RegisterType<BuildInvocationSpanQuery>()
                .As<IBuildInvocationSpanQuery>()
                .SingleInstance();
        }

        #endregion
    }
}

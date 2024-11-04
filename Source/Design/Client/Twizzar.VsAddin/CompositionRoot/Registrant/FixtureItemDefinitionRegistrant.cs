using System.Diagnostics.CodeAnalysis;
using Autofac;
using Twizzar.Design.Core.Command.FixtureItem.Definition;
using Twizzar.Design.CoreInterfaces.Command.FixtureItem.Definition;
using Twizzar.VsAddin.Factory;
using Twizzar.VsAddin.Interfaces.CompositionRoot;

namespace Twizzar.VsAddin.CompositionRoot.Registrant
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class FixtureItemDefinitionRegistrant : IIocComponentRegistrant
    {
        #region Implementation of IIocComponentRegistrant

        /// <inheritdoc />
        public void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterType<FixtureDefinitionNodeFactory>()
                .As<IFixtureDefinitionNodeFactory>()
                .SingleInstance();

            builder.RegisterType<FixtureItemDefinitionQuery>()
                .As<IFixtureItemDefinitionQuery>()
                .SingleInstance();

            builder.RegisterType<FixtureItemDefinitionNode>()
                .As<IFixtureItemDefinitionNode>();

            builder.RegisterType<FixtureItemDefinitionRepository>()
                .As<IFixtureItemDefinitionRepository>()
                .SingleInstance();
        }

        #endregion
    }
}

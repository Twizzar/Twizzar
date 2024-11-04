using System.Diagnostics.CodeAnalysis;

using Autofac;
using Twizzar.Design.CoreInterfaces.Common.FixtureItem.Description.Services;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.Design.Shared.Infrastructure.Description;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.SharedKernel.Core.FixtureItem.Description.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.VsAddin.Factory;
using Twizzar.VsAddin.Interfaces.CompositionRoot;

namespace Twizzar.VsAddin.CompositionRoot.Registrant
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage] // Ioc registrant.
    public class TypeDescriptionRegistrant : IIocComponentRegistrant
    {
        #region Implementation of IIocComponentRegistrant

        /// <inheritdoc />
        public void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterType<BaseTypeService>()
                .As<IBaseTypeService>()
                .SingleInstance();

            builder.RegisterType<RoslynDescriptionQuery>()
                .As<ITypeDescriptionQuery>()
                .SingleInstance();

            builder.RegisterType<RoslynAssignableTypesQuery>()
                .As<IAssignableTypesQuery>()
                .SingleInstance();

            builder.RegisterType<ViSymbolFinder>()
                .As<ISymbolFinder>()
                .SingleInstance();

            builder.RegisterType<RoslynDescriptionFactory>()
                .As<IRoslynDescriptionFactory>()
                .SingleInstance();

            builder.RegisterType<RoslynTypeDescription>()
                .As<ITypeDescription>();

            builder.RegisterType<TypeSymbolQuery>()
                .As<ITypeSymbolQuery>()
                .SingleInstance();
        }

        #endregion
    }
}

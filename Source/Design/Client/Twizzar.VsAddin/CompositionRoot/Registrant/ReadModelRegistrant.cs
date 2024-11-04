using System.Diagnostics.CodeAnalysis;

using Autofac;
using Twizzar.Design.Core.Query.Services;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.VsAddin.Interfaces.CompositionRoot;

namespace Twizzar.VsAddin.CompositionRoot.Registrant
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class ReadModelRegistrant : IIocComponentRegistrant
    {
        #region Implementation of IIocComponentRegistrant

        /// <inheritdoc />
        public void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterType<FixtureItemReadModelQuery>()
                .As<IFixtureItemReadModelQuery>()
                .SingleInstance();
        }

        #endregion
    }
}

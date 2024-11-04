using Autofac;
using Autofac.Core;
using Moq;
using Twizzar.Design.Infrastructure.Services;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.Tests.Services
{
    partial class ScopedServiceProviderTests
    {
        #region Nested type: EmptyScopedServiceProviderConfig

        private class EmptyScopedServiceProviderBuilder : ItemBuilder<ScopedServiceProvider, EmptyScopedServiceProviderBuilderPaths>
        {
            #region fields

            private readonly Mock<ILifetimeScope> _lifetimeScopeMock;

            #endregion

            #region ctors

            public EmptyScopedServiceProviderBuilder()
            {
                this._lifetimeScopeMock = new Mock<ILifetimeScope>();

                var componentRegistration = It.IsAny<IComponentRegistration>();

                this._lifetimeScopeMock
                    .Setup(scope => scope.ComponentRegistry)
                    .Returns(
                        () => Mock.Of<IComponentRegistry>(
                            registry =>
                                registry.TryGetRegistration(It.IsAny<Service>(), out componentRegistration) == true));

                this.With(p => p.Ctor.lifetimeScope.Value(this._lifetimeScopeMock.Object));
            }

            #endregion

            #region members

            public EmptyScopedServiceProviderBuilder AddService<T>(T service)
            {
                this._lifetimeScopeMock
                    .Setup(
                        scope => scope.ResolveComponent(
                            It.Is<ResolveRequest>(request => request.Service.Description == typeof(T).FullName)))
                    .Returns(service);

                this.With(p => p.Ctor.lifetimeScope.Value(this._lifetimeScopeMock.Object));
                return this;
            }

            #endregion
        }

        #endregion
    }
}
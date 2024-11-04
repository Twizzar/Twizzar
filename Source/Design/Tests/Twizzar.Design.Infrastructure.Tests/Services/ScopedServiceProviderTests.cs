using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.Infrastructure.Services;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.Tests.Services
{
    [TestFixture]
    public partial class ScopedServiceProviderTests
    {
        [Test]
        public void Ctor_parameters_throw_ArgumentNullException_when_null()
        {
            // assert
            Verify.Ctor<ScopedServiceProvider>();
        }

        [Test]
        public void Registered_service_is_returned_correctly()
        {
            // arrange
            object expectedInt = 5;
            var sut = new EmptyScopedServiceProviderBuilder().AddService(expectedInt).Build();

            // act
            var service = sut.GetService<object>();

            // assert
            service.GetValueUnsafe().Should().Be(expectedInt);
        }

        [Test]
        public void Dispose_is_delegated_up_to_LifeTimeScope()
        {
            // arrange
            var sut = new EmptyScopedServiceProviderBuilder().Build(out var context);

            // act
            sut.Dispose();

            // assert
            context.Verify(p => p.Ctor.lifetimeScope.Dispose).Called(1);
        }
    }
}
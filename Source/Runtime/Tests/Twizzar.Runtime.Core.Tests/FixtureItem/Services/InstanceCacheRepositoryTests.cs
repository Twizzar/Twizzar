using FluentAssertions;
using NUnit.Framework;
using Twizzar.Runtime.Core.FixtureItem.Services;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.Core.Tests.FixtureItem.Services
{
    [TestFixture]
    public class InstanceCacheRepositoryTests
    {
        [Test]
        public void Ctor_parameters_throw_ArgumentNullException_when_null()
        {
            // assert
            Verify.Ctor<InstanceCacheRepository>()
                .ShouldThrowArgumentNullException();
        }

        [Test]
        public void Registered_value_can_be_returned()
        {
            // assert
            var path = TestHelper.RandomString();
            var sut = new InstanceCacheRepository();

            // act
            sut.Register(path, 2);
            var output = sut.GetInstance(path);
            
            // assert
            var subjectValue = output.AsMaybeValue().Should().BeAssignableTo<SomeValue<object>>().Subject.Value;
            subjectValue.Should().Be(2);
        }
    }
}
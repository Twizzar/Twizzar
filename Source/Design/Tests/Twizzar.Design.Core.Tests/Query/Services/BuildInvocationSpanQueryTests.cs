using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Twizzar.Design.Core.Query.Services;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;

using TwizzarInternal.Fixture;

using ViCommon.Functional.Monads.MaybeMonad;

using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Core.Tests.Query.Services
{
    [TestFixture]
    public class BuildInvocationSpanQueryTests
    {
        [Test]
        public void Ctor_parameters_throw_ArgumentNullException_when_null()
        {
            // assert
            Verify.Ctor<BuildInvocationSpanQuery>()
                .ShouldThrowArgumentNullException();
        }

        [Test]
        public async Task When_FixtureItemConfigurationStartedEvent_is_in_eventStore_return_the_span_of_the_event()
        {
            // arrange
            var e = new ItemBuilder<FixtureItemConfigurationStartedEvent>().Build();

            var eventStore = new Mock<IEventStore>();

            eventStore.Setup(store => store.FindLast<FixtureItemConfigurationStartedEvent>(It.IsAny<string>()))
                .Returns(Maybe.SomeAsync(e));

            var sut = new BuildInvocationSpanQuery(eventStore.Object);

            // act
            var result = await sut.GetSpanAsync(RandomString());

            // assert
            var span = AssertAndUnwrapSuccess(result);
            span.Should().Be(e.InvocationSpan);
        }

        [Test]
        public async Task When_FixtureItemConfigurationStartedEvent_is_not_in_eventStore_return_failure()
        {
            // arrange
            var eventStore = new Mock<IEventStore>();

            eventStore.Setup(store => store.FindLast<FixtureItemConfigurationStartedEvent>(It.IsAny<string>()))
                .Returns(Maybe.NoneAsync<FixtureItemConfigurationStartedEvent>());

            var sut = new BuildInvocationSpanQuery(eventStore.Object);

            // act
            var result = await sut.GetSpanAsync(RandomString());

            // assert
            result.IsFailure.Should().BeTrue();
        }
    }
}
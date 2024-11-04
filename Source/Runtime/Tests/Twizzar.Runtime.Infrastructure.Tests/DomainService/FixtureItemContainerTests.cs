using NUnit.Framework;
using Twizzar.Fixture;
using Twizzar.Runtime.Infrastructure.DomainService;

namespace Twizzar.Runtime.Infrastructure.Tests.DomainService
{
    [Category("TwizzarInternal")]
    public class FixtureItemContainerTests
    {
        [Test]
        public void Ctor_parameters_throw_ArgumentNullException_when_null()
        {
            // assert
            Verify.Ctor<FixtureItemContainer>()
                .ShouldThrowArgumentNullException();
        }
    }
}
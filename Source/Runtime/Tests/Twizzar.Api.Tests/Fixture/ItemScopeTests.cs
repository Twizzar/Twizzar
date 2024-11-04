using NUnit.Framework;
using Twizzar.Fixture;

namespace Twizzar.Api.Tests.Fixture
{
    [TestFixture]
    public class ItemScopeTests
    {
        [Test]
        public void Ctor_parameters_throw_ArgumentNullException_when_null()
        {
            // assert
            Verify.Ctor<ItemContext<int, int>>()
                .ShouldThrowArgumentNullException();
        }
    }
}
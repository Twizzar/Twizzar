using NUnit.Framework;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;

using TwizzarInternal.Fixture;

namespace Twizzar.SharedKernel.CoreInterfaces.Tests.FixtureItem.Configuration.MemberConfigurations;

[TestFixture]
public class MethodConfigurationTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<MethodConfiguration>()
            .ShouldThrowArgumentNullException();
    }
}
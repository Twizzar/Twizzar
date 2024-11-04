using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.CoreInterfaces.Failures;
using TwizzarInternal.Fixture;
using static Twizzar.TestCommon.TestHelper;
using Verify = TwizzarInternal.Fixture.Verify;

namespace Twizzar.Design.Test.Failures;

[TestClass]
public class RoslynFailureTests
{
    [TestMethod]
    public void Ctor_test()
    {
        // assert
        Verify.Ctor<RoslynFailure>()
            .SetupParameter("message", RandomString())
            .ShouldThrowArgumentNullException();
    }

    [TestMethod]
    public void Equals_test()
    {
        // arrange
        var message1 = new ItemBuilder<string>().Build();
        var message2 = new ItemBuilder<string>().Build();

        var f1 = new RoslynFailure(message1);
        var f2 = new RoslynFailure(message2);
        var f3 = new RoslynFailure(message1);

        // act
        var expectFalse = f1.Equals(f2);
        var expectTrue = f1.Equals(f3);

        // assert
        expectFalse.Should().BeFalse();
        expectTrue.Should().BeTrue();
    }
}
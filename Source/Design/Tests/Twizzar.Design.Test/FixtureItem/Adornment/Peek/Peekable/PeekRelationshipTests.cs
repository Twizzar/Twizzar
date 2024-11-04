using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment.Peek.Peekable;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using TwizzarInternal.Fixture;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Test.FixtureItem.Adornment.Peek.Peekable;

[TestClass]
public class PeekRelationshipTests
{
    [TestMethod]
    public void Ctor_test()
    {
        // arrange
        var id = AdornmentId.CreateNew(RandomString());

        // assert
        Verify.Ctor<PeekRelationship>()
            .SetupParameter("entityId", id)
            .ShouldThrowArgumentNullException();
    }

    [TestMethod]
    public void Test_name_properties()
    {
        // arrange
        var id = AdornmentId.CreateNew(RandomString());
        var sut = new PeekRelationship(id);

        // assert
        sut.Name.Should().Be("TWIZZAR");
        sut.DisplayName.Should().Be("TWIZZAR");
    }

    [TestMethod]
    public void Equality_test()
    {
        // arrange
        var id1 = AdornmentId.CreateNew(RandomString());
        var id2 = AdornmentId.CreateNew(RandomString());

        var rel1 = new PeekRelationship(id1);
        var rel2 = new PeekRelationship(id2);
        var rel3 = new PeekRelationship(id1);

        // act
        var expetFalse = rel1.Equals(rel2);
        var expectTrue = rel1.Equals(rel3);

        // assert
        expetFalse.Should().BeFalse();
        expectTrue.Should().BeTrue();
    }
}
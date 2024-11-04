using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Interfaces.Enums;
using Twizzar.Design.Ui.Interfaces.ValueObjects;

namespace Twizzar.Design.Test.UnitTest.ValueObjects;

[TestClass]
public class ItemValueSegmentTest
{
    [TestMethod]
    public void Ctor_throws_ArgumentNullException_when_Content_Is_null()
    {
        // act
        Action contentIsNull = () => new ItemValueSegment(null, SegmentFormat.DefaultOrUndefined, true);

        // assert
        contentIsNull.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("content");
    }

    [TestMethod]
    public void PropertiesGetter_Has_SameValues_AsCtorParameter()
    {
        var itemValueSegment = new ItemValueSegment("null", SegmentFormat.Keyword, true);

        itemValueSegment.Content.Should().Be("null");
        itemValueSegment.Format.Should().Be(SegmentFormat.Keyword);
        itemValueSegment.IsValid.Should().Be(true);
    }
}
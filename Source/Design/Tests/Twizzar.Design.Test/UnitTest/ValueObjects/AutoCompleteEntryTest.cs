using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Interfaces.Enums;
using Twizzar.Design.Ui.Interfaces.ValueObjects;

namespace Twizzar.Design.Test.UnitTest.ValueObjects;

[TestClass]
public class AutoCompleteEntryTest
{
    [TestMethod]
    public void Ctor_throws_ArgumentNullException_when_Text_Is_null()
    {
        // act
        Action contentIsNull = () => new AutoCompleteEntry(null, AutoCompleteFormat.Keyword);

        // assert
        contentIsNull.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("text");
    }

    [TestMethod]
    public void PropertiesGetter_Has_SameValues_AsCtorParameter()
    {
        var itemValueSegment = new AutoCompleteEntry("null", AutoCompleteFormat.Keyword);

        itemValueSegment.Text.Should().Be("null");
        itemValueSegment.Format.Should().Be(AutoCompleteFormat.Keyword);
    }
}
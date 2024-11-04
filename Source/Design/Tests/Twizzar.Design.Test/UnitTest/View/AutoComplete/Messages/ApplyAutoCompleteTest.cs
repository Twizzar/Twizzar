using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Interfaces.View.Mediator;
using Twizzar.Design.Ui.View.Mediator;
using Twizzar.Design.Ui.View.Mediator.Messages;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Test.UnitTest.View.AutoComplete.Messages;

/// <summary>
/// Tests the Apply Auto complete message.
/// </summary>
[TestClass]
public class ApplyAutoCompleteTest
{
    #region members

    [TestMethod]
    public void Ctor_throws_ArgumentNullException_when_Arguments_are_null()
    {
        Verify.Ctor<ApplyAutoComplete>()
            .SetupParameter("sender", this)
            .SetupParameter("richTextBoxSpan", new RichTextBoxSpan(0, 0, ""))
            .SetupParameter("selectedString", TestHelper.RandomString())
            .ShouldThrowArgumentNullException();
    }

    [TestMethod]
    public void When_create_a_mediator_message_then_properties_must_equal_ctor_parameters()
    {
        // Arrange
        var selectedString = "autCompleteText";
        var richTextBoxSpan = new RichTextBoxSpan(0, 1, "someText");

        // Act
        var sut = new ApplyAutoComplete(this, richTextBoxSpan, selectedString);

        // Assert
        sut.Sender.Should().BeEquivalentTo(this);
        sut.SelectedString.Should().BeEquivalentTo(selectedString);
        sut.RichTextBoxSpan.Should().BeEquivalentTo(richTextBoxSpan);
        sut.Should().BeAssignableTo(typeof(IMediatorMessage));
    }

    #endregion
}
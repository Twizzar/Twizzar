using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Interfaces.View.Mediator;
using Twizzar.Design.Ui.View.Mediator;
using Twizzar.Design.Ui.View.Mediator.Messages;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Test.UnitTest.View.AutoComplete.Messages;

/// <summary>
/// Test the message to start the auto complete.
/// </summary>
[TestClass]
public class RichTextBoxTextChangedTest
{
    #region members

    [TestMethod]
    public void Ctor_throws_ArgumentNullException_when_Arguments_are_null()
    {
        Verify.Ctor<RichTextBoxTextChanged>()
            .SetupParameter("sender", this)
            .SetupParameter("richTextBoxSpan", new RichTextBoxSpan(0, 0, ""))
            .ShouldThrowArgumentNullException();
    }

    [TestMethod]
    public void When_create_a_mediator_message_then_properties_must_equal_ctor_parameters()
    {
        // Arrange
        var caretSpan = new RichTextBoxSpan(1, 4, "TextFromStartToCaret");

        // Act
        var sut = new RichTextBoxTextChanged(this, caretSpan);

        // Assert
        sut.Sender.Should().BeEquivalentTo(this);
        sut.CaretSpan.Should().BeEquivalentTo(caretSpan);
        sut.Should().BeAssignableTo(typeof(IMediatorMessage));
    }

    #endregion
}
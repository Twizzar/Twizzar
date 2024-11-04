using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Interfaces.Enums;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.View.Mediator;
using Twizzar.Design.Ui.View.Mediator;
using Twizzar.Design.Ui.View.Mediator.Messages;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Test.UnitTest.View.AutoComplete.Messages;

/// <summary>
/// Tests the message to start the auto complete.
/// </summary>
[TestClass]
public class StartAutoCompleteTest
{
    #region members

    [TestMethod]
    public void Ctor_throws_ArgumentNullException_when_Arguments_are_null()
    {
        Verify.Ctor<StartAutoComplete>()
            .SetupParameter("sender", this)
            .SetupParameter("caretSpan", new RichTextBoxSpan(0, 0, ""))
            .SetupParameter("autocompleteEntries", new List<AutoCompleteEntry>())
            .ShouldThrowArgumentNullException();
    }

    [TestMethod]
    public void When_create_a_mediator_message_then_properties_must_equal_ctor_parameters()
    {
        // Arrange
        var caretSpan = new RichTextBoxSpan(1, 4, "TextFromStartToCaret");

        var autoCompleteEntries = new List<AutoCompleteEntry>
            {new("IBus #123", AutoCompleteFormat.TypeAndId)};

        // Act
        var sut = new StartAutoComplete(this, caretSpan, autoCompleteEntries);

        // Assert
        sut.Sender.Should().BeEquivalentTo(this);
        sut.CaretSpan.Should().BeEquivalentTo(caretSpan);
        sut.AutoCompleteEntries.Should().BeEquivalentTo(autoCompleteEntries);
        sut.Should().BeAssignableTo(typeof(IMediatorMessage));
    }

    #endregion
}
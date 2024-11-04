using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Interfaces.Enums;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.View.Mediator;
using Twizzar.Design.Ui.View.Mediator.Messages;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Test.UnitTest.View.AutoComplete.Messages;

/// <summary>
/// Test the message to tell, that the original auto complete list has been changed.
/// </summary>
[TestClass]
public class AutoCompleteEntriesChangedTest
{
    [TestMethod]
    public void Ctor_throws_ArgumentNullException_when_Arguments_are_null()
    {
        Verify.Ctor<AutoCompleteEntry>()
            .SetupParameter("sender", this)
            .SetupParameter("autoCompleteEntries", new List<AutoCompleteEntry>())
            .ShouldThrowArgumentNullException();
    }

    [TestMethod]
    public void When_create_a_mediator_message_then_properties_must_equal_ctor_parameters()
    {
        // Arrange
        var autoCompleteEntry = new AutoCompleteEntry("ICar", AutoCompleteFormat.Type);
        var autoCompleteEntries = new List<AutoCompleteEntry>() {autoCompleteEntry};
            
        // Act
        var sut = new AutoCompleteEntriesChanged(this, autoCompleteEntries);
            
        // Assert
        sut.Sender.Should().BeEquivalentTo(this);
        sut.AutoCompleteEntries.Should().BeEquivalentTo(autoCompleteEntries);
        sut.AutoCompleteEntries.First().Text.Should().BeEquivalentTo(autoCompleteEntry.Text);
        sut.AutoCompleteEntries.First().Format.Should().BeEquivalentTo(autoCompleteEntry.Format);
        sut.Should().BeAssignableTo(typeof(IMediatorMessage));
    }
}
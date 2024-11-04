using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Interfaces.View.Mediator;
using Twizzar.Design.Ui.View.Mediator.Messages;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Test.UnitTest.View.AutoComplete.Messages;

/// <summary>
/// The message to close the auto complete.
/// </summary>
[TestClass]
public class CloseAutoCompleteTest
{
    #region members

    [TestMethod]
    public void Ctor_throws_ArgumentNullException_when_Arguments_are_null()
    {
        Verify.Ctor<CloseAutoComplete>()
            .SetupParameter("sender", this)
            .ShouldThrowArgumentNullException();
    }

    [TestMethod]
    public void When_create_a_mediator_message_then_properties_must_equal_ctor_parameters()
    {
        // Arrange
        // Act
        var sut = new CloseAutoComplete(this);

        // Assert
        sut.Sender.Should().BeEquivalentTo(this);
        sut.Should().BeAssignableTo(typeof(IMediatorMessage));
    }

    #endregion
}
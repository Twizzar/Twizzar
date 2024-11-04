using System;
using System.Windows;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Interfaces.View.Mediator;
using Twizzar.Design.Ui.View.Mediator.Messages;

namespace Twizzar.Design.Test.UnitTest.View.AutoComplete.Messages;

/// <summary>
/// Test the message notifies, that placement target has changed.
/// </summary>
[TestClass]
public class PlacementTargetChangedTest
{
    #region members

    [TestMethod]
    public void Ctor_throws_ArgumentNullException_when_Arguments_are_null()
    {
        // Arrange
        Action action = () => new PlacementTargetChanged(null, default);

        // Act
        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void When_create_a_mediator_message_then_properties_must_equal_ctor_parameters()
    {
        // Arrange
        var placementRect = new Rect(new Size(3, 2));

        // Act
        var sut = new PlacementTargetChanged(this, placementRect);

        // Assert
        sut.Sender.Should().BeEquivalentTo(this);
        sut.PlacementRect.Should().BeEquivalentTo(placementRect);
        sut.Should().BeAssignableTo(typeof(IMediatorMessage));
    }

    #endregion
}
using System;
using System.Windows.Input;
using System.Windows.Interop;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Interfaces.View.Mediator;
using Twizzar.Design.Ui.View.Mediator.Messages;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Test.UnitTest.View.AutoComplete.Messages;

/// <summary>
/// Test the message to close the auto complete.
/// </summary>
[TestClass]
public class OnPreviewKeyDownTest
{
    #region members

    [TestMethod]
    public void Ctor_throws_ArgumentNullException_when_Arguments_are_null()
    {
        Verify.Ctor<OnPreviewKeyDown>()
            .SetupParameter("sender", this)
            .SetupParameter("keyEventArgs", CreateKeyEventArgs(Key.A))
            .ShouldThrowArgumentNullException();
    }

    [TestMethod]
    public void When_create_a_mediator_message_then_properties_must_equal_ctor_parameters()
    {
        // Arrange
        var keyEventArgs = CreateKeyEventArgs(Key.B);

        // Act
        var sut = new OnPreviewKeyDown(this, keyEventArgs);

        // Assert
        sut.Sender.Should().BeEquivalentTo(this);
        sut.KeyEventArgs.Should().BeEquivalentTo(keyEventArgs);
        sut.Should().BeAssignableTo(typeof(IMediatorMessage));
    }

    public static KeyEventArgs CreateKeyEventArgs(Key key)
    {
        var keyboardDevice = Keyboard.PrimaryDevice;

        var keyEventArgs = new KeyEventArgs(
            keyboardDevice,
            new HwndSource(0, 0, 0, 0, 0, "", IntPtr.Zero), // dummy source
            0,
            key);

        return keyEventArgs;
    }

    #endregion
}
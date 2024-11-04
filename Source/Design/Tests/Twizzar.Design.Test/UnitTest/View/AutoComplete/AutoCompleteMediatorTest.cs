using System;
using System.Linq;
using System.Windows.Input;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Design.Test.UnitTest.View.AutoComplete.Messages;
using Twizzar.Design.Ui.View;
using Twizzar.Design.Ui.View.Mediator;
using Twizzar.Design.Ui.View.Mediator.Messages;
using Twizzar.Design.Ui.View.RichTextBox;

namespace Twizzar.Design.Test.UnitTest.View.AutoComplete;

/// <summary>
/// Tests the <see cref="InputValueMediator"/>
/// </summary>
[TestClass]
public class AutoCompleteMediatorTest
{
    #region members

    [TestMethod]
    public void Ctor_throws_ArgumentNullException_when_Arguments_are_null()
    {
        // Arrange
        Action action1 = () => new InputValueMediator(null, new AutoCompletionPopup());
        Action action2 = () => new InputValueMediator(new FixtureItemValueRichTextBox(), null);

        // Act
        // Assert
        action1.Should().Throw<ArgumentNullException>();
        action2.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void NotifyMediator_From_TextBox_ToClose_Popup_Results_in_Closed_Popup()
    {
        // Act
        var richTextBox = new FixtureItemValueRichTextBox();

        var popup = new AutoCompletionPopup
        {
            IsOpen = true,
        };

        var sut = new InputValueMediator(richTextBox, popup);

        // Act
        sut.Notify(new CloseAutoComplete(richTextBox));

        // Assert
        popup.IsOpen.Should().Be(false);
    }

    [TestMethod]
    public void RichTextBox_tells_the_mediator_that_enter_is_clicked_then_selected_AutoCompleteEntry_is_applied()
    {
        // Act
        var dataContext = new FixtureItemNodeValueViewModelMock();
        var richTextBox = new FixtureItemValueRichTextBox();

        var popup = new AutoCompletionPopup
        {
            IsOpen = true,
        };

        var sut = new InputValueMediator(richTextBox, popup);
        richTextBox.InitializeMediator(sut);
        popup.InitializeMediator(sut);

        richTextBox.DataContext = dataContext;
        var downArgs = OnPreviewKeyDownTest.CreateKeyEventArgs(Key.Down);
        var enterArgs = OnPreviewKeyDownTest.CreateKeyEventArgs(Key.Enter);

        // Open the popup if not yet opened.
        sut.Notify(
            new StartAutoComplete(richTextBox, new RichTextBoxSpan(0, 0, ""), dataContext.AutoCompleteEntries));

        // select first auto complete entry.
        sut.Notify(new OnPreviewKeyDown(richTextBox, downArgs));

        // Act
        sut.Notify(new OnPreviewKeyDown(richTextBox, enterArgs));

        // Assert
        richTextBox.FullText().Should().BeEquivalentTo(dataContext.AutoCompleteEntries.First().Text);
        popup.IsOpen.Should().Be(false);
    }

    [TestMethod]
    [DataRow(Key.Down, FocusNavigationDirection.Down)]
    [DataRow(Key.Up, FocusNavigationDirection.Up)]
    public void When_popUp_is_closed_notify_richText_box_on_keyDown(Key key, FocusNavigationDirection direction)
    {
        // arrange
        var richTextBox = new Mock<IFixtureItemValueRichTextBox>();
        var popup = Mock.Of<IAutoCompletionPopup>(completionPopup => completionPopup.IsOpen == false);

        var sut = new InputValueMediator(richTextBox.Object, popup);

        // act
        var downArgs = OnPreviewKeyDownTest.CreateKeyEventArgs(key);
        sut.Notify(new OnPreviewKeyDown(richTextBox.Object, downArgs));

        // assert
        richTextBox.Verify(box => box.RespondToMediator(new MoveFocus(sut, direction)), Times.Once);
    }

    [TestMethod]
    [DataRow(Key.Down, FocusNavigationDirection.Down)]
    [DataRow(Key.Up, FocusNavigationDirection.Up)]
    public void When_popUp_is_open_do_not_notify_richText_box_on_keyDown(Key key, FocusNavigationDirection direction)
    {
        // arrange
        var richTextBox = new Mock<IFixtureItemValueRichTextBox>();
        var popup = Mock.Of<IAutoCompletionPopup>(completionPopup => completionPopup.IsOpen == true);

        var sut = new InputValueMediator(richTextBox.Object, popup);

        // act
        var downArgs = OnPreviewKeyDownTest.CreateKeyEventArgs(key);
        sut.Notify(new OnPreviewKeyDown(richTextBox.Object, downArgs));

        // assert
        richTextBox.Verify(box => box.RespondToMediator(new MoveFocus(sut, direction)), Times.Never);
    }

    #endregion
}
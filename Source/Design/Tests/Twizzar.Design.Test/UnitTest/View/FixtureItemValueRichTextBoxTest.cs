using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Design.Ui.Interfaces.View.Mediator;
using Twizzar.Design.Ui.View;
using Twizzar.Design.Ui.View.Mediator;
using Twizzar.Design.Ui.View.Mediator.Messages;
using Twizzar.Design.Ui.View.RichTextBox;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Test.UnitTest.View;

[TestClass]
public class FixtureItemValueRichTextBoxTest
{
    #region members

    [TestMethod]
    public void Ctor_throws_ArgumentNullException_when_Arguments_are_null()
    {
        // assert
        Verify.Ctor<FixtureItemValueRichTextBox>()
            .ShouldThrowArgumentNullException();
    }

    [TestMethod]
    public void When_text_is_changed_new_segments_and_runs_are_correctly_generated()
    {
        // arrange
        var sut = new FixtureItemValueRichTextBox();
        sut.InitializeMediator(new InputValueMediator(sut, new AutoCompletionPopup()));
        var fixtureItemValueViewModel = new FixtureItemNodeValueViewModelMock();
        sut.DataContext = fixtureItemValueViewModel;

        // act
        ChangeText(sut, "SomeText");

        // assert
        var segmentContents = fixtureItemValueViewModel.ItemValueSegments.Select((segment) => segment.Content);
        var expectedFullText = string.Join("", segmentContents);

        sut.FullText().Should().BeEquivalentTo(expectedFullText);
        sut.Document.Blocks.Count.Should().Be(1);
        var paragraph = (Paragraph)sut.Document.Blocks.FirstBlock;
        var valueSegmentsCount = fixtureItemValueViewModel.ItemValueSegments.Count();
        paragraph.Inlines.Count.Should().Be(valueSegmentsCount);
    }

    [TestMethod]
    public void RichTextBox_WithEmptyParagraph_FullTextIsEmpty()
    {
        var rtb = new RichTextBox();
        rtb.Document.Blocks.Add(new Paragraph());
        var fullText = rtb.FullText();
        fullText.Should().BeEquivalentTo(string.Empty);
    }

    [TestMethod]
    public void CommitTotalValue_mediator_message_causes_commit_on_the_viewModel()
    {
        // Arrange
        var rtb = new FixtureItemValueRichTextBox();
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(mediator => mediator.Notify(It.IsAny<IMediatorMessage>()));
        rtb.InitializeMediator(mediatorMock.Object);
        var fixtureItemValueViewModelMock = new FixtureItemNodeValueViewModelMock {FullText = "some Text"};
        rtb.DataContext = fixtureItemValueViewModelMock;

        // Act
        rtb.RespondToMediator(new CommitTotalValue(this));

        // Assert
        fixtureItemValueViewModelMock.CommitsCount.Should().Be(1);
    }

    private static void ChangeText(FixtureItemValueRichTextBox sut, string text)
    {
        var inline = new Run
        {
            Text = text,
        };

        var paragraph = new Paragraph(inline);
        sut.Document.Blocks.Add(paragraph);
    }

    #endregion
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Design.Ui.Interfaces.Enums;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.View;
using Twizzar.Design.Ui.View.RichTextBox;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Test.UnitTest.View;

[TestClass]
public class ItemValueSegmentToRunConverterTest
{
    [TestMethod]
    public void Ctor_throws_ArgumentNullException_when_Arguments_are_null()
    {
        // assert
        Verify.Ctor<ItemValueSegmentToRunConverter>()
            .ShouldThrowArgumentNullException();
    }

    [TestMethod]
    public async Task ConvertToRuns_ShouldCreateExpectedRunsFromSegments()
    {
        // arrange
        var valueSegmentColorPicker = new Mock<IValueSegmentColorPicker>();

        var expectedBrush = new SolidColorBrush(Colors.Blue);

        valueSegmentColorPicker.Setup(picker => picker.GetSegmentColor(It.IsAny<ItemValueSegment>()))
            .Returns(Task.FromResult<Brush>(expectedBrush));

        var sut = new ItemValueSegmentToRunConverter(valueSegmentColorPicker.Object);
        var segmentText = "some content";

        IList<ItemValueSegment> valueSegments = new List<ItemValueSegment>
        {
            new(segmentText, SegmentFormat.Letter, false),
        };

        // act
        var runs = await sut.ConvertToRunsAsync(valueSegments);

        // assert
        runs.Count().Should().Be(valueSegments.Count);
        var firstRun = runs.First();
        firstRun.Text.Should().BeEquivalentTo(segmentText);
        firstRun.Foreground.Should().BeEquivalentTo(expectedBrush);
    }
}
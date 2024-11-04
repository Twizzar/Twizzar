using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NCrunch.Framework;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.Ui.Interfaces.Messaging.VsEvents;
using Twizzar.Design.Ui.Interfaces.Parser;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Validator;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.Parser.SyntaxTree.Literals;
using Twizzar.Design.Ui.Validator;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Value;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Ui.Tests.ViewModels.FixtureItem.Value;

[TestFixture]
[Atomic]
public partial class FixtureItemNodeValueViewModelTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<FixtureItemNodeValueViewModel>()
            .SetupParameter("id", new NodeId())
            .SetupParameter("isReadOnly", false)
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void Set_FullText_gets_expected_text()
    {
        var expectedToken = ViStringToken.CreateWithoutWhitespaces(0, 10, "0123456789");
        var sut = CreateFixtureItemValueViewModel(expectedToken);

        var expectedText = "new text";

        sut.FullText = expectedText;

        sut.FullText.Should().Be(expectedText);
    }

    [Test]
    public async Task Update_logs_thrown_exceptions()
    {
        // arrange
        var expectedToken = ViStringToken.CreateWithoutWhitespaces(0, 10, "0123456789");
        var loggerMock = new Mock<ILogger>();
        var invalidParserMock = new Mock<IParser>();
        invalidParserMock.Setup(p => p.Parse(It.IsAny<string>()))
            .Throws<InvalidOperationException>();

        var sut = CreateFixtureItemValueViewModel(
            expectedToken,
            invalidParserMock.Object);
        sut.Logger = loggerMock.Object;

        // act
        await sut.SetValueAsync("newValue", false);

        // assert
        loggerMock.Verify(l => l.Log(It.IsAny<Exception>(), LogLevel.Error), Times.Once);
    }

    [Test]
    public void Commit_logs_thrown_exceptions()
    {
        // arrange
        var expectedToken = ViStringToken.CreateWithoutWhitespaces(0, 10, "0123456789");
        var loggerMock = new Mock<ILogger>();
        var invalidParserMock = new Mock<IParser>();
        invalidParserMock.Setup(p => p.Parse(It.IsAny<string>()))
            .Throws<InvalidOperationException>();

        var sut = CreateFixtureItemValueViewModel(
            expectedToken,
            invalidParserMock.Object);
        sut.FullText = TestHelper.RandomString();
        sut.Logger = loggerMock.Object;

        // act
        sut.Commit.Execute(null);

        // assert
        loggerMock.Verify(l => l.Log(It.IsAny<Exception>(), LogLevel.Error), Times.Once);
    }

    [Test]
    public void When_this_is_not_disposed_FixtureItemNodeFocusedEvent_is_triggered()
    {
        // arrange
        var sut = new FixtureItemNodeValueViewModel28c7Builder().Build(out var context);

        // act
        sut.Focus();

        // assert
        var eventHubMock = context.GetAsMoq(p => p.Ctor.uiEventHub);
        eventHubMock.Verify(hub => hub.Publish(It.IsAny<FixtureItemNodeFocusedEvent>()));
    }

    [Test]
    public void When_this_is_disposed_no_focus_FixtureItemNodeFocusedEvent_is_triggered()
    {
        // arrange
        var sut = new FixtureItemNodeValueViewModelf18cBuilder().Build(out var context);

        // act
        sut.Focus();

        // assert
        var eventHubMock = context.GetAsMoq(p => p.Ctor.uiEventHub);
        eventHubMock.Verify(hub => hub.Publish(It.IsAny<FixtureItemNodeFocusedEvent>()), Times.Never);
    }

    [Test]
    public void Te()
    {
        // arrange
        var sut = new FixtureItemNodeValueViewModel414eBuilder().Build(out var context);

        // act
        sut.Focus();

        // assert
        var fixtureItemNodeViewModel = context.Get(p => p.FixtureNodeVM);
        var parentNode = ((IFixtureItemNode)fixtureItemNodeViewModel).Parent;

        var mock = Mock.Get(parentNode.GetValueUnsafe().NodeValueController);
        mock.Verify(controller => controller.Focus());
    }

    private static FixtureItemNodeValueViewModel CreateFixtureItemValueViewModel(ViStringToken expectedToken, IParser parser = null)
    {
        var parserMock = new Mock<IParser>();
        parserMock.SetupAllProperties();

        parserMock.Setup(parser => parser.Parse(It.IsAny<string>()))
            .Returns(() => new Result<IViToken, ParseFailure>());

        var validatorMock = new Mock<IValidator>();
        validatorMock.SetupAllProperties();

        validatorMock.Setup(validator =>
            validator.ValidateAsync(It.IsAny<IResult<IViToken, ParseFailure>>())).Returns(Task.FromResult<IViToken>(expectedToken));

        validatorMock.Setup(
                validator =>
                    validator.Prettify(expectedToken))
            .Returns(expectedToken);

        var sut = new FixtureItemNodeValueViewModel(
            new NodeId(),
            parser ?? parserMock.Object,
            validatorMock.Object,
            new ValidTokenToItemValueSegmentsConverter(),
            false,
            new Mock<IUiEventHub>().Object,
            new Mock<IVsCommandQuery>().Object,
            Mock.Of<IScopeServiceProviderFactory>());

        return sut;
    }
}
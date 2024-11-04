using System;
using FluentAssertions;
using Microsoft.VisualStudio.Text.Editor;
using Moq;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Common.Messaging.Events;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.Design.Ui.Interfaces.Factories;
using Twizzar.Design.Ui.Interfaces.Messaging.VsEvents;
using Twizzar.Design.Ui.Interfaces.VisualStudio;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.FixtureItem.Adornment;

[TestFixture]
public class ViAdornmentTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<ViAdornment>()
            .IgnoreParameter("adornmentInformation", DefaultAdornmentInformationItemBuilder().Build())
            .ShouldThrowArgumentNullException();
    }

    [TestCase(true, 1)]
    [TestCase(false, 0)]
    public void ViAdornment_dispose_release_events_correctly(bool isExpanded, int expectedNumberOfCalls)
    {
        // arrange
        var adornmentExpander = new Mock<IAdornmentExpander>();
        var eventHub = new Mock<IUiEventHub>();
        var adornmentController = new Mock<IDocumentAdornmentController>();

        var factory = new ItemBuilder<IUiElementsFactory>()
            .With(p => p.CreateAdornmentExpander.Value(adornmentExpander.Object))
            .Build();

        var sut = new ItemBuilder<ViAdornment>()
            .With(p => p.Ctor.eventHub.Value(eventHub.Object))
            .With(p => p.Ctor.documentAdornmentController.Value(adornmentController.Object))
            .With(p => p.Ctor.elementsFactory.Value(factory))
            .With(p => p.Ctor.adornmentInformation.Value(DefaultAdornmentInformationItemBuilder().Build()))
            .Build();

        sut.UpdateIsExpanded(isExpanded);

        // act
        sut.Dispose();

        // assert
        adornmentExpander.Verify(x => x.Dispose(), Times.Once);
        eventHub.Verify(hub => hub.Unsubscribe(sut, It.IsAny<Action<AdornmentExpandedOrCollapsedEvent>>()), Times.Once);
        eventHub.Verify(hub => hub.Unsubscribe(sut, It.IsAny<Action<VsOpenOrCloseShortcutPressedEvent>>()), Times.Once);
        eventHub.Verify(hub => hub.Unsubscribe(sut, It.IsAny<Action<PeekCollapsedEvent>>()), Times.Once);
        adornmentController.Verify(c => c.CloseAdornmentAsync(sut), Times.Exactly(expectedNumberOfCalls));
    }

    [TestCase(true, 1)]
    [TestCase(false, 0)]

    public void ViAdornment_update_calls_adornmentController(bool isExpanded, int expectedNumberOfCalls)
    {
        // arrange
        var adornmentController = new Mock<IDocumentAdornmentController>();

        var oldAdornmentInfo = DefaultAdornmentInformationItemBuilder().Build();
        var newAdornmentInfo = DefaultAdornmentInformationItemBuilder().Build();

        var sut = new ItemBuilder<ViAdornment>()
            .With(p => p.Ctor.documentAdornmentController.Value(adornmentController.Object))
            .With(p => p.Ctor.adornmentInformation.Value(oldAdornmentInfo))
            .Build();

        sut.UpdateIsExpanded(isExpanded);

        // act
        sut.Update(newAdornmentInfo);

        // assert
        adornmentController.Verify(c => c.UpdateInformationAsync(newAdornmentInfo), Times.Exactly(expectedNumberOfCalls));
        sut.AdornmentInformation.Should().Be(newAdornmentInfo);
    }

    [Test]
    public void Equals_is_implemented_correctly()
    {
        //// arrange
        var factory =Build.New<IUiElementsFactory>();
        var info = DefaultAdornmentInformationItemBuilder().Build();
        var history = Build.New<ISnapshotHistory>();
        var textView =Build.New<IWpfTextView>();
        var eventHub = Build.New<IUiEventHub>();
        var statusFactory = Build.New<IFixtureItemNodeStatusFactory>();
        var controller = Build.New<IDocumentAdornmentController>();
        var baseTypeService = Build.New<IBaseTypeService>();

        var adornmentA = new ViAdornment(factory, info, history, textView, eventHub, statusFactory, controller, baseTypeService, statusFactory);
        var adornmentB = new ViAdornment(factory, info, history, textView, eventHub, statusFactory, controller, baseTypeService, statusFactory);

        // act
        var aEqualsA = adornmentA.Equals(adornmentA);
        var aEqualsB = adornmentA.Equals(adornmentB);

        // assert
        aEqualsA.Should().BeTrue();
        aEqualsB.Should().BeFalse();
    }

    private static ItemBuilder<IAdornmentInformation> DefaultAdornmentInformationItemBuilder() =>
        new ItemBuilder<IAdornmentInformation>()
            .With(p => p.ProjectName.Value("TestProject"))
            .With(p => p.FixtureItemId.Value(TestHelper.RandomNamedFixtureItemId()));
}
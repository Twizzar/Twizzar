using System;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Twizzar.Design.Core.Command;
using Twizzar.Design.CoreInterfaces.Command.Commands;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.SharedKernel.NLog.Logging;
using Twizzar.TestCommon;

using TwizzarInternal.Fixture;
using TwizzarInternal.Fixture.Member;

using ViCommon.Functional;
using ViCommon.Functional.Monads.ResultMonad;

using SutContext = TwizzarInternal.Fixture.IItemContext<Twizzar.Design.Core.Command.UnitTestNavigationCommandHandler, Twizzar.Design.Core.Tests.Command.UnitTestNavigationCommandHandlerBuilderPaths>;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Twizzar.Design.Core.Tests.Command;

[TestFixture]
public partial class UnitTestNavigationCommandHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        LoggerFactory.SetConfig(new LoggerConfigurationBuilder());
    }

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<UnitTestNavigationCommandHandler>()
            .ShouldThrowArgumentNullException();
    }

    private static async Task<SutContext> ArrangeAndActAsync(params Func<UnitTestNavigationCommandHandlerBuilderPaths, MemberConfig<UnitTestNavigationCommandHandler>>[] additionalConfigs)
    {
        var builder = new UnitTestNavigationCommandHandlerBuilder();
        foreach (var additionalConfig in additionalConfigs)
        {
            builder.With(additionalConfig);
        }

        var sut = builder
            .Build(out var context);

        var command = new ItemBuilder<UnitTestNavigationCommand>()
            .Build();

        await sut.HandleAsync(command);

        return context;
    }

    [Test]
    public async Task GetCurrentLocation_was_called()
    {
        // arrange & act
        var context = await ArrangeAndActAsync();

        // assert
        context.Verify(p => p.Ctor.locationService.GetCurrentLocation)
            .Called(1);
    }

    [Test]
    public async Task MapAsync_is_called_when_navigation_back_fails()
    {
        // arrange & act
        var context = await ArrangeAndActAsync(
            p => p.Ctor.navigationService.NavigateBackAsync.Value(Result.FailureAsync<Unit, Failure>(new Failure(""))));

        // assert
        context.Verify(p => p.Ctor.mappingService.MapAsync)
            .Called(1);
    }

    [Test]
    public async Task NavigateAsync_is_called_when_navigation_back_fails()
    {
        // arrange & act
        var context = await ArrangeAndActAsync(
            p => p.Ctor.navigationService.NavigateBackAsync.Value(Result.FailureAsync<Unit, Failure>(new Failure(""))));

        // assert
        context.Verify(p => p.Ctor.navigationService.NavigateAsync)
            .Called(1);
    }

    [Test]
    public async Task OnSuccess_publish_UnitTestNavigatedEvent()
    {
        // arrange & act
        var context = await ArrangeAndActAsync();

        // assert
        context.Verify(p => p.Ctor.eventBus.PublishAsyncTEvent)
            .WhereEIs(new UnitTestNavigatedEvent())
            .Called(1);
    }

    [Test]
    public async Task OnFailure_publish_UnitTestUnitTestNavigationFailedEvent()
    {
        // arrange
        var reason = TestHelper.RandomString();
        var navigationServiceMock = new Mock<INavigationService>();

        navigationServiceMock.Setup(service => service.NavigateBackAsync(It.IsAny<CreationContext>(), CancellationToken.None))
            .Throws(() => new Exception(reason));

        var sut = new UnitTestNavigationCommandHandlerBuilder()
            .With(p => p.Ctor.navigationService.NavigateBackAsync.Value(async (_, _) => throw new Exception(reason)))
            .Build(out var context);

        var command = new ItemBuilder<UnitTestNavigationCommand>()
            .Build();

        // act
        await sut.HandleAsync(command);

        // assert
        context.Verify(p => p.Ctor.eventBus.PublishAsyncTEvent)
            .WhereEIs<UnitTestNavigationFailedEvent>(e => ((UnitTestNavigationFailedEvent)e).Reason == reason)
            .Called(1);
    }

    [Test]
    public async Task When_NavigateBack_and_Navigate_fails_report_to_user()
    {
        // arrange
        var result = Result.FailureAsync<Unit, Failure>(new Failure(""));

        var sut = new UnitTestNavigationCommandHandlerBuilder()
            .With(p => p.Ctor.navigationService.NavigateAsync.Value(result))
            .With(p => p.Ctor.navigationService.NavigateBackAsync.Value(result))
            .Build(out var context);

        var command = new ItemBuilder<UnitTestNavigationCommand>()
            .Build();

        // act
        await sut.HandleAsync(command);

        // assert
        context.Verify(p => p.Ctor.userFeedbackService.ShowMessageBoxAsync)
            .Called(1);
    }
}
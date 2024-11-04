using System;
using System.Threading.Tasks;

using NUnit.Framework;

using Twizzar.Design.Core.Command;
using Twizzar.Design.CoreInterfaces.Command.Commands;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.SharedKernel.NLog.Logging;
using Twizzar.TestCommon;

using TwizzarInternal.Fixture;

using SutContext = TwizzarInternal.Fixture.IItemContext<Twizzar.Design.Core.Command.CreateUnitTestCommandHandler, Twizzar.Design.Core.Tests.Command.CreateUnitTestCommandHandlerBuilderPaths>;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Twizzar.Design.Core.Tests.Command;

[TestFixture, TestOf(typeof(CreateUnitTestCommandHandler))]
public partial class CreateUnitTestCommandHandlerTests
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
        Verify.Ctor<CreateUnitTestCommandHandler>()
            .ShouldThrowArgumentNullException();
    }

    private static async Task<SutContext> ArrangeAndActAsync()
    {
        var sut = new CreateUnitTestCommandHandlerBuilder()
            .Build(out var context);

        var command = new ItemBuilder<CreateUnitTestCommand>()
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
    public async Task MapAsync_was_called()
    {
        // arrange & act
        var context = await ArrangeAndActAsync();

        // assert
        context.Verify(p => p.Ctor.mappingService.MapAsync)
            .Called(1);
    }

    [Test]
    public async Task GetOrCreateDocumentAsync_was_called()
    {
        // arrange & act
        var context = await ArrangeAndActAsync();

        // assert
        context.Verify(p => p.Ctor.documentQuery.GetOrCreateDocumentAsync)
            .Called(1);
    }

    [Test]
    public async Task AddTemplate_was_called()
    {
        // arrange & act
        var context = await ArrangeAndActAsync();

        // assert
        context.Verify(p => p.Ctor.templateService.AddTemplate)
            .Called(1);
    }

    [Test]
    public async Task TryCreateContentAsync_was_called()
    {
        // arrange & act
        var context = await ArrangeAndActAsync();

        // assert
        context.Verify(p => p.Ctor.documentContentCreationService.CreateContentAsync)
            .Called(1);
    }

    [Test]
    public async Task OnSuccess_publish_UnitTestCreatedEvent()
    {
        // arrange & act
        var context = await ArrangeAndActAsync();

        // assert
        context.Verify(p => p.Ctor.eventBus.PublishAsyncTEvent)
            .WhereEIs(new UnitTestCreatedEvent())
            .Called(1);
    }

    [Test]
    public async Task OnFailure_publish_UnitTestCreateFailedEvent()
    {
        // arrange
        var reason = TestHelper.RandomString();

        var sut = new CreateUnitTestCommandHandlerBuilder()
            .With(p => p.Ctor.projectQuery.GetOrCreateProject.Value(async (_, _, _) => throw new Exception(reason)))
            .Build(out var context);

        var command = new ItemBuilder<CreateUnitTestCommand>()
            .Build();

        // act
        await sut.HandleAsync(command);

        // assert
        context.Verify(p => p.Ctor.eventBus.PublishAsyncTEvent)
            .WhereEIs<UnitTestCreateFailedEvent>(e => ((UnitTestCreateFailedEvent)e).Reason == reason)
            .Called(1);
    }
}
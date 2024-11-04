using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Design.Core.Command.FixtureItem.Definition;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.FixtureItem.Definition;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.TestCommon;
using ViCommon.Functional;
using ViCommon.Functional.Monads.ResultMonad;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;
using static ViCommon.Functional.Monads.ResultMonad.Result;

namespace Twizzar.Design.Test.UnitTest;

[TestClass]
public class FixtureItemDefinitionRepositoryTest
{
    [TestMethod]
    public async Task Create_fixtureItem_when_not_exists()
    {
        // arrange
        var id = TestHelper.RandomNamedFixtureItemId();

        var node = new Mock<IFixtureItemDefinitionNode>();
        node.Setup(
                definitionNode => definitionNode.Replay(It.IsAny<IEnumerable<IFixtureItemEvent>>()))
            .Returns(Success<Failure>());

        var eventStore = new Mock<IEventStore>();
        eventStore.Setup(store => store.FindLast<FixtureItemCreatedEvent>(id))
            .Returns(Task.FromResult(None<FixtureItemCreatedEvent>()));

        var fixtureItemDefQuery = new Mock<IFixtureItemDefinitionQuery>();
        fixtureItemDefQuery.Setup(query => query.GetDefinitionNode(id))
            .Returns(Task.FromResult(Success<IFixtureItemDefinitionNode, Failure>(node.Object)));

        var sut = new FixtureItemDefinitionRepository(eventStore.Object, fixtureItemDefQuery.Object);

        // act
        var result = await sut.CreateFixtureItem(id);

        // assert
        var definitionNode = TestHelper.AssertAndUnwrapSuccess(result);
        definitionNode.Should().Be(node.Object);
        node.Verify(n => n.CreateNamedFixtureItem(), Times.Once);
    }

    [TestMethod]
    public async Task Get_fixtureItem_when_exists()
    {
        // arrange
        var id = TestHelper.RandomNamedFixtureItemId();
        var node = new Mock<IFixtureItemDefinitionNode>();
        var e = new FixtureItemCreatedEvent(id);

        var eventStore = new Mock<IEventStore>();
        eventStore.Setup(store => store.FindLast<FixtureItemCreatedEvent>(id))
            .Returns(Task.FromResult(Some(e)));

        var fixtureItemDefQuery = new Mock<IFixtureItemDefinitionQuery>();
        fixtureItemDefQuery.Setup(query => query.GetDefinitionNode(id))
            .Returns(Task.FromResult(Success<IFixtureItemDefinitionNode, Failure>(node.Object)));

        var sut = new FixtureItemDefinitionRepository(eventStore.Object, fixtureItemDefQuery.Object);

        // act
        var result = await sut.CreateFixtureItem(id);

        // assert
        result.IsSuccess.Should().BeFalse();
    }

    [TestMethod]
    public async Task RestoreDefinitionNode_calls_replay_on_node()
    {
        // arrange
        var id = TestHelper.RandomNamedFixtureItemId();
        var node = new Mock<IFixtureItemDefinitionNode>();
        node.Setup(
                definitionNode => definitionNode.Replay(It.IsAny<IEnumerable<IFixtureItemEvent>>()))
            .Returns(Result.Success<Unit, Failure>(Unit.New));
        var e = new FixtureItemCreatedEvent(id);

        var eventStore = new Mock<IEventStore>();
        eventStore.Setup(store => store.FindAll(id))
            .Returns(Task.FromResult<IEnumerable<IFixtureItemEvent>>(new List<IFixtureItemEvent>()
            {
                e,
            }));

        var fixtureItemDefQuery = new Mock<IFixtureItemDefinitionQuery>();
        fixtureItemDefQuery.Setup(query => query.GetDefinitionNode(id))
            .Returns(Task.FromResult(Success<IFixtureItemDefinitionNode, Failure>(node.Object)));

        var sut = new FixtureItemDefinitionRepository(eventStore.Object, fixtureItemDefQuery.Object);

        // act
        var result = await sut.RestoreDefinitionNode(id);

        // assert
        var unpacked = TestHelper.AssertAndUnwrapSuccess(result);
        unpacked.Should().Be(node.Object);
        node.Verify(definitionNode => definitionNode.Replay(
                It.Is<IEnumerable<IFixtureItemEvent>>(es => es.Count() == 1)),
            Times.Once);
    }

    [TestMethod]
    public async Task FixtureItemExitsInEventStore_returns_true_when_a_FixtureItemCreatedEvent_exists()
    {
        // arrange
        var id = TestHelper.RandomNamedFixtureItemId();
        var e = new FixtureItemCreatedEvent(id);

        var eventStore = new Mock<IEventStore>();
        eventStore.Setup(store => store.FindLast<FixtureItemCreatedEvent>(id))
            .Returns(Task.FromResult(Some(e)));

        var fixtureItemDefQuery = new Mock<IFixtureItemDefinitionQuery>();

        var sut = new FixtureItemDefinitionRepository(eventStore.Object, fixtureItemDefQuery.Object);

        // act
        var result = await sut.FixtureItemExitsInEventStore(id);

        // assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task FixtureItemExitsInEventStore_returns_false_when_no_FixtureItemCreatedEvent_exists()
    {
        // arrange
        var id = TestHelper.RandomNamedFixtureItemId();

        var eventStore = new Mock<IEventStore>();
        eventStore.Setup(store => store.FindLast<FixtureItemCreatedEvent>(id))
            .Returns(Task.FromResult(None<FixtureItemCreatedEvent>()));

        var fixtureItemDefQuery = new Mock<IFixtureItemDefinitionQuery>();

        var sut = new FixtureItemDefinitionRepository(eventStore.Object, fixtureItemDefQuery.Object);

        // act
        var result = await sut.FixtureItemExitsInEventStore(id);

        // assert
        result.Should().BeFalse();
    }
}
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.CoreInterfaces.Query.Services.ReadModel;
using Twizzar.Design.Test.Builder;
using Twizzar.Design.Ui.Interfaces.Factories;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.Queries;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.FixtureInformations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.FixtureConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

using TwizzarInternal.Fixture;

using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

using static Twizzar.TestCommon.TestHelper;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;
using static ViCommon.Functional.Monads.ResultMonad.Result;

namespace Twizzar.Design.Test.UnitTest;

[TestClass]
public class FixtureItemNodeViewModelQueryTests
{
    private static readonly IConfigurationSource Source = Mock.Of<IConfigurationSource>();


    [TestMethod]
    public async Task Requesting_baseType_calls_query_and_factory_and_returns_baseTypeViewModel()
    {
        // arrange
        var id = RandomNamedFixtureItemId();
        var memberName = RandomString();
        var value = RandomString();
        var typeDescription = new Mock<ITypeDescription>();

        var baseTypeReadModel = new BaseTypeFixtureItemModel(
            id,
            ImmutableDictionary<string, IFixtureConfiguration>.Empty,
            new ValueMemberConfiguration(memberName, value, Source),
            typeDescription.Object);

        var viewModel = new Mock<IFixtureItemNodeViewModel>().Object;

        var fixtureItemNodeFactory = new Mock<IFixtureItemNodeFactory>();
        fixtureItemNodeFactory
            .Setup(
                factory => factory.CreateViewModelNode(
                    It.IsAny<NodeId>(),
                    It.IsAny<Func<IFixtureItemNode, IFixtureItemNodeReceiver>>(),
                    It.IsAny<IFixtureItemNodeFactory.CreateChildrenFactory>(),
                    It.IsAny<IFixtureItemNodeSender>(),
                    It.IsAny<IFixtureItemInformation>(),
                    It.IsAny<IFixtureItemNodeValueViewModel>(),
                    It.IsAny<Maybe<IFixtureItemNode>>()
                ))
            .Returns(viewModel);

        var readModelQuery = new Mock<IFixtureItemReadModelQuery>();
        readModelQuery.Setup(query => query.GetFixtureItem(id))
            .Returns(SuccessAsync<IFixtureItemModel, Failure>(baseTypeReadModel));

        var valueReadModelFactory = new Mock<IFixtureItemValueViewModelFactory>();

        var sut = new FixtureItemNodeViewModelQuery(
            fixtureItemNodeFactory.Object,
            readModelQuery.Object,
            valueReadModelFactory.Object);

        // act
        var result = await sut.GetFixtureItemNodeViewModels(
            id,
            None(),
            None(),
            new ItemBuilder<ICompilationTypeQuery>().Build());

        // assert
        var nodes = AssertAndUnwrapSuccess(result);
        nodes.Should().HaveCount(1);
        nodes.Should().Contain(viewModel);
        readModelQuery.Verify(query => query.GetFixtureItem(id), Times.Once);
    }

    [TestMethod]
    public async Task FailureInReadModelQueryReturnsTheSameFailure()
    {
        // arrange
        var id = RandomNamedFixtureItemId();
        var inputFailure = new Failure(RandomString());
        var expectedResult = Failure<IFixtureItemModel, Failure>(inputFailure);

        var readModelQuery = new Mock<IFixtureItemReadModelQuery>();
        readModelQuery.Setup(query => query.GetFixtureItem(id))
            .Returns(Task.FromResult(expectedResult));

        var fixtureItemNodeFactory = new Mock<IFixtureItemNodeFactory>();

        var valueReadModelFactory = new Mock<IFixtureItemValueViewModelFactory>();

        var sut = new FixtureItemNodeViewModelQuery(
            fixtureItemNodeFactory.Object,
            readModelQuery.Object,
            valueReadModelFactory.Object);

        // act
        var result = await sut.GetFixtureItemNodeViewModels(
            id,
            None(),
            None(),
            new ItemBuilder<ICompilationTypeQuery>().Build());

        // assert
        var failure = result.AsResultValue().Should().BeAssignableTo<FailureValue<Failure>>().Subject;
        failure.Value.Message.Should().Be(inputFailure.Message);
    }

    [TestMethod]
    public async Task UnknowFixtureItemModelInReadModelQueryThrowsArgumentOutOfRangeExceptiont()
    {
        // arrange
        var id = RandomNamedFixtureItemId();

        var readModelQuery = new Mock<IFixtureItemReadModelQuery>();
        readModelQuery.Setup(query => query.GetFixtureItem(id))
            .Returns(
                SuccessAsync<IFixtureItemModel, Failure>(
                    new Mock<IFixtureItemModel>().Object));

        var fixtureItemNodeFactory = new Mock<IFixtureItemNodeFactory>();
        var valueReadModelFactory = new Mock<IFixtureItemValueViewModelFactory>();

        var sut = new FixtureItemNodeViewModelQuery(
            fixtureItemNodeFactory.Object,
            readModelQuery.Object,
            valueReadModelFactory.Object);

        // act
        Func<Task> func = () => sut.GetFixtureItemNodeViewModels(
            id,
            None(),
            None(),
            new ItemBuilder<ICompilationTypeQuery>().Build());

        // assert
        await func.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public async Task ObjectReadModelWithNoMembersReturnsAEmptySequence()
    {
        // arrange
        var id = RandomNamedFixtureItemId();

        var readModel = new ObjectFixtureItemModelBuilder()
            .WithId(id)
            .Build();

        var readModelQuery = new Mock<IFixtureItemReadModelQuery>();
        readModelQuery.Setup(query => query.GetFixtureItem(id))
            .Returns(Task.FromResult(Success<IFixtureItemModel, Failure>(readModel)));

        var fixtureItemNodeFactory = new Mock<IFixtureItemNodeFactory>();
        var valueReadModelFactory = new Mock<IFixtureItemValueViewModelFactory>();

        var sut = new FixtureItemNodeViewModelQuery(
            fixtureItemNodeFactory.Object,
            readModelQuery.Object,
            valueReadModelFactory.Object);

        // act
        var result = await sut.GetFixtureItemNodeViewModels(
            id,
            None(),
            None(),
            new ItemBuilder<ICompilationTypeQuery>().Build());

        // assert
        var nodes = AssertAndUnwrapSuccess(result);
        nodes.Should().HaveCount(0);
    }

    [TestMethod]
    public async Task FixtureItemNodeFactoryIsCalledOnceForUsedConstructor()
    {
        // arrange
        var id = RandomNamedFixtureItemId();

        var readModel = new ObjectFixtureItemModelBuilder()
            .WithId(id)
            .WithUsedConstructor(new FixtureItemConstructorModelBuilder().Build())
            .Build();

        var readModelQuery = new Mock<IFixtureItemReadModelQuery>();
        readModelQuery.Setup(query => query.GetFixtureItem(id))
            .Returns(Task.FromResult(Success<IFixtureItemModel, Failure>(readModel)));

        var fixtureItemNodeFactory = new Mock<IFixtureItemNodeFactory>();

        var valueReadModelFactory = new Mock<IFixtureItemValueViewModelFactory>();

        var sut = new FixtureItemNodeViewModelQuery(
            fixtureItemNodeFactory.Object,
            readModelQuery.Object,
            valueReadModelFactory.Object);

        // act
        var result = await sut.GetFixtureItemNodeViewModels(
            id,
            None(),
            None(),
            new ItemBuilder<ICompilationTypeQuery>().Build());

        // assert
        var nodes = AssertAndUnwrapSuccess(result);
        nodes.Should().HaveCount(1);
        fixtureItemNodeFactory.Verify(
            factory => factory.CreateViewModelNode(
                It.IsAny<NodeId>(),
                null,
                It.IsAny<IFixtureItemNodeFactory.CreateChildrenFactory>(),
                null,
                It.Is<FixtureItemInformation>(information => information.Id == id),
                null,
                None()),
            Times.Once);
    }

    [TestMethod]
    public async Task FixtureItemNodeFactoryIsCalledNTimesForPropertiesAndFields()
    {
        // arrange
        var id = RandomNamedFixtureItemId();
        var nProperties = RandomInt(1, 10);
        var nFields = RandomInt(1, 10);

        static ImmutableDictionary<string, FixtureItemMemberModel> CreateRandomMemberModel(int count) =>
            Enumerable.Range(0, count)
                .Select(i => new FixtureItemMemberModelBuilder().Build())
                .ToImmutableDictionary(model => RandomString(), model => model);

        var readModel = new ObjectFixtureItemModelBuilder()
            .WithId(id)
            .WithProperties(CreateRandomMemberModel(nProperties))
            .WithFields(CreateRandomMemberModel(nFields))
            .Build();

        var readModelQuery = new Mock<IFixtureItemReadModelQuery>();
        readModelQuery.Setup(query => query.GetFixtureItem(id))
            .Returns(Task.FromResult(Success<IFixtureItemModel, Failure>(readModel)));

        var fixtureItemNodeFactory = new Mock<IFixtureItemNodeFactory>();

        var valueReadModelFactory = new Mock<IFixtureItemValueViewModelFactory>();

        var sut = new FixtureItemNodeViewModelQuery(
            fixtureItemNodeFactory.Object,
            readModelQuery.Object,
            valueReadModelFactory.Object);

        // act
        var result = await sut.GetFixtureItemNodeViewModels(
            id,
            None(),
            None(),
            new ItemBuilder<ICompilationTypeQuery>().Build());

        // assert
        var nodes = AssertAndUnwrapSuccess(result);
        nodes.Should().HaveCount(nFields + nProperties);
        fixtureItemNodeFactory.Verify(
            factory => factory.CreateViewModelNode(
                It.IsAny<NodeId>(),
                null,
                It.IsAny<IFixtureItemNodeFactory.CreateChildrenFactory>(),
                null,
                It.Is<FixtureItemInformation>(information => information.Id == id),
                null,
                None()),
            Times.Exactly(nFields + nProperties));
    }
}
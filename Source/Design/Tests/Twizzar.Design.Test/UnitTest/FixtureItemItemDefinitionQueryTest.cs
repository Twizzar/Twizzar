using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using Twizzar.Design.Core.Command.FixtureItem.Definition;
using Twizzar.Design.CoreInterfaces.Command.FixtureItem.Definition;
using Twizzar.Design.CoreInterfaces.Common.FixtureItem.Description.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

using ViCommon.Functional.Monads.ResultMonad;

using static Twizzar.TestCommon.TestHelper;
using static ViCommon.Functional.Monads.ResultMonad.Result;

namespace Twizzar.Design.Test.UnitTest;

[TestClass]
public class FixtureItemItemDefinitionQueryTest
{
    [TestMethod]
    public async Task GetDefinitionNode_uses_descriptionQuery_and_nodeFactory_correctly()
    {
        // arrange
        var id = RandomNamedFixtureItemId();
        var descriptionQuery = new Mock<ITypeDescriptionQuery>();
        var nodeFactory = new Mock<IFixtureDefinitionNodeFactory>();

        var description = new Mock<ITypeDescription>().Object;
        var expectedNode = new Mock<IFixtureItemDefinitionNode>().Object;

        descriptionQuery.Setup(query => query.GetTypeDescriptionAsync(id.TypeFullName, id.RootItemPath))
            .Returns(SuccessAsync<ITypeDescription, Failure>(description));

        nodeFactory.Setup(factory => factory.Create(id, description))
            .Returns(expectedNode);

        var sut = new FixtureItemDefinitionQuery(descriptionQuery.Object, nodeFactory.Object);

        // act
        var result = await sut.GetDefinitionNode(id);

        // assert
        var node = AssertAndUnwrapSuccess(result);
        nodeFactory.Verify(factory => factory.Create(id, description), Times.Once);
        node.Should().Be(expectedNode);
    }
}
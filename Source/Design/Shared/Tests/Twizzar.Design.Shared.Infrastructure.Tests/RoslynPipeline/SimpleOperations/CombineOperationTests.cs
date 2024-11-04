using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations;

namespace Twizzar.Design.Shared.Infrastructure.Tests.RoslynPipeline.SimpleOperations;

[TestFixture]
public class CombineOperationTests
{
    [Test]
    public void Test_eval()
    {
        // arrange
        var leftOperation = Mock.Of<ISimpleValueOperation<int>>(
            operation =>
                operation.Evaluate() == 1);

        var rightOperation = Mock.Of<ISimpleValueOperation<int>>(
            operation =>
                operation.Evaluate() == 2);

        var sut = new CombineOperation<int, int>(leftOperation, rightOperation);

        // act
        var (left, right) = sut.Evaluate();

        // assert
        left.Should().Be(1);
        right.Should().Be(2);
    }
}
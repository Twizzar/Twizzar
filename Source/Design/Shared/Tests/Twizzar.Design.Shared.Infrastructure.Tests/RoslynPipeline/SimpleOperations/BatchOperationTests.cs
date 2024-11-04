using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations;
using Twizzar.TestCommon;

namespace Twizzar.Design.Shared.Infrastructure.Tests.RoslynPipeline.SimpleOperations;

[TestFixture]
public partial class BatchOperationTests
{
    [Test]
    public void Test_evaluation()
    {
        // arrange
        var right = TestHelper.RandomInt();

        var leftOperation = Mock.Of<ISimpleValuesOperation<int>>(
            operation =>
                operation.Evaluate() == Enumerable.Range(0, 10));
        
        var rightOperation = Mock.Of<ISimpleValueOperation<int>>(
            operation =>
                operation.Evaluate() == right);

        var batchOperation = new BatchOperation<int, int>(leftOperation, rightOperation);

        // act
        var valueTuples = batchOperation.Evaluate().ToList();

        // assert
        valueTuples.Should().HaveCount(10);

        for (int i = 0; i < 10; i++)
        {
            valueTuples[i].Left.Should().Be(i);
            valueTuples[i].Right.Should().Be(right);
        }
    }
}
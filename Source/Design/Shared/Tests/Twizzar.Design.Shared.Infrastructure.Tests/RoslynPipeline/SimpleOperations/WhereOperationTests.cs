using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations;

namespace Twizzar.Design.Shared.Infrastructure.Tests.RoslynPipeline.SimpleOperations;

[TestFixture]
public class WhereOperationTests
{
    [Test]
    public void Test_eval()
    {
        // arrange
        var input = Mock.Of<ISimpleValuesOperation<int>>(
            operation =>
                operation.Evaluate() == Enumerable.Range(0, 10));

        var sut = new WhereOperation<int>(i => i % 2 == 0, input);

        // act
        var result = sut.Evaluate();

        // assert
        result.Should().OnlyContain(i => i % 2 == 0);
    }
}
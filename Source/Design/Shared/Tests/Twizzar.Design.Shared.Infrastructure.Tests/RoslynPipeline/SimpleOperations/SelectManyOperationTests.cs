using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations;

namespace Twizzar.Design.Shared.Infrastructure.Tests.RoslynPipeline.SimpleOperations;

[TestFixture]
public class SelectManyOperationTests
{
    [Test]
    public void Test_eval()
    {
        // arrange
        var input = Mock.Of<ISimpleValuesOperation<int>>(
            operation =>
                operation.Evaluate() == Enumerable.Range(0, 3));

        var sut = new SelectManyOperation<int, int>((i, token) => Enumerable.Repeat(i, 2).ToImmutableArray(), input);

        // act
        var result = sut.Evaluate().ToList();

        // assert
        result.Should().HaveCount(6);
        result[0].Should().Be(0);
        result[1].Should().Be(0);
        result[2].Should().Be(1);
        result[3].Should().Be(1);
        result[4].Should().Be(2);
        result[5].Should().Be(2);
    }
}
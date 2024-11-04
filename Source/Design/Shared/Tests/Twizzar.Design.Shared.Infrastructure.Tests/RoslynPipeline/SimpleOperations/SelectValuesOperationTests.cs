using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations;

namespace Twizzar.Design.Shared.Infrastructure.Tests.RoslynPipeline.SimpleOperations;

[TestFixture]
public class SelectValuesOperationTests
{
    [Test]
    public void Test_eval()
    {
        // arrange
        var expectedResults = Enumerable.Range(0, 3);

        var input = Mock.Of<ISimpleValuesOperation<int>>(operation => operation.Evaluate() == expectedResults);

        var sut = new SelectValuesOperation<int, int>((source, token) => source * 2, input);

        // act
        var result = sut.Evaluate();

        // assert
        result.SequenceEqual(expectedResults.Select(i => i * 2)).Should().BeTrue();
    }
}
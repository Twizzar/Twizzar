using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations;

namespace Twizzar.Design.Shared.Infrastructure.Tests.RoslynPipeline.SimpleOperations;

[TestFixture]
public class SelectValueOperationTests
{
    [Test]
    public void test_eval()
    {
        // arrange
        var input = Mock.Of<ISimpleValueOperation<int>>(operation => 
            operation.Evaluate() == 1);

        var sut = new SelectValueOperation<int, int>((source, token) => source * 2, input);

        // act
        var i = sut.Evaluate();

        // assert
        i.Should().Be(2);
    }
}
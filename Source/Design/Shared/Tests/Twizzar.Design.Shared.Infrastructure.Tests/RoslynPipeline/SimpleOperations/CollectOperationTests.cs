using System.Linq;
using Moq;
using NUnit.Framework;
using Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations;

namespace Twizzar.Design.Shared.Infrastructure.Tests.RoslynPipeline.SimpleOperations;

[TestFixture]
public class CollectOperationTests
{
    [Test]
    public void Test_eval()
    {
        // arrange
        var expectedSequence = Enumerable.Range(0, 10);

        var input = Mock.Of<ISimpleValuesOperation<int>>(operation => 
            operation.Evaluate() == expectedSequence);

        var sut = new CollectOperation<int>(input);

        // act
        var immutableArray = sut.Evaluate();

        // assert
        immutableArray.SequenceEqual(expectedSequence);
    }
}
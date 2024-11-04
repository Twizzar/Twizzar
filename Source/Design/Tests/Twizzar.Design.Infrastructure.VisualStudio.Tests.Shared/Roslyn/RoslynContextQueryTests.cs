using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.Fixture;
using Twizzar.TestCommon;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Roslyn;

[TestFixture]
public class RoslynContextQueryTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<RoslynContextQuery>()
            .SetupParameter("workspace", new RoslynWorkspaceBuilder().Build())
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public async Task Context_is_returned_successfully()
    {
        // arrange
        const string filePath = "testfile.cs";

        var workspace = new RoslynWorkspaceBuilder()
            .AddDocument(filePath, "")
            .Build();

        var sut = new RoslynContextQuery(workspace);

        // act
        var result = await sut.GetContextAsync(filePath);

        // assert
        var context = TestHelper.AssertAndUnwrapSuccess(result);
        context.Document.FilePath.Should().Be(filePath);
    }
        
}
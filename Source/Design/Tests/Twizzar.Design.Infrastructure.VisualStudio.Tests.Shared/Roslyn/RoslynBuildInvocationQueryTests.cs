using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.Design.Shared.Infrastructure;
using Twizzar.TestCommon;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Roslyn;

[TestFixture]
public class RoslynBuildInvocationQueryTests
{
    private const string Code = @$"
namespace {ApiNames.ProjectStatisticsNameSpace}
{{
    public static class {ApiNames.ProjectStatisticsTypeName}
    {{
        public const int {ApiNames.BuilderInvocationCountMemberName} = <count>;
    }}
}}";

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(-1)]
    public async Task BuildInvocationCount_is_read_correctly(int expectedCount)
    {
        // arrange
        var code = Code.Replace("<count>", expectedCount.ToString());
        Console.WriteLine(code);

        var workspace = new RoslynWorkspaceBuilder()
            .AddReference(typeof(int).Assembly.Location)
            .AddDocument("ProjectStats.cs", code)
            .Build();

        var sut = new RoslynBuildInvocationQuery(workspace);

        // act
        var buildInvocationCount =
            await sut.GetBuildInvocationCountAsync(RoslynWorkspaceBuilder.ProjectName, CancellationToken.None);

        // assert
        var count = TestHelper.AssertAndUnwrapSuccess(buildInvocationCount);
        count.Should().Be(expectedCount);
    }

    [Test]
    public async Task Failure_is_returned_when_project_does_not_exists()
    {
        // arrange
        var workspace = new RoslynWorkspaceBuilder().Build();

        var sut = new RoslynBuildInvocationQuery(workspace);

        // act
        var buildInvocationCount = await sut.GetBuildInvocationCountAsync("DoesNotExist", CancellationToken.None);

        buildInvocationCount.IsFailure.Should().BeTrue();
        buildInvocationCount.GetFailureUnsafe().Message.Should().Contain("project");
    }

    [Test]
    public void Throw_when_cancellation_is_requested()
    {
        // arrange
        var workspace = new RoslynWorkspaceBuilder().Build();

        var sut = new RoslynBuildInvocationQuery(workspace);

        var source = new CancellationTokenSource();
        source.Cancel();

        // act
        var func = () => sut.GetBuildInvocationCountAsync("DoesNotExist", source.Token);

        func.Should().Throw<OperationCanceledException>();
    }

    [Test]
    public async Task When_member_not_exists_return_failure()
    {
        // arrange
        var code = Code
            .Replace(ApiNames.BuilderInvocationCountMemberName, "MyField")
            .Replace("<count>", "0");
        Console.WriteLine(code);

        var workspace = new RoslynWorkspaceBuilder()
            .AddReference(typeof(int).Assembly.Location)
            .AddDocument("ProjectStats.cs", code)
            .Build();

        var sut = new RoslynBuildInvocationQuery(workspace);

        // act
        var buildInvocationCount = await sut.GetBuildInvocationCountAsync(RoslynWorkspaceBuilder.ProjectName, CancellationToken.None);

        // assert
        buildInvocationCount.IsFailure.Should().BeTrue();
        buildInvocationCount.GetFailureUnsafe().Message.Should().Contain("member");
    }

    [Test]
    public async Task When_member_not_of_type_int_return_failure()
    {
        // arrange
        var code = Code
            .Replace("int", "float")
            .Replace("<count>", "0.0");
        Console.WriteLine(code);

        var workspace = new RoslynWorkspaceBuilder()
            .AddReference(typeof(int).Assembly.Location)
            .AddDocument("ProjectStats.cs", code)
            .Build();

        var sut = new RoslynBuildInvocationQuery(workspace);

        // act
        var buildInvocationCount = await sut.GetBuildInvocationCountAsync(RoslynWorkspaceBuilder.ProjectName, CancellationToken.None);

        // assert
        buildInvocationCount.IsFailure.Should().BeTrue();
        buildInvocationCount.GetFailureUnsafe().Message.Should().Contain("int");
    }

    [Test]
    public async Task When_member_not_a_field_return_failure()
    {
        // arrange
        var code = Code
            .Replace(ApiNames.BuilderInvocationCountMemberName, $"{ApiNames.BuilderInvocationCountMemberName} {{get;}}")
            .Replace("<count>", "0.0");
        Console.WriteLine(code);

        var workspace = new RoslynWorkspaceBuilder()
            .AddReference(typeof(int).Assembly.Location)
            .AddDocument("ProjectStats.cs", code)
            .Build();

        var sut = new RoslynBuildInvocationQuery(workspace);

        // act
        var buildInvocationCount = await sut.GetBuildInvocationCountAsync(RoslynWorkspaceBuilder.ProjectName, CancellationToken.None);

        // assert
        buildInvocationCount.IsFailure.Should().BeTrue();
        buildInvocationCount.GetFailureUnsafe().Message.Should().Contain("field");
    }
}
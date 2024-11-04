using System.Collections.Generic;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Roslyn;

[TestFixture]
public class RoslynSolutionExtensionsTests
{
    private Workspace _workspace;

    [SetUp]
    public void SetUp()
    {
        var projectNames = new List<string>
        {
            "TestProject",
            "TestProject1",
            "TestProject22",
            "TestProject21",
            "TestProject3",
        };

        projectNames.Reverse();

        var workspaceBuilder = new RoslynWorkspaceBuilder();
        projectNames.ForEach(s => workspaceBuilder.AddProject(s, s));
        this._workspace = workspaceBuilder.Build();
    }

    [Test]
    public void Exact_match_finds_the_correct_project()
    {
        // arrange
        var expectedName = "TestProject";
        var solution = this._workspace.CurrentSolution;
            
        // act
        var project = solution.GetProjectByName(expectedName);

        // assert
        var someProject = project.AsMaybeValue()
            .Should()
            .BeAssignableTo<SomeValue<Project>>()
            .Subject;
        someProject.Value.Name.Should().Be(expectedName);
    }

    [Test]
    public void Prefix_match_find_first_matching_project()
    {
        // arrange
        var expectedName = "TestProject21";
        var solution = this._workspace.CurrentSolution;
            
        // act
        var project = solution.GetProjectByName("TestProject2");

        // assert
        var someProject = project.AsMaybeValue()
            .Should()
            .BeAssignableTo<SomeValue<Project>>()
            .Subject;
        someProject.Value.Name.Should().Be(expectedName);
    }
}
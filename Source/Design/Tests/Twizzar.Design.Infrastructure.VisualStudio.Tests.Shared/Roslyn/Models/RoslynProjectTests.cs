using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn.Models;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Roslyn.Models;

[TestFixture]
public class RoslynProjectTests
{
    private string _projectName;
    private string _projectFilePath;
    private Workspace _workspace;
    private Project _project;

    [SetUp]
    public void SetUp()
    {
        this._projectName = TestHelper.RandomString();
        this._projectFilePath = TestHelper.RandomString() + @$"\{this._projectName}";
        this._workspace =
            new RoslynWorkspaceBuilder()
                .AddProject(this._projectName, this._projectName, this._projectFilePath)
                .Build();

        this._project = this._workspace.CurrentSolution.Projects.Last();
    }

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<RoslynProject>()
            .SetupParameter("project", this._project)
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void Name_is_set_correctly()
    {
        // arrange
        var sut = this.GetSut();

        // assert
        sut.Name.Should().Be(this._projectName);
    }

    [Test]
    public void FullName_is_set_correctly()
    {
        // arrange
        var sut = this.GetSut();

        // assert
        sut.FullName.Should().Be(this._projectFilePath);
    }

    [Test]
    public void ProjectDirectory_is_set_correctly()
    {
        // arrange
        var sut = this.GetSut();

        // assert
        sut.ProjectDirectory.Should().Be(this._projectFilePath.Replace(@"\" + this._projectName, string.Empty));
    }

    private RoslynProject GetSut() => new ItemBuilder<RoslynProject>()
        .With(p => p.Ctor.project.Value(this._project))
        .Build();
}
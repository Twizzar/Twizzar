using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Host.Mef;
using Moq;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Common.Messaging.Events;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.Design.Shared.Infrastructure;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;
using Verify = TwizzarInternal.Fixture.Verify;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Roslyn;

[TestFixture]
public class RoslynSolutionEventsTests
{
    #region fields

    private Project _project;
    private TestWorkspace _workspace;
    private Solution _solution;
    private IUiEventHub _uiEventHub;
    private RoslynSolutionEvents _sut;
    private Mock<IUiEventHub> _uiEventHubMock;

    #endregion

    #region members

    [SetUp]
    public void SetUp()
    {
        this._workspace = new TestWorkspace();
        this._solution = this._workspace.CurrentSolution;
        this._project = this._solution.Projects.First();
        this._uiEventHub = Build.New<IUiEventHub>();

        this._sut = new RoslynSolutionEventsBuilder()
            .With(p => p.Ctor.workspace.Value(this._workspace))
            .With(p => p.Ctor.eventHub.Value(this._uiEventHub))
            .Build(out var context);

        this._uiEventHubMock = context.GetAsMoq(p => p.Ctor.eventHub);
        this._sut.Initialize();
    }


    public class RoslynSolutionEventsBuilder : ItemBuilder<RoslynSolutionEvents, RoslynSolutionEventsCustomPaths>
    {

    }

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<RoslynSolutionEvents>()
            .SetupParameter("workspace", this._workspace)
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public async Task Event_is_published_when_project_is_renamed()
    {
        // arrange
        const string newProjectName = "NewName";

        // act
        await this._workspace.RenameProjectAsync(this._project, newProjectName);

        // assert
        this._uiEventHubMock
            .Verify(
                hub =>
                    hub.Publish(It.Is<ProjectRenamedEvent>(e => e.NewProject.Name == newProjectName)),
                Times.Once);
    }

    [Test]
    public async Task Event_is_published_only_once_when_project_is_renamed_to_the_same_name_twice()
    {
        // arrange
        const string newProjectName = "NewName";

        // act
        await this._workspace.RenameProjectAsync(this._project, newProjectName);
        await this._workspace.RenameProjectAsync(this._project, newProjectName);

        // assert
        this._uiEventHubMock
            .Verify(
                hub =>
                    hub.Publish(It.Is<ProjectRenamedEvent>(e => e.NewProject.Name == newProjectName)),
                Times.Once);
    }

    [Test]
    public async Task Removing_a_project_applies_the_change_to_the_internal_cache()
    {
        // arrange
        const string newProjectName = "NewName";

        // act
        var project = await this._workspace.RenameProjectAsync(this._project, newProjectName);
        await this._workspace.RemoveProjectAsync(project);
        project = await this._workspace.AddProject("NewProject");
        await this._workspace.RenameProjectAsync(project, newProjectName);

        // assert
        this._uiEventHubMock
            .Verify(
                hub =>
                    hub.Publish(It.Is<ProjectRenamedEvent>(e => e.NewProject.Name == newProjectName)),
                Times.Exactly(2));
    }

    [Test]
    public async Task When_project_added_TwizzarAnalyzerAddedEvent_will_be_published()
    {
        var analyzer = new Mock<AnalyzerReference>();
        var version = new Version(1, 0, 0, 0);  
        analyzer.Setup(reference => reference.Id)
            .Returns(new AssemblyIdentity("Test", version));

        analyzer.Setup(reference => reference.FullPath)
            .Returns(ApiNames.AnalyzerProjectNamePrefix);

        await this._workspace.AddProject("NewProject", new[] { analyzer.Object });

        // assert
        this._uiEventHubMock.Verify(hub =>
            hub.Publish(It.Is<TwizzarAnalyzerAddedEvent>(e => e.Version == version)));
    }

    [Test]
    public async Task When_project_changed_TwizzarAnalyzerAddedEvent_will_be_published()
    {
        var analyzer = new Mock<AnalyzerReference>();
        var version = new Version(1, 0, 0, 0);
        analyzer.Setup(reference => reference.Id)
            .Returns(new AssemblyIdentity("Test", version));

        analyzer.Setup(reference => reference.FullPath)
            .Returns(ApiNames.AnalyzerProjectNamePrefix);

        var project = await this._workspace.AddProject("NewProject");

        this._workspace.TryApplyChanges(
            this._workspace.CurrentSolution.AddAnalyzerReference(project.Id, analyzer.Object));

        await Task.Delay(50);

        // assert
        this._uiEventHubMock.Verify(hub =>
            hub.Publish(It.Is<TwizzarAnalyzerAddedEvent>(e => e.Version == version)));
    }

    #endregion

    #region Nested type: TestWorkspace

    public class TestWorkspace : Workspace
    {
        #region ctors

        /// <inheritdoc />
        public TestWorkspace()
            : base(MefHostServices.DefaultHost, null)
        {
            var projectName = "TestProject";

            this.ApplyProjectAdded(
                ProjectInfo.Create(
                    ProjectId.CreateNewId(),
                    VersionStamp.Default,
                    projectName,
                    projectName,
                    LanguageNames.CSharp,
                    projectName));
        }

        #endregion

        #region members

        /// <inheritdoc />
        public override bool CanApplyChange(ApplyChangesKind feature) =>
            true;

        public async Task<Project> AddProject(string projectName, IEnumerable<AnalyzerReference> analyzerReferences = null)
        {
            var id = ProjectId.CreateNewId();

            var oldSolution = this.CurrentSolution;

            var newSolution = this.CurrentSolution.AddProject(
                ProjectInfo.Create(
                    id,
                    VersionStamp.Default,
                    projectName,
                    projectName,
                    LanguageNames.CSharp,
                    projectName,
                    analyzerReferences: analyzerReferences ?? Enumerable.Empty<AnalyzerReference>()));

            this.TryApplyChanges(newSolution).Should().BeTrue();
            this.SetCurrentSolution(newSolution);

            var project = this.CurrentSolution.GetProject(id);
            await this.RaiseProjectAddedEventAsync(oldSolution, newSolution, project);

            return project;
        }

        public async Task RemoveProjectAsync(Project project)
        {
            var oldSolution = this.CurrentSolution;
            var newSolution = this.CurrentSolution.RemoveProject(project.Id);
            this.TryApplyChanges(newSolution).Should().BeTrue();
            this.SetCurrentSolution(newSolution);

            await this.RaiseProjectRemovedEventAsync(oldSolution, newSolution, project);
        }

        public async Task<Project> RenameProjectAsync(Project project, string newName)
        {
            var oldSolution = this.CurrentSolution;
            var newSolution = this.CurrentSolution.WithProjectFilePath(project.Id, newName);
            this.TryApplyChanges(newSolution).Should().BeTrue();
            this.SetCurrentSolution(newSolution);

            await this.RaiseProjectRenameEventAsync(oldSolution, newSolution, newSolution.GetProject(project.Id));
            return this.CurrentSolution.GetProject(project.Id);
        }

        public async Task RaiseProjectRenameEventAsync(Solution oldSolution, Solution newSolution, Project project)
        {
            await this.RaiseWorkspaceChangedEventAsync(
                WorkspaceChangeKind.ProjectChanged,
                oldSolution,
                newSolution,
                project.Id);
        }

        public async Task RaiseProjectAddedEventAsync(Solution oldSolution, Solution newSolution, Project project)
        {
            await this.RaiseWorkspaceChangedEventAsync(
                WorkspaceChangeKind.ProjectAdded,
                oldSolution,
                newSolution,
                project.Id);
        }

        public async Task RaiseProjectRemovedEventAsync(Solution oldSolution, Solution newSolution, Project project)
        {
            await this.RaiseWorkspaceChangedEventAsync(
                WorkspaceChangeKind.ProjectRemoved,
                oldSolution,
                newSolution,
                project.Id);
        }

        #endregion
    }

    #endregion
}
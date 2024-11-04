using System.Collections.Immutable;
using System.Threading.Tasks;

using FluentAssertions;

using NUnit.Framework;

using TestCreation.Services;

using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.Design.CoreInterfaces.TestCreation.Config;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation;

using TwizzarInternal.Fixture;

using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.TestCreation.Services;

[TestFixture]
public class ProjectQueryTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<ProjectQuery>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public async Task If_project_exists_return_created_project_and_add_references()
    {
        // arrange
        var project = new IdeProjectBuilder()
            .Build(out var projectContext);

        var sut = new ItemBuilder<ProjectQuery>()
            .With(p => p.Ctor.vsProjectQuery.FindProjectAsync.Value(Maybe.SomeAsync(project)))
            .Build();

        var destination = new ItemBuilder<CreationInfo>().Build();
        var sourceContext = new ItemBuilder<CreationContext>().Build();
        var config = new TestCreationConfigBuilder().Build();

        // act
        var result = await sut.GetOrCreateProject(destination, sourceContext, config);

        // assert
        result.Should().Be(project);

        projectContext.Verify(p => p.AddProjectReferenceAsync)
            .Called(1);
    }

    [Test]
    public async Task If_project_does_not_exist_create_project_and_add_references()
    {
        // arrange
        var project = new IdeProjectBuilder()
            .Build(out var projectContext);

        var destination = new ItemBuilder<CreationInfo>().Build();
        var sourceContext = new ItemBuilder<CreationContext>().Build();

        var sut = new ItemBuilder<ProjectQuery>()
            .With(p => p.Ctor.vsProjectQuery.FindProjectAsync.Value(s =>
                s == sourceContext.Info.Project.SplitPath().FileName
                    ? Maybe.SomeAsync(project)
                    : Maybe.NoneAsync<IIdeProject>()))
            .With(p => p.Ctor.vsProjectFactory.CreateProjectAsync.Value(Task.FromResult(project)))
            .With(p => p.Ctor.feedbackService.ShowYesNoBoxAsync.Value(Task.FromResult(true)))
            .Build();

        var config = new TestCreationConfigBuilder().Build();

        // act
        var result = await sut.GetOrCreateProject(destination, sourceContext, config);

        // assert
        result.Should().Be(project);

        projectContext.Verify(p => p.AddProjectReferenceAsync)
            .Called(1);
    }

    [Test]
    public async Task Additional_nuget_packages_are_installed()
    {
        // arrange
        var project = new IdeProjectBuilder()
            .Build(out var projectContext);

        var sut = new ItemBuilder<ProjectQuery>()
            .With(p => p.Ctor.vsProjectQuery.FindProjectAsync.Value(Maybe.SomeAsync(project)))
            .Build();

        var destination = new ItemBuilder<CreationInfo>().Build();
        var sourceContext = new ItemBuilder<CreationContext>().Build();
        var config = new TestCreationConfigBuilder()
            .With(p => p.Ctor.AdditionalNugetPackages.Value(
                ImmutableArray<(string, Maybe<string>)>.Empty.Add(("SomeNuget", Maybe.Some("1.0")))))
            .Build();

        // act
        var result = await sut.GetOrCreateProject(destination, sourceContext, config);

        // assert
        result.Should().Be(project);

        projectContext.Verify(p => p.AddNugetPackageAsync)
            .WherePackageIdIs("SomeNuget")
            .WhereVersionIs("1.0")
            .Called(1);
    }

    private class IdeProjectBuilder : ItemBuilder<IIdeProject, IdeProjectCustomPaths>
    {
        /// <inheritdoc />
        public IdeProjectBuilder()
        {
            this.With(p => p.IsLoaded.Value(true));
        }
    }

    private class TestCreationConfigBuilder : ItemBuilder<TestCreationConfig, TestCreationConfigCustomPaths>
    {
        public TestCreationConfigBuilder()
        {
            this.With(p => p.Ctor.AdditionalNugetPackages.Value(ImmutableArray<(string, Maybe<string>)>.Empty));
        }
    }
}
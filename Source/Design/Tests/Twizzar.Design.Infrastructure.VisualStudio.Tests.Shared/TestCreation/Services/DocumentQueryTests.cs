using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using TestCreation.Services;
using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using TwizzarInternal.Fixture;
using Assert = NUnit.Framework.Assert;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.TestCreation.Services;

[TestFixture]
public class DocumentQueryTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<MappingService>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void GetOrCreateDocumentAsync_throws_AggregateException_when_destination_is_null()
    {
        var sut = CreateSut(CreateWorkspace());

        Func<Task> action = () => sut.GetOrCreateDocumentAsync(null, null);

        action.Should().ThrowAsync<AggregateException>();
    }

    [Test]
    public void GetOrCreateDocumentAsync_throws_AggregateException_when_sourceContext_is_null()
    {
        var sut = CreateSut(CreateWorkspace());

        var ex = Assert.ThrowsAsync<AggregateException>(
            () => sut.GetOrCreateDocumentAsync(
                CreateDestination(),
                null));

        ex.Should().NotBeNull();
    }

    [Test]
    public async Task If_project_not_found_return_None()
    {
        // arrange
        var sut = CreateSut(CreateWorkspace(withTestProject: false));
        var sourceContext = new ItemBuilder<CreationContext>().Build();

        // act
        var creationContext = await sut.GetOrCreateDocumentAsync(CreateDestination(), sourceContext);

        // assert
        creationContext.IsNone.Should().BeTrue();
    }

    [Test]
    public async Task If_document_not_exists_add_it()
    {
        // arrange
        var workspace = CreateWorkspace();
        var sut = CreateSut(workspace);

        var sourceContext = new ItemBuilder<CreationContext>()
            .With(p => p.Ctor.Info.Value(CreateSource()))
            .Build();

        // act
        var creationContext = await sut.GetOrCreateDocumentAsync(CreateDestination(), sourceContext);

        // assert
        var (_, _, _, codeAnalysisContext, _) = creationContext.GetValueUnsafe();

        var document = codeAnalysisContext.GetDocument();

        var project = workspace.CurrentSolution.Projects.Single(p => p.Name == "DummyTestProject");
        project.Documents.Should().NotContain(document);
        document.Project.Name.Should().Be(project.Name);
    }

    private static Workspace CreateWorkspace(bool withTestProject = true)
    {
        var builder = new RoslynWorkspaceBuilder()
            .AddProject("DummyProject", "DummyProject")
            .AddDocument("DummyFile", "");

        if (withTestProject)
        {
            builder.AddProject("DummyTestProject", "DummyTestProject", "DummyTestProject");
        }

        return builder.Build();
    }

    private static DocumentQuery CreateSut(Workspace workspace) =>
        new ItemBuilder<DocumentQuery>()
            .With(p => p.Ctor.workspace.Value(workspace))
            .Build();

    private static CreationInfo CreateSource() =>
        new(
            "DummySolution",
            "DummyProject",
            "DummyFile",
            "DummyNamespace",
            "DummyType",
            "DummyMember");

    private static CreationInfo CreateDestination() =>
        new(
            "DummyTestSolution",
            "DummyTestProject",
            "DummyTestFile",
            "DummyTestNamespace",
            "DummyTestType",
            "DummyTestMember");
}
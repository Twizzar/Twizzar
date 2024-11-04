using System.Linq;

using FluentAssertions;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;

using Moq;

using NUnit.Framework;

using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment.IntraTextAdornment;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Configs;
using Twizzar.SharedKernel.NLog.Logging;
using Twizzar.TestCommon;

using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.FixtureItem.Adornment.IntraTextAdornment;

[TestFixture]
public class ViDocumentTaggerTests
{
    [SetUp]
    public void SetUp()
    {
        LoggerFactory.SetConfig(new LoggerConfigurationBuilder());
    }

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<ViDocumentTagger>()
            .SetupParameter("workspace", CreateWorkspace())
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void Empty_AdornmentInformation_return_empty_sequence()
    {
        var documentReader = new ItemBuilder<IDocumentReader>()
            .With(p => p.GetAdornmentInformation.Value(Enumerable.Empty<IAdornmentInformation>()))
            .Build();

        var factory = new ItemBuilder<IDocumentWorkspaceFactory>()
            .With(p => p.Create.DocumentReader.Value(documentReader))
            .With(p => p.Create.SnapshotHistory.Stub<ISnapshotHistory>())
            .Build();

        var sut = new ItemBuilder<ViDocumentTagger>()
            .With(p => p.Ctor.workspace.Value(CreateWorkspace()))
            .With(p => p.Ctor.documentWorkspaceFactory.Value(factory))
            .With(p => p.Ctor.documentFilePath.Value("testPath"))
            .Build();

        var tagSpans = sut.GetTags(
            new NormalizedSnapshotSpanCollection(new[] { new EmptyConfigs.DefaultSnapShotConfigBuilder().Build() }));

        tagSpans.Should().HaveCount(0);
    }

    [Test]
    public void DocumentAdornmentController_is_disposed_on_Dispose()
    {
        var documentAdornmentController = Build.New<IDocumentAdornmentController>();

        var factory = new ItemBuilder<IDocumentWorkspaceFactory>()
            .With(p =>
                p.Create.DocumentAdornmentController.Value(
                    documentAdornmentController))
            .Build();

        var sut = new ItemBuilder<ViDocumentTagger>()
                .With(p => p.Ctor.documentWorkspaceFactory.Value(factory))
                .With(p => p.Ctor.workspace.Value(CreateWorkspace()))
                .With(p => p.Ctor.documentFilePath.Value("testPath"))
                .Build();

        sut.Dispose();

        Mock.Get(documentAdornmentController)
            .Verify(controller => controller.Dispose(), Times.Once);
    }

    private static Workspace CreateWorkspace()
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("Test", LanguageNames.CSharp);
        workspace.AddDocument(
            DocumentInfo.Create(DocumentId.CreateNewId(project.Id), "testDoc", filePath: "testPath"));
        return workspace;
    }
}
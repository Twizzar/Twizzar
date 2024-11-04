using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Moq;
using NUnit.Framework;
using TestCreation.Services;
using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.DocumentContent;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.TestCreation.
    Services;

[TestFixture]
public class DocumentContentCreationServiceTests
{
    #region static fields and constants

    private const string CodeWithClassWithoutMethod = @"
using NUnit.Framework;
using Twizzar.Fixture;
using System;
using DemoCode;
namespace DemoCode.Tests
{
[TestFixture]
public class LoggerTests
{
}
}
";

    private const string CodeWithoutClass = @"
using NUnit.Framework;
using Twizzar.Fixture;
using System;
using DemoCode;
namespace DemoCode.Tests
{
}
";

    private const string TotalExpectedCode = @"
using NUnit.Framework;
using Twizzar.Fixture;
using System;
using DemoCode;
namespace DemoCode.Tests
{
[TestFixture]
public class LoggerTests
{
[Test]
public void Log_Scenario_ExpectedBehavior()
{
// Arrange
var sut = new ItemBuilder<Logger>().Build();
var message = new ItemBuilder<String>().Build();
// Act
sut.Log(message);
// Assert
Assert.Fail();
}
}
}
";

    private const string TemplateFileCode = @"
using NUnit.Framework;
using Twizzar.Fixture;
using System;
using DemoCode;
namespace DemoCode.Tests
{
[TestFixture]
public class LoggerTests
{
[Test]
public void Log_Scenario_ExpectedBehavior()
{
// Arrange
var sut = new ItemBuilder<Logger>().Build();
var message = new ItemBuilder<String>().Build();
// Act
sut.Log(message);
// Assert
Assert.Fail();
}
}
}
";

    private const string TemplateMethodCode = @"
[Test]
public void Log_Scenario_ExpectedBehavior()
{
// Arrange
var sut = new ItemBuilder<Logger>().Build();
var message = new ItemBuilder<String>().Build();
// Act
sut.Log(message);
// Assert
Assert.Fail();
}";

    private const string TemplateClassCode = @"
[TestFixture]
public class LoggerTests
{
[Test]
public void Log_Scenario_ExpectedBehavior()
{
// Arrange
var sut = new ItemBuilder<Logger>().Build();
var message = new ItemBuilder<String>().Build();
// Act
sut.Log(message);
// Assert
Assert.Fail();
}
}
";

    private const string EmptyCode = "";

    #endregion

    #region ContentType enum

    public enum ContentType
    {
        Empty,
        WithClass,
        WithoutClass,
    }

    #endregion

    #region members

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<DocumentContentCreationService>()
            .IgnoreParameter("workspace")
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void CreateContentAsync_throws_ArgumentNullException_when_destination_is_null()
    {
        var sut = CreateDocumentContentCreationService(CreateWorkspace(EmptyCode));

        var action = () => sut.CreateContentAsync(null);

        action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    [TestCase(ContentType.Empty)]
    [TestCase(ContentType.WithClass)]
    [TestCase(ContentType.WithoutClass)]
    public async Task CreateContentAsync_should_create_test_method_correctly(ContentType contentType)
    {
        // Arrange

        var existingCode = contentType switch
        {
            ContentType.Empty => EmptyCode,
            ContentType.WithClass => CodeWithClassWithoutMethod,
            ContentType.WithoutClass => CodeWithoutClass,
            _ => throw new ArgumentOutOfRangeException(nameof(contentType), contentType, null),
        };

        var workspace = CreateWorkspace(existingCode);
        var destination = CreateDestination(workspace);
        var sut = CreateDocumentContentCreationService(workspace);

        // Act
        await sut.CreateContentAsync(destination);

        var actual = await GetDocument(workspace).GetTextAsync()
            .Map(text => text.ToString());

        actual = RemoveWhiteSpaces(actual);
        var expected = RemoveWhiteSpaces(TotalExpectedCode);
        actual.Should().Be(expected);
    }

    private static string RemoveWhiteSpaces(string input)
    {
        return string.Concat(input.Where(c => !char.IsWhiteSpace(c)));
    }

    private static Workspace CreateWorkspace(string code) =>
        new RoslynWorkspaceBuilder()
            .AddProject("DummyProject", "DummyProject")
            .AddDocument("DummyFile", code)
            .Build();

    private static DocumentContentCreationService CreateDocumentContentCreationService(Workspace workspace)
    {
        return new ItemBuilder<DocumentContentCreationService>()
            .With(p => p.Ctor.templateCodeProvider.Value(GetTemplateCodeProvider()))
            .With(p => p.Ctor.workspace.Value(workspace))
            .With(p => p.Ctor.updateUsingService.Value(GetUpdateUsingService(workspace)))
            .Build();
    }

    private static IUpdateUsingService GetUpdateUsingService(Workspace workspace)
    {
        var updateUsingServiceMock = new Mock<IUpdateUsingService>();

        updateUsingServiceMock.Setup(service =>
                service.UpdateUsings(It.IsAny<Document>(), It.IsAny<SyntaxNode>(), It.IsAny<ITemplateContext>()))
            .Returns(GetDocument(workspace));

        var updateUsingServic = updateUsingServiceMock.Object;
        return updateUsingServic;
    }

    private static ITemplateCodeProvider GetTemplateCodeProvider()
    {
        var templateCodeProviderMock = new Mock<ITemplateCodeProvider>();

        templateCodeProviderMock
            .Setup(provider => provider.GetCode(SnippetType.File, It.IsAny<ITemplateContext>()))
            .Returns(TemplateFileCode);

        templateCodeProviderMock
            .Setup(provider => provider.GetCode(SnippetType.Class, It.IsAny<ITemplateContext>()))
            .Returns(TemplateClassCode);

        templateCodeProviderMock
            .Setup(provider => provider.GetCode(SnippetType.Method, It.IsAny<ITemplateContext>()))
            .Returns(TemplateMethodCode);

        var templateCodeProvider = templateCodeProviderMock.Object;
        return templateCodeProvider;
    }

    private static CreationContext CreateDestination(Workspace workspace)
    {
        var contextMock = new Mock<ITemplateContext>();
        contextMock.Setup(context => context.File).Returns(Mock.Of<ITemplateFile>());
        var templateContext = contextMock.Object;

        var codeAnalysisContext = new CodeAnalysisContext(GetDocument(workspace));

        return new CreationContext(
            new CreationInfo(
                "DummySolution",
                "DummyProject",
                "DummyFile",
                "DummyNamespace",
                "LoggerTests",
                "DummyMember"),
            Mock.Of<IMemberDescription>(),
            Mock.Of<ITypeDescription>(),
            codeAnalysisContext,
            Maybe.Some(templateContext));
    }

    private static Document GetDocument(Workspace workspace)
    {
        var document = workspace.CurrentSolution.Projects.First().Documents.Last();

        return document;
    }

    #endregion
}
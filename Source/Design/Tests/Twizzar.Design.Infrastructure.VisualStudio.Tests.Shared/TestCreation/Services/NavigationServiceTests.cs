using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using NUnit.Framework;

using TestCreation.Services;

using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.Design.Shared.Infrastructure.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using TwizzarInternal.Fixture;

using ViCommon.Functional.Extensions;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.TestCreation.Services;

[TestFixture]
public class NavigationServiceTests
{
    private const string TestCode = @$"
using System;

public class MyClassTests
{{
    [TestSource(<attributeArgument>)]
    public void MyMethodTest()
    {{
        var sut = new MyClass();
        sut.MyMethod();
    }}
}}

public class TestSourceAttribute : Attribute
{{
    public TestSourceAttribute(string memberName)
    {{
    }}
}}
";

    private const string ProductiveCode = @$"
public class MyClass{{
    public void MyMethod(){{}}
}}
";

    #region members

    [Test]
    public void NavigateAsync_throws_ArgumentNullException_when_destination_is_null()
    {
        var sut = CreateNavigationService(CreateWorkspace());

        var action = () => sut.NavigateAsync(null, CancellationToken.None);

        action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task NavigateAsync_should_navigates_correctly()
    {
        // Arrange
        Document openedDocument = null;

        var workspace = CreateWorkspace();

        workspace.DocumentOpened += (sender, args) => { openedDocument = args.Document; };
        var destination = CreateDestination(workspace);
        var sut = CreateNavigationService(workspace);

        // Act
        await sut.NavigateAsync(destination, CancellationToken.None);

        // Assert
        GetDocument(workspace).Should().Be(openedDocument);
    }

    [TestCase("nameof(MyClass.MyMethod)")]
    [TestCase("\"MyMethod\"")]
    public async Task Navigation_from_TestSource_annotated_method_is_successful(string attributeArgument)
    {
        // arrange
        var workspace = new RoslynWorkspaceBuilder()
            .AddReference(typeof(Attribute).Assembly.Location)
            .AddDocument("ProdDoc", ProductiveCode)
            .AddDocument("TestDoc", TestCode.Replace("<attributeArgument>", attributeArgument))
            .Build();

        var testDocId = workspace.CurrentSolution.GetDocumentIdsWithFilePath("TestDoc").Single();
        var expectedDocId = workspace.CurrentSolution.GetDocumentIdsWithFilePath("ProdDoc").Single();
        var doc = workspace.CurrentSolution.GetDocument(testDocId);

        var semanticModel = await doc.GetSemanticModelAsync();

        var testMethod = await doc.GetSyntaxRootAsync()
            .Map(node => node.DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>().Single())
            .Map(node => semanticModel.GetDeclaredSymbol(node) as IMethodSymbol)
            .Map(symbol =>
            {
                var m = new Mock<IRoslynBaseDescription>();
                m.Setup(x => x.GetSymbol()).Returns(symbol);
                return m.As<IMemberDescription>().Object;
            });

        var sut = CreateNavigationService(workspace);

        var context = new ItemBuilder<CreationContext>()
            .With(p => p.Ctor.SourceMember.Value(testMethod))
            .With(p => p.Ctor.CodeAnalysisContext.Value(new CodeAnalysisContext(doc)))
            .Build();

        // act
        var result = await sut.NavigateBackAsync(context, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        workspace.GetOpenDocumentIds().Should().Contain(expectedDocId);
    }

    private static Workspace CreateWorkspace() =>
        new RoslynWorkspaceBuilder()
            .AddProject("TestProject", "TestProject")
            .AddDocument("TestFile", string.Empty)
            .Build();

    private static NavigationService CreateNavigationService(Workspace workspace) => 
        new ItemBuilder<NavigationService>()
            .With(p => p.Ctor.workspace.Value(workspace))
            .Build();

    private static CreationInfo CreateDestination(Workspace workspace) =>
        new(
            "DummySolution",
            "TestProject",
            "TestFile",
            "DummyNamespace",
            "LoggerTests",
            "DummyMember");

    private static Document GetDocument(Workspace workspace)
    {
        var document = workspace.CurrentSolution.Projects.First().Documents.Last();

        return document;
    }

    #endregion

    private class CreationContextWithPropertyBuilder : ItemBuilder<CreationContext, CreationcontextWithPropertyPaths>
    {
        public CreationContextWithPropertyBuilder()
        {
            var propMock = new Mock<IRoslynBaseDescription>()
                .As<IPropertyDescription>();

            this.With(p => p.Ctor.SourceMember.Value(propMock.Object));
        }
    }
}
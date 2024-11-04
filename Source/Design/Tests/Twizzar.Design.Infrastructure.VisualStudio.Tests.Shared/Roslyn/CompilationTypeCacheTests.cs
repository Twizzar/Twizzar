using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Dummies;
using Twizzar.Design.Infrastructure.VisualStudio2019.Roslyn;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio2019.Tests.Roslyn;

public class CompilationTypeCacheTests
{
    [Test]
    public void Test_constructors()
    {
        Verify.Ctor<CompilationTypeCache>()
            .SetupParameter("workspace", new AdhocWorkspace())
            .ShouldThrowArgumentNullException();
    }

    private Workspace SetupWorkspace()
    {
        var code = @$"
class A {{}}
interface IA {{}}
class B {{}}
interface IB {{}}
";

        var workspace = new RoslynWorkspaceBuilder()
            .AddDocument("test", code)
            .Build();

        return workspace;
    }

    [Test]
    [Ignore("Does not work.")]
    public async Task When_initialized_type_in_code_are_returned()
    {
        var sut = new CompilationTypeCache(
            this.SetupWorkspace(),
            new RoslynDescriptionFactoryDummy());

        await sut.InitializeAsync();
        
        var allTypeForAssembly = sut.GetAllTypeForAssembly(RoslynWorkspaceBuilder.ProjectName);

        allTypeForAssembly.Should().Contain(description => description.TypeFullName.GetTypeName() == "A");
    }

    [Test]
    [Ignore("Does not work.")]
    public async Task On_document_change_cache_is_updated()
    {
        var workspace = this.SetupWorkspace();

        var sut = new CompilationTypeCache(
            workspace,
            new RoslynDescriptionFactoryDummy());

        await sut.InitializeAsync();

        var document = workspace.CurrentSolution.Projects.First().Documents.First();
        var oldText = await document.GetTextAsync();
        var newDocument = document.WithText(SourceText.From(oldText + "class MyClass {}"));

        workspace.TryApplyChanges(newDocument.Project.Solution).Should().BeTrue();
        await Task.Delay(500);

        var allTypeForAssembly = sut.GetAllTypeForAssembly(RoslynWorkspaceBuilder.ProjectName);

        allTypeForAssembly.Should().Contain(description => description.TypeFullName.GetTypeName() == "MyClass");
    }
}
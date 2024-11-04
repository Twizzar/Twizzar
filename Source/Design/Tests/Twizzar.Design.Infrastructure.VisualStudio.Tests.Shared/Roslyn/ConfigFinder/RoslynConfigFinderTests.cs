using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.Design.TestCommon;
using Twizzar.Fixture;
using Verify = TwizzarInternal.Fixture.Verify;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Roslyn.ConfigFinder;

[TestFixture]
public class RoslynConfigFinderTests
{
    private const string Code = @"
using Twizzar.Fixture;

namespace TestNamespace
{
    public partial class UtClass
    {
        public static void TestMethod()
        {
            new BuilderClass();
        }
    }

    public partial class UtClass
    {
        private class BuilderClass : ItemBuilder<int, MyIntPaths>
        {

        }
    }
}
";

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<RoslynConfigFinder>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public async Task Test_FindConfigClass()
    {
        // arrange
        var workspace = new RoslynWorkspaceBuilder()
            .AddDocument("testFile", Code)
            .AddReference(typeof(int).Assembly.Location)
            .AddReference(typeof(ItemBuilder<>).Assembly.Location)
            .Build();

        var sut = new RoslynConfigFinder();

        var document = workspace.CurrentSolution.Projects.First().Documents.Last();
        var compilation = await workspace.CurrentSolution.Projects.First().GetCompilationAsync();

        var semanticModel = await document.GetSemanticModelAsync();
        var root = await document.GetSyntaxRootAsync();

        var context = new RoslynContext(semanticModel, document, root, compilation);
        var invocation = "new BuilderClass()";
        var span = RegexSpan.CreateWithStringMatch(Code, invocation);

        // act
        var maybeConfigInformation = sut.FindConfigClass(span, context);

        // assert
        maybeConfigInformation.IsSome.Should().BeTrue();
        var configInformation = maybeConfigInformation.GetValueUnsafe();

        configInformation.CustomItemBuilderSymbol.Name.Should().Be("BuilderClass");
        configInformation.FixtureItemSymbol.Name.Should().Be("Int32");
        configInformation.ObjectCreationExpression.ToString().Should().Be("new BuilderClass()");
    }
}
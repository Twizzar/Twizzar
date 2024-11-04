using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.Fixture;
using Twizzar.TestCommon;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Roslyn;

[TestFixture]
public class ViSymbolFinderTests
{
    private Workspace _roslynWorkspace;
    private Compilation _compilation;
    private Solution _solution;

    [SetUp]
    public async Task SetUp()
    {
        this._roslynWorkspace = new RoslynWorkspaceBuilder()
            .AddReference(typeof(ViSymbolFinderTests).Assembly.Location)
            .Build();

        this._solution = this._roslynWorkspace.CurrentSolution;
        this._compilation = await this._solution.Projects.First().GetCompilationAsync();
    }

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<ViSymbolFinder>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public async Task AllAssignableSymbolsAreFound()
    {
        var sut = Build.New<ViSymbolFinder>();

        var typeSymbol = this._compilation.GetTypeByMetadataName(typeof(IA).FullName);

        var findImplementationsAndDerivedTypes = await sut.FindImplementationsAndDerivedTypesAsync(typeSymbol, this._solution);

        findImplementationsAndDerivedTypes.Should().HaveCount(5);
    }

    public interface IA { }
    public interface IB1 : IA { }
    public interface IB2 : IA { }
    public class B1 : IB1 {}
    public interface IC1 : IB1 { }
    public class C1 : IC1 {}
}
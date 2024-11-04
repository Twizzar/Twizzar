using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;
using NUnit.Framework;
using TestCreation.Services;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Dummies;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using TwizzarInternal.Fixture;

using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.TestCreation.Services;

public class LocationServiceTests
{
    #region static fields and constants

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

        public string TestProperty {get; set;}

        private void PrivateTestMethod()
        {
            new BuilderClass();
        }
    }
}
";

    #endregion

    #region members

    [Test]
    public void Ctor_throws_when_argument_is_null()
    {
        Verify.Ctor<LocationService>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    [TestCase(124, "TestMethod")]
    [TestCase(128, "TestMethod")]
    [TestCase(130, "TestMethod")]
    [TestCase(217, "TestProperty")]
    [TestCase(220, "TestProperty")]
    [TestCase(229, "TestProperty")]
    public async Task GetCurrentLocationWhenCaretOnPropertyIdentifierReturnsTheExpectedCreationContext(
        int caretPosition,
        string expectedMemberName)
    {
        // arrange
        var context = await ArrangeRoslynContext();
        var sut = CreateLocationService(context);

        // act
        var result = await sut.GetCurrentLocation("Some/Directory/", caretPosition);

        result.Info.Project.Should().Be("TestProject");
        result.Info.File.Should().Be("testFile");
        result.Info.Namespace.Should().Be("TestNamespace");
        result.Info.Type.Should().Be("UtClass");
        result.Info.Member.Should().Be(expectedMemberName);

        result.TemplateContext.Should().Be(Maybe.None<ITemplateContext>());
    }

    [Test]
    [TestCase(93)]
    public async Task GetCurrentLocationWhenCaretNotOnPropertyOrMethodIdentifierThrowsException(int caretPosition)
    {
        // arrange
        var context = await ArrangeRoslynContext();
        var sut = CreateLocationService(context);

        // act
        Func<Task> a = () => sut.GetCurrentLocation("Some/Directory/", caretPosition);

        // assert
        await a.Should().ThrowAsync<InternalException>();
    }

    [Test]
    [TestCase(270)]
    public async Task GetCurrentLocationWhenCaretOnPrivateMethodThrowsException(int caretPosition)
    {
        // arrange
        var context = await ArrangeRoslynContext();
        var sut = CreateLocationService(context);

        // act
        Func<Task> a = () => sut.GetCurrentLocation("Some/Directory/", caretPosition);

        // assert
        await a.Should().ThrowAsync<InternalException>();
    }

    [Test]
    [TestCase(40)]
    public async Task GetCurrentLocationWhenCaretNotOnMemberSyntaxThrowsException(int caretPosition)
    {
        // arrange
        var context = await ArrangeRoslynContext();
        var sut = CreateLocationService(context);

        // act
        Func<Task> a = () => sut.GetCurrentLocation("Some/Directory/", caretPosition);

        // assert
        await a.Should().ThrowAsync<InternalException>();
    }

    [TestCase(124)]
    public async Task GetCurrentLocationWithNullRoslynContextThrowsException(int caretPosition)
    {
        // arrange
        var sut = new ItemBuilder<LocationService>()
            .With(ls => ls.Ctor.roslynContextQuery.GetContextAsync.Value(
                Task.FromResult(new Failure("SomeError").ToResult<IRoslynContext, Failure>())))
            .With(ls => ls.Ctor.baseTypeService.Stub<IBaseTypeService>())
            .With(ls => ls.Ctor.roslynDescriptionFactory.Value(new RoslynDescriptionFactoryDummy()))
            .Build();

        // act
        Func<Task> a = () => sut.GetCurrentLocation("Some/Directory/", caretPosition);

        // assert
        await a.Should().ThrowAsync<InternalException>();
    }

    [TestCase(40, "Not on member")]
    [TestCase(93, "Not on member")]
    [TestCase(270, "On private member")]
    public async Task CheckIfValid_returns_false_when_not_on_valid_member(int caretPosition, string reason)
    {
        // arrange
        var context = await ArrangeRoslynContext();
        var sut = CreateLocationService(context);

        // act
        var result = await sut.CheckIfValidLocationAsync("Some/Directory/", caretPosition);

        // assert
        result.Should().BeFalse(reason);
    }

    [TestCase(124)]
    [TestCase(128)]
    [TestCase(130)]
    [TestCase(217)]
    [TestCase(220)]
    [TestCase(229)]
    public async Task CheckIfValid_returns_true_when_onValid_member(int caretPosition)
    {
        // arrange
        var context = await ArrangeRoslynContext();
        var sut = CreateLocationService(context);

        // act
        var result = await sut.CheckIfValidLocationAsync("Some/Directory/", caretPosition);

        // assert
        result.Should().BeTrue($"The caret position {caretPosition} is on a valid member \n {Code.Insert(caretPosition, "|")}");
    }

    private static async Task<RoslynContext> ArrangeRoslynContext()
    {
        var workspace = new RoslynWorkspaceBuilder()
            .AddDocument("testFile", Code)
            .AddReference(typeof(int).Assembly.Location)
            .AddReference(typeof(ItemBuilder<>).Assembly.Location)
            .Build();

        var document = workspace.CurrentSolution.Projects.First().Documents.Last();
        var compilation = await workspace.CurrentSolution.Projects.First().GetCompilationAsync();

        var semanticModel = await document.GetSemanticModelAsync();
        var root = await document.GetSyntaxRootAsync();

        var context = new RoslynContext(semanticModel, document, root, compilation);
        return context;
    }

    private static LocationService CreateLocationService(RoslynContext context)
    {
        var sut = new ItemBuilder<LocationService>()
            .With(ls => ls.Ctor.roslynContextQuery.GetContextAsync.Value(
                Result.SuccessAsync<IRoslynContext, Failure>(context)))
            .With(ls => ls.Ctor.baseTypeService.Stub<IBaseTypeService>())
            .With(ls => ls.Ctor.roslynDescriptionFactory.Value(new RoslynDescriptionFactoryDummy()))
            .Build();

        return sut;
    }

    #endregion
}
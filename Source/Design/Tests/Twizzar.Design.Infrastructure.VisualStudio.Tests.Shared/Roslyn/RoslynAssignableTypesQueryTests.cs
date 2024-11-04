using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Moq;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Common.FixtureItem.Description.Services;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Dummies;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.NLog.Interfaces;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Roslyn;

[TestFixture]
public class RoslynAssignableTypesQueryTests
{
    private IProjectNameQuery _projectNameQuery;
    private const string RootItemPath = "RootItemPath";
    private const string DependenciesFileName = @"ExampleCode\Dependencies.cs";
    private const string DependenciesNamespace = "ExampleCode.";
    private const string InterfaceATypeName = DependenciesNamespace + "InterfaceA";
    private const string ImplementationAName = DependenciesNamespace + "ImplementationA";
    private const string ImplementationBName = DependenciesNamespace + "ImplementationB";
    private const string ExtensionAName = DependenciesNamespace + "ExtensionA";
    private const string ExtensionBName = DependenciesNamespace + "ExtensionB";
    private const string MyListName = DependenciesNamespace + "MyList";

    private string DependenciesFilePath => 
        Path.Combine(Path.GetDirectoryName(typeof(RoslynAssignableTypesQueryTests).Assembly.Location), DependenciesFileName);

    [SetUp]
    public void SetUp()
    {
        this._projectNameQuery = Mock.Of<IProjectNameQuery>(
            query =>
                query.GetProjectNameAsync(Maybe.Some(RootItemPath)) ==
                Result.SuccessAsync<string, Failure>(RoslynWorkspaceBuilder.ProjectName) &&
                query.GetProjectNameAsync(Maybe.None()) ==
                Result.FailureAsync<string, Failure>(new Failure("RootItemPath is None")));
    }

    [Test]
    public void Ctor_test()
    {
        // assert
        Verify.Ctor<RoslynAssignableTypesQuery>()
            .SetupParameter("workspace", new RoslynWorkspaceWithExampleCodeBuilder().Build())
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public async Task Assignable_symbols_in_the_current_project_are_found()
    {
        // arrange
        var workspace = new RoslynWorkspaceWithExampleCodeBuilder()
            .AddDocument(this.DependenciesFilePath, File.ReadAllText(this.DependenciesFilePath))
            .Build();

        var typeDescriptionQuery = new RoslynDescriptionQuery(
            workspace,
            new RoslynDescriptionFactoryDummy(),
            this._projectNameQuery,
            new TypeSymbolQuery());
        var sut = new RoslynAssignableTypesQuery(workspace, new ViSymbolFinder(), typeDescriptionQuery, new RoslynDescriptionFactoryDummy());

        // act
        var symbols = await sut.GetAssignableTypesAsync(
            await this.GetBaseDescription(workspace, InterfaceATypeName));
        var symbols2 = await sut.GetAssignableTypesAsync(
            await this.GetBaseDescription(workspace, ImplementationAName));
        var symbols3 = await sut.GetAssignableTypesAsync(
            await this.GetBaseDescription(workspace, ImplementationBName));

        // assert
        symbols.Select(description => description.TypeFullName).Should().HaveCount(5)
            .And.Contain(
                new[]
                {
                    TypeFullName.Create(InterfaceATypeName),
                    TypeFullName.Create(ImplementationAName),
                    TypeFullName.Create(ImplementationBName),
                    TypeFullName.Create(ExtensionAName),
                    TypeFullName.Create(ExtensionBName),
                });

        symbols2.Select(description => description.TypeFullName).Should().HaveCount(3)
            .And.Contain(
                new[]
                {
                    TypeFullName.Create(ImplementationAName),
                    TypeFullName.Create(ExtensionAName),
                    TypeFullName.Create(ExtensionBName),
                });

        symbols3.Select(description => description.TypeFullName).Should().HaveCount(1)
            .And.Contain(
                new[]
                {
                    TypeFullName.Create(ImplementationBName),
                });
    }

    [Test]
    public async Task Assignable_symbols_from_external_library_found()
    {
        var coreDir = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);
        var mscorlibFilePath = Path.Combine(coreDir, "mscorlib.dll");

        // arrange
        var workspace = new RoslynWorkspaceWithExampleCodeBuilder()
            .AddDocument(this.DependenciesFilePath, File.ReadAllText(this.DependenciesFilePath))
            .AddReference(mscorlibFilePath)
            .Build();

        var typeDescriptionQuery = new RoslynDescriptionQuery(
            workspace,
            new RoslynDescriptionFactoryDummy(),
            this._projectNameQuery,
            new TypeSymbolQuery());
        var sut = new RoslynAssignableTypesQuery(workspace, new ViSymbolFinder(),  typeDescriptionQuery, new RoslynDescriptionFactoryDummy());

        // act
        var symbols = await sut.GetAssignableTypesAsync(
            await this.GetBaseDescription(workspace, typeof(IList).FullName));

        // assert
        symbols.Select(description => description?.TypeFullName).Should().Contain(new[]
        {
            TypeFullName.CreateFromType(typeof(IList)),
            TypeFullName.CreateFromType(typeof(ArrayList)),
            TypeFullName.Create(MyListName),
        });
    }

    [Test]
    public async Task GetAssignableTypesAsync_logs_thrown_exception()
    {
        // arrange
        var workspace = new RoslynWorkspaceWithExampleCodeBuilder()
            .AddDocument(this.DependenciesFilePath, File.ReadAllText(this.DependenciesFilePath))
            .Build();

        var invalidSymbolFinder = new Mock<ISymbolFinder>();

        invalidSymbolFinder.Setup(sf =>
                sf.FindImplementationsAndDerivedTypesAsync(It.IsAny<INamedTypeSymbol>(), It.IsAny<Solution>()))
            .Throws<InvalidOperationException>();

        var logger = new Mock<ILogger>();
        var sut = new RoslynAssignableTypesQuery(workspace, invalidSymbolFinder.Object, Mock.Of<ITypeDescriptionQuery>(), Mock.Of<IRoslynDescriptionFactory>())
        {
            Logger = logger.Object,
        };

        // act
        await sut.GetAssignableTypesAsync(
            await this.GetBaseDescription(workspace, InterfaceATypeName));

        // assert
        logger.Verify(l => l.Log(It.IsAny<Exception>(), LogLevel.Error), Times.Once);

    }

        
    [TestCase(InterfaceATypeName, ImplementationAName, true)]
    [TestCase(InterfaceATypeName, ExtensionAName, true)]
    [TestCase(ImplementationAName, ExtensionAName, true)]
    [TestCase(ImplementationBName, ExtensionAName, false)]
    [TestCase(ExtensionBName, ExtensionAName, false)]
    public async Task IsAssignableTo_returns_true_for_assignable_types_and_false_otherwise(string baseType, string typeName, bool expectedResult)
    {
        // arrange
        var workspace = new RoslynWorkspaceWithExampleCodeBuilder()
            .AddDocument(this.DependenciesFilePath, File.ReadAllText(this.DependenciesFilePath))
            .Build();

        var typeResolver = new RoslynDescriptionQuery(
            workspace,
            new RoslynDescriptionFactoryDummy(),
            this._projectNameQuery,
            new TypeSymbolQuery());
        var sut = new RoslynAssignableTypesQuery(workspace, new ViSymbolFinder(), typeResolver, new RoslynDescriptionFactoryDummy());

        var baseTypeDescription = 
            await this.GetBaseDescription(workspace, TypeFullName.Create(baseType));

        // act
        var result = await sut.IsAssignableTo(
            baseTypeDescription,
            TypeFullName.Create(typeName),
            RootItemPath);

        // assert 
        result.IsSome.Should().Be(expectedResult);
    }

    [Test]
    public async Task IsAssignableTo_returns_false_for_invalid_input()
    {
        // arrange
        var workspace = new RoslynWorkspaceWithExampleCodeBuilder()
            .AddDocument(this.DependenciesFilePath, File.ReadAllText(this.DependenciesFilePath))
            .Build();

        var descriptionQuery = new RoslynDescriptionQuery(
            workspace,
            new RoslynDescriptionFactoryDummy(),
            this._projectNameQuery,
            new TypeSymbolQuery());

        var failureTypeDescriptionQuery = new Mock<ITypeDescriptionQuery>();

        failureTypeDescriptionQuery.Setup(
                query => query.GetTypeDescriptionAsync(It.IsAny<TypeFullName>(), It.IsAny<Maybe<string>>()))
            .Returns(
                Task.FromResult(Result<ITypeDescription, Failure>.Failure(new Failure("dummy Failure"))));

        var emptyTypeDescriptionQuery = Mock.Of<ITypeDescriptionQuery>(
            query => query.GetTypeDescriptionAsync(It.IsAny<TypeFullName>(), It.IsAny<Maybe<string>>()) ==
                     Task.FromResult(Result<ITypeDescription, Failure>.Success(Mock.Of<ITypeDescription>())));

        var sut = new RoslynAssignableTypesQuery(workspace, new ViSymbolFinder(), descriptionQuery, new RoslynDescriptionFactoryDummy());
        var sut2 = new RoslynAssignableTypesQuery(workspace, new ViSymbolFinder(), failureTypeDescriptionQuery.Object, Mock.Of<IRoslynDescriptionFactory>());
        var sut3 = new RoslynAssignableTypesQuery(workspace, new ViSymbolFinder(), emptyTypeDescriptionQuery, Mock.Of<IRoslynDescriptionFactory>());

        var validBaseDescription = await this.GetBaseDescription(workspace, TypeFullName.Create(InterfaceATypeName));
        var validTypeName = TypeFullName.Create(ImplementationAName);

        // act
        var error = await sut.IsAssignableTo(null, validTypeName, RootItemPath);
        var error2 = await sut.IsAssignableTo(validBaseDescription, null, RootItemPath);
        var error3 = await sut.IsAssignableTo(validBaseDescription, validTypeName, Maybe.None());
        var error4 = await sut2.IsAssignableTo(validBaseDescription, validTypeName, RootItemPath);
        var error5 = await sut3.IsAssignableTo(validBaseDescription, validTypeName, RootItemPath);

        // assert 
        error.IsSome.Should().BeFalse();
        error2.IsSome.Should().BeFalse();
        error3.IsSome.Should().BeFalse();
        error4.IsSome.Should().BeFalse();
        error5.IsSome.Should().BeFalse();
    }

    private async Task<IBaseDescription> GetBaseDescription(Workspace workspace, string implementationAName)
    {
        var roslynDescriptionQuery = new RoslynDescriptionQuery(
            workspace,
            new RoslynDescriptionFactoryDummy(),
            this._projectNameQuery,
            new TypeSymbolQuery());
        var baseTypeDescription = await roslynDescriptionQuery.GetTypeDescriptionAsync(
            TypeFullName.Create(TypeFullName.Create(implementationAName)),
            RootItemPath);

        return baseTypeDescription.GetSuccessUnsafe();
    }
}
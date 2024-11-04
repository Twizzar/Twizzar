using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Dummies;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using ViCommon.Functional.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Roslyn;

[TestFixture]
public class RoslynAssignableTypesQueryPerformanceTests
{
    private IProjectNameQuery _projectNameQuery;
    private const string DependenciesFileName= @"ExampleCode\Dependencies.cs";
    private const string DependenciesNamespace = "ExampleCode.";
    private const string ImplementationBName = DependenciesNamespace + "ImplementationB";

    private const string CodePattern = "class {className} : {extends}" +
                                       "{" +
                                       "}";
    private static string DependenciesFilePath =>
        Path.Combine(Path.GetDirectoryName(typeof(RoslynAssignableTypesQueryTests).Assembly.Location), DependenciesFileName);


    [SetUp]
    public void SetUp()
    {
        this._projectNameQuery = Mock.Of<IProjectNameQuery>(query => 
            query.GetProjectNameAsync(It.IsAny<Maybe<string>>()) == Result.SuccessAsync<string, Failure>(RoslynWorkspaceBuilder.ProjectName));
    }

    [Test]
    public async Task Performance_test()
    {
        // act
        var func = await this.GenerateTest();

        // assert
        (await func.Should().CompleteWithinAsync(TimeSpan.FromSeconds(2))).Subject.Should().HaveCount(1001);
    }

    /**
     *   With 1000 test documents and 1000 iterations:
     *   Min:		 11ms
     *   Max:		 48ms
     *   Average:	 16.973ms
     */
    [Test]
    [Ignore("Use this to make e performance report.")]
    public async Task Performance_metrics()
    {
        const int nTestDocuments = 1_000;
        const int nIterations = 1_000;
        const int nWarmUp = 100;

        var func = await this.GenerateTest(nTestDocuments);
        var measurements = new List<long>();

        for (int i = 0; i < nWarmUp; i++)
        {
            await func();
        }

        var stopWatch = new Stopwatch();
        for (int i = 0; i < nIterations; i++)
        {
            stopWatch.Restart();
            await func();
            stopWatch.Stop();
            measurements.Add(stopWatch.ElapsedMilliseconds);
        }

        Console.WriteLine($"With {nTestDocuments} test documents and {nIterations} iterations:");
        Console.WriteLine($"Min:\t\t {measurements.Min()}ms");
        Console.WriteLine($"Max:\t\t {measurements.Max()}ms");
        Console.WriteLine($"Average:\t {measurements.Average()}ms");
    }

    private async Task<Func<Task<IEnumerable<ITypeFullName>>>> GenerateTest(int nTestDocuments = 1000)
    {
        // arrange
        var workspaceBuilder = new RoslynWorkspaceWithExampleCodeBuilder()
            .AddDocument(DependenciesFilePath, File.ReadAllText(DependenciesFilePath));

        foreach (var file in Directory.GetFiles(@".", "*.dll"))
        {
            workspaceBuilder.AddReference(file);
        }

        for (int i = 0; i < nTestDocuments; i++)
        {
            var extends = i == 0
                ? ImplementationBName
                : $"TestClass{i - 1}";

            workspaceBuilder.AddDocument($"file{i}.cs", GetCode($"TestClass{i}", extends));
        }

        var workspace = workspaceBuilder.Build();

        var typeResolver = new RoslynDescriptionQuery
        (workspace,
            new RoslynDescriptionFactoryDummy(),
            this._projectNameQuery,
            new TypeSymbolQuery());
        var baseTypeDescription = await typeResolver.GetTypeDescriptionAsync(
            TypeFullName.Create(TypeFullName.Create(ImplementationBName)),
            "RootItemPath");

        var typeDescriptionQuery = new RoslynDescriptionQuery(
            workspace,
            new RoslynDescriptionFactoryDummy(),
            this._projectNameQuery,
            new TypeSymbolQuery());

        Console.WriteLine("Number of Documents: \t " + workspace.CurrentSolution.Projects.First().Documents.Count());
        Console.WriteLine("All references: \t " + workspace.CurrentSolution.Projects.First().AllProjectReferences.Count);
        Console.WriteLine("Metadata references \t : " + workspace.CurrentSolution.Projects.First().MetadataReferences.Count);
        Console.WriteLine("Project references \t : " + workspace.CurrentSolution.Projects.First().ProjectReferences.Count());
        Console.WriteLine();
        var sut = new RoslynAssignableTypesQuery(workspaceBuilder.Build(), new ViSymbolFinder(), typeDescriptionQuery, new RoslynDescriptionFactoryDummy());

        return () =>
            sut.GetAssignableTypesAsync(baseTypeDescription.GetSuccessUnsafe())
                .Map(enumerable => enumerable.Select(description => description.TypeFullName));
    }

    private static string GetCode(string className, string extends) =>
        CodePattern.Replace("{className}", className).Replace("{extends}", extends);

}
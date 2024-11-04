using System;
using System.IO;
using System.Linq;
using System.Reflection;

using FluentAssertions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using NUnit.Framework;

using Twizzar.Analyzer2022.App;
using Twizzar.SharedKernel.NLog.Logging;
using Twizzar.TestCommon.Performance;

namespace Twizzar.Analyzer.Core.Tests;

[TestFixture]
public class ItemConfigPathSourceGeneratorTests
{
    [SetUp]
    public void SetUp()
    {
        LoggerFactory.SetConfig(new LoggerConfigurationBuilder());
    }

    [Test]
    [Category("Integration")]
    public void IntegrationTest2()
    {
        // Create the 'input' compilation that the generator will act on
        var sourceCode = File.ReadAllText("./IntegrationTestSourceCode.cs");
        var inputCompilation = CreateCompilation(sourceCode);

        var generator = new ItemConfigPathSourceGenerator();

        // Create the driver that will control the generation, passing in our generator
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the generation pass
        // (Note: the generator driver itself is immutable, and all calls return an updated version of the driver that you should use for subsequent calls)
        driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out _);

        outputCompilation.SyntaxTrees.Should().HaveCountGreaterOrEqualTo(2); // we have two syntax trees, the original 'user' provided one, and the one added by the generator

        // Or we can look at the results directly:
        var runResult = driver.GetRunResult();

        // The runResult contains the combined results of all generators passed to the driver
        runResult.GeneratedTrees.Length.Should().BeGreaterOrEqualTo(1);

        // Or you can access the individual results on a by-generator basis
        var generatorResult = runResult.Results[0];
        generatorResult.GeneratedSources.Should().HaveCountGreaterOrEqualTo(1);
        generatorResult.Exception.Should().BeNull();

        foreach (var source in generatorResult.GeneratedSources)
        {
            Console.WriteLine($"// ======={source.HintName}======");
            Console.WriteLine(source.SourceText);
            Console.WriteLine("// =============");
        }
    }

    [Test]
    [Category("Integration")]
    public void IntegrationTest()
    {
        // Create the 'input' compilation that the generator will act on
        var inputCompilation = CreateCompilation(@"
using Twizzar.Config;
using Twizzar.Config.Fixture;
using Twizzar.Config.Fixture.Path;
using System.Collections.Generic;

namespace Twizzar.SourceGenerator.Playground
{
    public class A
    {
        public int IntProp => 5;
        public A AProp { get; set; }
        public B BProp { get; }
    }

    public class B
    {
        public B(Class2<string> class2) { }

        public int IntProp => 5;
    }

    public interface IA{
        public void AVoidMethod();
        public int AIntMethod();
    }

    public class Class2<T>
    {
        //public A AProp { get; set; }

        public Class2(IA ia){

        }
    }

    public partial class MyConfig : ItemConfig<Class2<int>>
    {
        public MyConfig()
        {
            Member(Constructor.ia).InstanceOf<A>();
        }
    }
}
");

        var generator = new ItemConfigPathSourceGenerator();

        // Create the driver that will control the generation, passing in our generator
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the generation pass
        // (Note: the generator driver itself is immutable, and all calls return an updated version of the driver that you should use for subsequent calls)
        driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out _);

        // we have three syntax trees, the original 'user' provided one, the one added by the generator and the project statistics.
        outputCompilation.SyntaxTrees.Should().HaveCount(3);

        // Or we can look at the results directly:
        var runResult = driver.GetRunResult();

        // The runResult contains the combined results of all generators passed to the driver
        runResult.GeneratedTrees.Length.Should().Be(2);

        // Or you can access the individual results on a by-generator basis
        var generatorResult = runResult.Results[0];
        generatorResult.GeneratedSources.Should().HaveCount(2);
        generatorResult.Exception.Should().BeNull();

        Console.WriteLine(generatorResult.GeneratedSources.First().SourceText);
    }

    [Test]
    [Ignore("performance measurement")]
    public void Test_caching_performance()
    {
        //arrange
        var emptyCompilation = CreateCompilation("");

        var inputCompilation = CreateCompilation(@"
using Twizzar.Config;
using Twizzar.Config.Fixture;
using Twizzar.Config.Fixture.Path;
using System.Collections.Generic;

namespace Twizzar.SourceGenerator.Playground
{
    public class A
    {
        public int IntProp => 5;
        public A AProp { get; set; }
        public B BProp { get; }
    }

    public class B
    {
        public B(Class2<string> class2) { }

        public int IntProp => 5;
    }

    public interface IA{
        public void AVoidMethod();
        public int AIntMethod();
    }

    public class Class2<T>
    {
        public A AProp { get; set; }

        public Class2(IA ia){

        }
    }

    public class MyBuilder : ItemBuilder<Class2<int>, MyClass2Paths>
    {
        public MyConfig()
        {
            With(p => p.Ctor.ia.InstanceOf<A>());
            With(p => p.A.B.IntProp.Value(5));
            With(p => p.AProp.AProp.AProp.AProp.AProp.AProp.AProp.AProp.AProp.AProp.AProp.AProp.AProp.AProp.AProp.IntProp.Value(5));
        }
    }
}
");

        var generator = new ItemConfigPathSourceGenerator();
        var generatorNoCaching = new ItemConfigPathSourceGenerator();
        typeof(ItemConfigPathSourceGenerator)
            .GetField("_disableCaching", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(generatorNoCaching, true);

        GeneratorDriver driverNoCaching = CSharpGeneratorDriver.Create(generatorNoCaching);
        driverNoCaching = driverNoCaching.RunGenerators(emptyCompilation);

        var driver = CSharpGeneratorDriver.Create(generator);

        var noCachingStatistics = new PerformanceStatistics(
            () => driverNoCaching.RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out _),
            false);
        var updatedDriver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out _);
        var cachingStatistics = new PerformanceStatistics(
            () => updatedDriver.RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out _), false);


        // act
        noCachingStatistics.Warmup(100);
        var noCachingResult = noCachingStatistics.Measure(10, 10);

        cachingStatistics.Warmup(100);
        var caching = cachingStatistics.Measure(10, 10);

        // assert
        Console.WriteLine("no caching");
        Console.WriteLine(noCachingResult);
        Console.WriteLine("");
        Console.WriteLine("caching");
        Console.WriteLine(caching);
        Console.WriteLine("");
        Console.WriteLine("Caching is " + noCachingResult.GetAvgFactor(caching.Avg) + " times faster.");
    }

    private static Compilation CreateCompilation(string source)
        => CSharpCompilation.Create("compilation",
            new[] { CSharpSyntaxTree.ParseText(source) },
            new[]
            {
                MetadataReference.CreateFromFile(typeof(int).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Fixture.ItemBuilder<>).Assembly.Location),
            },
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));
}
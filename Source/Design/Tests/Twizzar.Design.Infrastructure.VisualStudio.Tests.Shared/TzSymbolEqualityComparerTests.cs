using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExampleCode;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using Twizzar.Analyzer.App.SourceTextGenerators;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.Design.Shared.Infrastructure.Description;
using Twizzar.Design.Shared.Infrastructure.PathTree;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests;

public class TzSymbolEqualityComparerTests
{

    public class RecursiveClass
    {
        public RecursiveClass Self { get; set; }
    }

    [TestCase(typeof(int))]
    [TestCase(typeof(string))]
    [TestCase(typeof(List<>))]
    [TestCase(typeof(ClassA))]
    [TestCase(typeof(RoslynTypeDescription))]
    [TestCase(typeof(RecursiveClass))]
    public async Task Same_symbols_are_equal(Type type)
    {
        var sut = new TzSymbolEqualityComparer(Maybe.None());

        var workspace1 = new RoslynWorkspaceBuilder()
            .AddReference(type.Assembly.Location)
            .Build();

        var workspace2 = new RoslynWorkspaceBuilder()
            .AddReference(type.Assembly.Location)
            .Build();

        var compilation1 = await workspace1.CurrentSolution.Projects.First().GetCompilationAsync();
        var compilation2 = await workspace2.CurrentSolution.Projects.First().GetCompilationAsync();

        var t1 = compilation1.GetTypeByMetadataName(type.FullName);
        var t2 = compilation2.GetTypeByMetadataName(type.FullName);

        sut.Equals(t1, t2).Should().BeTrue();
        SymbolEqualityComparer.Default.Equals(t1, t2).Should().BeFalse();
    }

    [TestCase(typeof(int), typeof(string))]
    [TestCase(typeof(List<>), typeof(string))]
    [TestCase(typeof(ClassA), typeof(ClassB))]
    [TestCase(typeof(RoslynTypeDescription), typeof(ClassA))]
    public async Task Different_symbols_are_not_equals(Type type, Type type2)
    {
        var sut = new TzSymbolEqualityComparer(Maybe.None());

        var workspace1 = new RoslynWorkspaceBuilder()
            .AddReference(type.Assembly.Location)
            .Build();

        var workspace2 = new RoslynWorkspaceBuilder()
            .AddReference(type2.Assembly.Location)
            .Build();

        var compilation1 = await workspace1.CurrentSolution.Projects.First().GetCompilationAsync();
        var compilation2 = await workspace2.CurrentSolution.Projects.First().GetCompilationAsync();

        var t1 = compilation1.GetTypeByMetadataName(type.FullName);
        var t2 = compilation2.GetTypeByMetadataName(type2.FullName);

        sut.Equals(t1, t2).Should().BeFalse();
    }

    [Test]
    public async Task Symbol_with_different_generics_are_not_be_equal()
    {
        var sut = new TzSymbolEqualityComparer(Maybe.None());

        var workspace = new RoslynWorkspaceBuilder()
            .AddReference(typeof(System.Collections.Generic.List<int>).Assembly.Location)
            .Build();

        var compilation = await workspace.CurrentSolution.Projects.First().GetCompilationAsync();

        var t1 = compilation.GetTypeByMetadataName(typeof(List<>).FullName)
            .Construct(compilation.GetTypeByMetadataName(typeof(int).FullName));
        var t2 = compilation.GetTypeByMetadataName(typeof(List<>).FullName)
            .Construct(compilation.GetTypeByMetadataName(typeof(string).FullName));

        sut.Equals(t1, t2).Should().BeFalse();
    }

    [Test]
    public async Task Symbols_with_different_Ctor_parameters_are_not_equal()
    {
        var code = @$"""
            public class MyTest{{
                public MyTest(int <PARAMNAME>){{}}
            }}
            """;

        var sut = new TzSymbolEqualityComparer(
            Maybe.Some(PathNode.ConstructRoot(new [] {new []{"Ctor"}})));

        var workspace1 = new RoslynWorkspaceBuilder()
            .AddDocument("test", code)
            .Build();

        var workspace2 = new RoslynWorkspaceBuilder()
            .AddDocument("test", code.Replace("<PARAMNAME>", "b"))
            .Build();

        var compilation1 = await workspace1.CurrentSolution.Projects.First().GetCompilationAsync();
        var compilation2 = await workspace2.CurrentSolution.Projects.First().GetCompilationAsync();

        var t1 = compilation1.GetTypeByMetadataName("MyTest");
        var t2 = compilation2.GetTypeByMetadataName("MyTest");

        sut.Equals(t1, t2).Should().BeFalse();
    }

    [Test]
    public async Task Symbols_with_same_Ctor_parameters_are_equal()
    {
        var code = @$"""
            public class MyTest{{
                public MyTest(int a){{}}
            }}
            """;

        var sut = new TzSymbolEqualityComparer(
            Maybe.Some(PathNode.ConstructRoot(new[] { new[] { "Ctor" } })));

        var workspace1 = new RoslynWorkspaceBuilder()
            .AddDocument("test", code)
            .Build();

        var workspace2 = new RoslynWorkspaceBuilder()
            .AddDocument("test", code)
            .Build();

        var compilation1 = await workspace1.CurrentSolution.Projects.First().GetCompilationAsync();
        var compilation2 = await workspace2.CurrentSolution.Projects.First().GetCompilationAsync();

        var t1 = compilation1.GetTypeByMetadataName("MyTest");
        var t2 = compilation2.GetTypeByMetadataName("MyTest");

        sut.Equals(t1, t2).Should().BeTrue();
    }

    [TestCase("int32", "a", "int32", "b", false)]
    [TestCase("int32", "a", "string", "a", false)]
    [TestCase("int32", "a", "int32", "a", true)]
    public async Task Symbols_with_Method_parameters_get_compared_correctly(
        string param1Type,
        string param1Name,
        string param2Type,
        string param2Name,
        bool areEquals)
    {
        var code = @$"""
            public class MyTest{{
                public void MyMethod(<PType> <PName>){{}}

                public void MyMethod(int a, int b) {{}}
            }}
            """;

        var sut = new TzSymbolEqualityComparer(
            Maybe.Some(PathNode.ConstructRoot(new[]
            {
                new[] { $"MyMethod" },
            })));

        var workspace1 = new RoslynWorkspaceBuilder()
            .AddDocument("test", code
                .Replace("<PType>", param1Type)
                .Replace("<PName>", param1Name))
            .Build();

        var workspace2 = new RoslynWorkspaceBuilder()
            .AddDocument("test", code
                .Replace("<PType>", param2Type)
                .Replace("<PName>", param2Name))
            .Build();

        var compilation1 = await workspace1.CurrentSolution.Projects.First().GetCompilationAsync();
        var compilation2 = await workspace2.CurrentSolution.Projects.First().GetCompilationAsync();

        var t1 = compilation1.GetTypeByMetadataName("MyTest");
        var t2 = compilation2.GetTypeByMetadataName("MyTest");

        sut.Equals(t1, t2).Should().Be(areEquals);
    }

    [TestCase("int", "int", true)]
    [TestCase("int", "string", false)]
    public async Task Symbols_with_Method_returnType_get_compared_correctly(
        string rType1,
        string rType2,
        bool areEquals)
    {
        var code = @$"""
            public class MyTest{{
                public <RType> MyMethod(int a){{}}
            }}
            """;

        var sut = new TzSymbolEqualityComparer(
            Maybe.Some(PathNode.ConstructRoot(new[]
            {
                new[] { $"MyMethod" },
            })));

        var workspace1 = new RoslynWorkspaceBuilder()
            .AddDocument("test", code
                .Replace("<RType>", rType1))
            .Build();

        var workspace2 = new RoslynWorkspaceBuilder()
            .AddDocument("test", code
                .Replace("<RType>", rType2))
            .Build();

        var compilation1 = await workspace1.CurrentSolution.Projects.First().GetCompilationAsync();
        var compilation2 = await workspace2.CurrentSolution.Projects.First().GetCompilationAsync();

        var t1 = compilation1.GetTypeByMetadataName("MyTest");
        var t2 = compilation2.GetTypeByMetadataName("MyTest");

        sut.Equals(t1, t2).Should().Be(areEquals);
    }
}
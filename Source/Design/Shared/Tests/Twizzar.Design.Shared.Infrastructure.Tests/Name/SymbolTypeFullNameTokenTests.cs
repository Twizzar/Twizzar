using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Shared.Infrastructure.VisualStudio2019.Name;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using ViCommon.Functional;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;
using static ViCommon.Functional.Monads.ResultMonad.Result;

namespace Twizzar.Design.Shared.Infrastructure.Tests.Name;

[TestFixture]
public class SymbolTypeFullNameTokenTests
{
    public static IEnumerable<object[]> ValidTypes
    {
        get
        {
            yield return new object[] { typeof(System.Collections.Generic.List<Int64>) };
            yield return new object[] { typeof(System.UInt32) };
            yield return new object[] { typeof(Maybe<List<string>>) };
            yield return new object[] { typeof(int[]) };
            yield return new object[] { typeof(int[][]) };
            yield return new object[] { typeof((int[], string)) };
            yield return new object[] { typeof(Dictionary<int, string>) };
            yield return new object[] { typeof(Dictionary<int, string>.KeyCollection) };
        }
    }

    public static IEnumerable<object[]> ArrayTypes
    {
        get
        {
            yield return new object[] { typeof(int[]), new List<int> { 1 } };
            yield return new object[] { typeof(int[,]), new List<int> { 2 } };
            yield return new object[] { typeof(int[,,]), new List<int> { 3 } };
            yield return new object[] { typeof(int[,,][]), new List<int> { 1, 3 } };
            yield return new object[] { typeof(int[,,][][]), new List<int> { 1, 1, 3 } };
            yield return new object[] { typeof(int[,,][][,]), new List<int> { 2, 1, 3 } };
        }
    }

    public static IEnumerable<object[]> InvalidTypes
    {
        get
        {
            yield return new object[] { "System<int>.Test" };
            yield return new object[] { "Test/Type" };
            yield return new object[] { "System..Type" };
        }
    }

    public static IEnumerable<object[]> CSharpTypeFullNames
    {
        get
        {
            yield return new object[] { "System.Tuple<System.Int32>", typeof(Tuple<int>), };
            yield return new object[] { "System.Nullable<System.Int32>", typeof(int?), };
            yield return new object[] { "System.Collections.Generic.List<System.Int32[]>", typeof(List<int[]>), };
            yield return new object[] { "System.Int32[][]", typeof(int[][]), };
            yield return new object[] { "System.Int32[,]", typeof(int[,]), };
            yield return new object[] { "System.Collections.Generic.Dictionary<System.Int32, System.String>", typeof(Dictionary<int, string>), };
            yield return new object[] { "System.Collections.Generic.Dictionary<System.Int32, System.String>.KeyCollection", typeof(Dictionary<int, string>.KeyCollection), };
            yield return new object[]
            {
                    "System.Tuple<System.String, System.Tuple<System.Int32, System.Int32>>",
                    typeof(Tuple<string, Tuple<int, int>>),
            };
        }
    }

    public static IEnumerable<object[]> CSharpTypes
    {
        get
        {
            yield return new object[] { "Tuple<Int32>", typeof(Tuple<int>), };
            yield return new object[] { "Nullable<Int32>", typeof(int?), };
            yield return new object[]
            {
                    "Tuple<String, Tuple<Int32, Int32>>",
                    typeof(Tuple<string, Tuple<int, int>>),
            };
        }
    }

    private static SymbolTypeFullNameToken CreateToken(Type type)
    {
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            null,
            new[]
            {
                MetadataReference.CreateFromFile(type.Assembly.Location),
                MetadataReference.CreateFromFile(typeof(int).Assembly.Location)
            });


        var symbol = GetTypeSymbol(compilation, type)
            .Match(failure => throw new Exception(failure.Message));

        return new SymbolTypeFullNameToken(symbol);
    }


    [TestCaseSource(nameof(ValidTypes))]
    public void CreateFromType_GetNameSpace_and_GetTypeName_return_the_correct_result(Type type)
    {
        var sut = CreateToken(type);

        sut.Namespace.Should().Be(type.Namespace + ".");
        (sut.Typename + sut.GenericPostfix.SomeOrProvided("")).Should().Be(type.Name);

        Console.WriteLine(sut.ToFriendlyCSharpFullTypeName());
    }

    [TestCaseSource(nameof(ArrayTypes))]
    public void CreateFromType_Arrays_Correct_FullName(Type type, List<int> structure)
    {
        var sut = CreateToken(type);

        sut.ArrayBrackets.Should().HaveCountGreaterThan(0);
        sut.ToFullName().Should().Be(type.FullName);
        sut.ArrayDimension.Should().ContainInOrder(structure);
    }

    [TestCaseSource(nameof(ValidTypes))]
    public void Crate_GetNameSpace_and_GetTypeName_return_the_correct_result(Type type)
    {
        var sut = CreateToken(type);

        sut.Namespace.Should().Be(type.Namespace + ".");
        (sut.Typename + sut.GenericPostfix.SomeOrProvided("")).Should().Be(type.Name);
    }

    [TestCaseSource(nameof(CSharpTypeFullNames))]
    public void GetFriendlyCSharpName_returns_the_expected_result(string expectedTypeName, Type type)
    {
        // arrange
        //var expectedTypeName = "System.Tuple<System.Int32>";
        var sut = CreateToken(type);

        // act
        var name = sut.ToFriendlyCSharpFullTypeName();

        // assert
        name.Should().Be(expectedTypeName);
    }

    [TestCaseSource(nameof(CSharpTypes))]
    public void GetFriendlyCSharpTypeName_returns_the_expected_result(string expectedTypeName, Type type)
    {
        // arrange
        //var expectedTypeName = "System.Tuple<System.Int32>";
        var sut = CreateToken(type);

        // act
        var name = sut.ToFriendlyCSharpTypeName();

        // assert
        name.Should().Be(expectedTypeName);
    }

    private static IResult<ITypeSymbol, Failure> GetTypeSymbol(Compilation compilation, Type type) =>
        GetTypeSymbolInternal(compilation, TypeFullName.CreateFromType(type));
        


    private static IResult<ITypeSymbol, Failure> GetTypeSymbolInternal(Compilation compilation, TypeFullName tFullName)
    {
        if (tFullName.IsArray())
        {
            return GetArrayTypeSymbol(compilation, tFullName);
        }

        var metadataName = tFullName.GetTypeFullNameWithoutGenerics();
        var typeSymbol = compilation.GetTypeByMetadataName(metadataName);
        if (typeSymbol == null)
        {
            return Failure<ITypeSymbol, Failure>(
                new Failure($"Cannot find type {tFullName.FullName} in the compilation {compilation}"));
        }

        // When the type symbol is generic then construct the real type.
        // The typeSymbol System.Tuple´2 will be constructed to System.Tuple´2[[System.Int32, ca], [System.String, ca]] for example where ca is the containing assembly.
        if (typeSymbol.IsGenericType)
        {
            var typeArgumentSymbols = CreateNamedTypeSymbols(compilation, typeSymbol, tFullName);

            if (typeSymbol.ContainingSymbol is INamedTypeSymbol containingSymbol &&
                containingSymbol.IsGenericType)
            {
                return typeArgumentSymbols.MapSuccess(
                        symbols => containingSymbol.Construct(symbols.ToArray()) as ITypeSymbol)
                    .MapSuccess(symbol => symbol.GetTypeMembers(typeSymbol.Name).Single());
            }

            return typeArgumentSymbols.MapSuccess(
                symbols => typeSymbol.Construct(symbols.ToArray()) as ITypeSymbol);
        }
        else
        {
            return Success<ITypeSymbol, Failure>(typeSymbol);
        }
    }

    private static IResult<ITypeSymbol, Failure> GetArrayTypeSymbol(Compilation compilation, TypeFullName typeFullName)
    {
        ITypeSymbol arrayElementTypeSymbol = compilation.GetTypeByMetadataName(
            typeFullName.ArrayElementType().Match(
                some => some.GetTypeFullNameWithoutGenerics(),
                () => throw new InternalException("Array type but invalid TypeFullName")));

        if (arrayElementTypeSymbol is null)
        {
            return Failure<ITypeSymbol, Failure>(
                new Failure($"Cannot resolve array element type symbol {typeFullName.ArrayElementType()} in the compilation {compilation}"));
        }

        IArrayTypeSymbol arraySymbol = null;
        foreach (var dim in typeFullName.ArrayDimension().Reverse())
        {
            arraySymbol = compilation.CreateArrayTypeSymbol(arrayElementTypeSymbol, dim);
            arrayElementTypeSymbol = arraySymbol;
        }

        return Success<ITypeSymbol, Failure>(arraySymbol);
    }

    // Create names type symbols for the give type ful name
    // For example for Tuple<int, List<string>> this method will create a int type symbol and recursively creates a string type symbol and constructs with it a List<sting> type symbol.
    private static IResult<IEnumerable<ITypeSymbol>, Failure> CreateNamedTypeSymbols(
        Compilation compilation,
        INamedTypeSymbol namedTypeSymbol,
        ITypeFullName typeFullName)
    {
        var result = new List<ITypeSymbol>();

        if (namedTypeSymbol.IsGenericType)
        {
            var names = typeFullName.GenericTypeArguments();

            foreach (var genericTypeArgument in names)
            {
                var entry = GetTypeSymbolInternal(compilation, (TypeFullName)genericTypeArgument);
                switch (entry.AsResultValue())
                {
                    case FailureValue<Failure> failureValue:
                        return Failure<IEnumerable<ITypeSymbol>, Failure>(failureValue.Value);
                    case SuccessValue<ITypeSymbol> successValue:
                        result.Add(successValue.Value);
                        break;
                    default:
                        throw PatternErrorBuilder.PatternCase(nameof(entry.AsResultValue))
                            .IsNotOneOf(nameof(FailureValue<Failure>), nameof(SuccessValue<ITypeSymbol>));
                }
            }
        }

        return Success<IEnumerable<ITypeSymbol>, Failure>(result);
    }
}

public class OuterClass<T>
{
    public class InnerClass{}
}
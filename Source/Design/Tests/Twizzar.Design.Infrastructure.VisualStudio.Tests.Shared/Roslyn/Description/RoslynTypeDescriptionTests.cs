using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Shared.Infrastructure.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.TestCommon;
using Twizzar.TestCommon.TypeDescription;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Roslyn.Description;

[TestFixture]
public class RoslynTypeDescriptionTests
{
    private Workspace _roslynWorkspace;
    private Project _project;
    private Compilation _compilation;

    [SetUp]
    public async Task SetUp()
    {
        this._roslynWorkspace = new RoslynWorkspaceBuilder()
            .AddReference(typeof(RoslynTypeDescription).Assembly.Location)
            .AddReference(typeof(RoslynTypeDescriptionTests).Assembly.Location)
            .AddReference(typeof(ITypeDescription).Assembly.Location)
            .AddReference(typeof(List<>).Assembly.Location)
            .AddReference(typeof(TypeDescriptionTester<>).Assembly.Location)
            .Build();

        this._project = this._roslynWorkspace.CurrentSolution.Projects.FirstOrDefault();
        this._compilation = await this._project.GetCompilationAsync();
    }

    [Test]
    public void All_Ctor_parameter_throws_ArgumentNullException_when_null()
    {
        // arrange
        var symbol = this._compilation.GetTypeByMetadataName(typeof(TypeDescriptionTester<>.TestClass).FullName);

        // assert
        Verify.Ctor<RoslynTypeDescription>()
            .SetupParameter("typeSymbol", symbol)
            .ShouldThrowArgumentNullException();
    }

    [TestCase(typeof(TypeDescriptionTester<>.TestClass), typeof(object))]
    [TestCase(typeof(RoslynTypeDescription), typeof(TypeDescription))]
    [TestCase(typeof(TypeDescription), typeof(BaseTypeDescription))]
    public void BaseType_is_set_correctly(Type type, Type expectedBaseType)
    {
        // arrange
        var tester = this.CreateTester(type.FullName);

        // act & assert
        tester
            .BaseType_is_set_correctly(expectedBaseType);
    }

    [TestCase(typeof(TypeDescriptionTester<>.TestClass), new Type[] { })]
    [TestCase(typeof(TypeDescriptionTester<>.ClassA), new[] { typeof(TypeDescriptionTester<>.IA1), typeof(TypeDescriptionTester<>.IA2), typeof(TypeDescriptionTester<>.IA3), typeof(TypeDescriptionTester<>.IC1), typeof(TypeDescriptionTester<>.IB1) })]
    public void ImplementedInterfaceNames_is_set_correctly(
        Type type,
        IEnumerable<Type> expectedImplementedInterfaceNames)
    {
        // arrange
        var tester = this.CreateTester(type.FullName);

        // act & assert
        tester
            .ImplementedInterfaceNames_is_set_correctly(expectedImplementedInterfaceNames);
    }

    [TestCase(typeof(NonGenericClass), false)]
    [TestCase(typeof(TypeDescriptionTester<>.SealedClass), true)]
    public void IsSealed_is_set_correctly(Type type, bool expectedIsSealed)
    {
        // arrange
        var tester = this.CreateTester(type.FullName);

        // act & assert
        tester
            .IsSealed_is_set_correctly(expectedIsSealed);
    }

    [TestCase(typeof(TypeDescriptionTester<>.TestClass), false)]
    [TestCase(typeof(TypeDescriptionTester<>.StaticClass), true)]
    public void IsStatic_is_set_correctly(Type type, bool expectedIsStatic)
    {
        // arrange
        var tester = this.CreateTester(type.FullName);

        // act & assert
        tester
            .IsStatic_is_set_correctly(expectedIsStatic);
    }

    [TestCase(typeof(NonGenericClass), false)]
    [TestCase(typeof(TypeDescriptionTester<>.GenericClass<,>), true)]
    public void IsGeneric_is_set_correctly(Type type, bool expectedIsGeneric)
    {
        // arrange
        var tester = this.CreateTester(type.FullName);

        // act & assert
        tester
            .IsGeneric_is_set_correctly(expectedIsGeneric);
    }

    [TestCase(typeof(TypeDescriptionTester<>.TestClass), true)]
    [TestCase(typeof(RoslynTypeDescriptionTests), false)]
    public void IsNested_is_set_correctly(Type type, bool expectedIsNested)
    {
        // arrange
        var tester = this.CreateTester(type.FullName);

        // act & assert
        tester
            .IsNested_is_set_correctly(expectedIsNested);
    }

    [TestCase(typeof(TypeDescriptionTester<>.TestClass))]
    [TestCase(typeof(RoslynTypeDescriptionTests))]
    public void GetTypeSymbol_returns_symbol(Type type)
    {
        // arrange
        var tester = this.CreateTester(type.FullName);

        // act
        var sut = Build.New<RoslynTypeDescription>();

        // assert
        sut.GetTypeSymbol().Should().NotBeNull();
    }

    [Test]
    public void All_properties_from_implemented_interfaces_are_found()
    {
        // arrange
        var expectedPropertyNames = new[]
        {
            nameof(TypeDescriptionTester<RoslynTypeDescription>.IA3.IA3Prop),
            nameof(TypeDescriptionTester<RoslynTypeDescription>.IA3.IB1Prop),
            nameof(TypeDescriptionTester<RoslynTypeDescription>.IA3.IC1Prop),
        };

        // act
        var sut = new ItemBuilder<RoslynTypeDescription>()
            .With(p => p.Ctor.typeSymbol.Value(this._compilation.GetTypeByMetadataName(typeof(TypeDescriptionTester<>.IA3).FullName)))
            .Build();

        // assert
        sut.GetDeclaredProperties()
            .Select(description => description.Name)
            .Should()
            .Contain(expectedPropertyNames);
    }

    [TestCase(typeof(List<>), true)]
    [TestCase(typeof(List<string>), true, true)]
    [TestCase(typeof(TypeDescriptionTester<>.TestCollection), true)]
    [TestCase(typeof(ICollection<>), false)]
    [TestCase(typeof(Dictionary<,>), true)]
    [TestCase(typeof(Dictionary<int, string>), true, true)]
    public void Is_InheritedICollection_determines_correctly(Type type, bool expectedValue, bool isGeneric = false)
    {
        // arrange
        var t = TypeFullName.CreateFromType(type);

        var tester = this.CreateTester(
            isGeneric 
                ? t.GetTypeFullNameWithoutGenerics()
                : t.FullName);

        // act
        tester
            .IsInheritedFromICollection_set_correctly(expectedValue);
    }

    [TestCase(typeof(int[]), true)]
    [TestCase(typeof(string[]), true)]
    [TestCase(typeof(IPropertyDescription[]), true)]
    [TestCase(typeof(int), false)]
    [TestCase(typeof(string), false)]
    [TestCase(typeof(IPropertyDescription), false)]
    public void IsArray_is_set_correctly(Type type, bool expectedValue)
    {
        var tester = expectedValue
            ? this.CreateTesterWithSymbol(
                this._compilation.CreateArrayTypeSymbol(
                    this._compilation.GetTypeByMetadataName(type.GetElementType().FullName)))
            : this.CreateTester(type.FullName);


        // act & assert
        tester.IsArray_is_set_correctly(expectedValue);
    }

    [TestCase(typeof(int), new[] { 1 })]
    [TestCase(typeof(int), new[] { 1, 1 })]
    [TestCase( typeof(int), new[] { 1, 1, 1 })]
    [TestCase( typeof(int), new[] { 2 })]
    [TestCase(typeof(int), new[] { 3, 2 })]
    [TestCase(typeof(int), new[] { 1, 3 })]
    public void ArrayDimension_is_set_correctly(Type arrayElementType, int[] expectedValue)
    {
        ITypeSymbol arrayElementTypeSymbol = this._compilation.GetTypeByMetadataName(arrayElementType.FullName);

        IArrayTypeSymbol arraySymbol = null;
        foreach (var dim in expectedValue)
        {
            arraySymbol = this._compilation.CreateArrayTypeSymbol(arrayElementTypeSymbol, dim);
            arrayElementTypeSymbol = arraySymbol;
        }

        // arrange
        var tester = this.CreateTesterWithSymbol(arraySymbol);

        // act & assert
        tester.ArrayDimension_is_set_correctly(expectedValue);
    }

    private TypeDescriptionTester<RoslynTypeDescription> CreateTester(string typeFullName) =>
        new(new ItemBuilder<RoslynTypeDescription>()
            .With(p => p.Ctor.typeSymbol.Value(this._compilation.GetTypeByMetadataName(typeFullName))));

    private TypeDescriptionTester<RoslynTypeDescription> CreateTesterWithSymbol(ITypeSymbol typeSymbol) =>
        new(new ItemBuilder<RoslynTypeDescription>()
            .With(p => p.Ctor.typeSymbol.Value(typeSymbol)));

    public class NonGenericClass { }
}
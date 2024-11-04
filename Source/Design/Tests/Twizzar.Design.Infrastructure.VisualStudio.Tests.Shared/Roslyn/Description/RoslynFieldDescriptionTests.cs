using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.Design.Shared.Infrastructure.Description;
using Twizzar.Design.Shared.Infrastructure.Extension;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.TestCommon.TypeDescription;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Roslyn.Description;

[TestFixture]
public partial class RoslynFieldDescriptionTests
{
    #region static fields and constants

    private static readonly object[] _accessModifier_is_set_correctlyCases =
    {
        new object[] {(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.IntField)), AccessModifier.CreatePublic()},
        new object[] {"protectedInt", AccessModifier.CreateProtected()},
        new object[] {"privateInt", AccessModifier.CreatePrivate()},
        new object[] {"internalInt", AccessModifier.CreateInternal()},
    };

    #endregion

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
            .AddReference(typeof(TypeDescriptionTester<>).Assembly.Location)
            .Build();

        this._project = this._roslynWorkspace.CurrentSolution.Projects.FirstOrDefault();
        this._compilation = await this._project.GetCompilationAsync();
    }

    [Test]
    public void All_Ctor_parameter_throws_ArgumentNullException_when_null()
    {
        // arrange
        var symbol = this._compilation.GetTypeByMetadataName(typeof(FieldDescriptionTester<>.TestClass).FullName);
        var field = symbol.GetMembers().OfType<IFieldSymbol>().FirstOrDefault();

        // assert
        Verify.Ctor<RoslynFieldDescription>()
            .SetupParameter("fieldSymbol", field)
            .ShouldThrowArgumentNullException();
    }

    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.IntField))]
    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.ConstStr))]
    public void Name_is_set_correctly(string fieldName)
    {
        // arrange
        var tester = this.CreateTester(fieldName);

        // act & assert
        tester.Name_is_set_correctly(fieldName);
    }

    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.IntField))]
    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.ConstStr))]
    public void TypeFullName_is_set_correctly(string fieldName)
    {
        // arrange
        var tester = this.CreateTester(fieldName);
        var symbol = this.GetFieldSymbol(fieldName);
        //this._fixture.SetInstance(symbol);

        // act & assert
        tester.TypeFullName_is_set_correctly(symbol.Type.GetTypeFullName().FullName);
    }

    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.IntField))]
    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.ConstStr))]
    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.StaticFloat))]
    public void DeclaringType_is_set_correctly(string fieldName)
    {
        // arrange
        var tester = this.CreateTester(fieldName);

        // act & assert
        tester.DeclaringType_is_set_correctly(typeof(FieldDescriptionTester<>.TestClass).FullName);
    }

    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.IntField), false)]
    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.ConstStr), true)]
    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.StaticFloat), false)]
    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.ReadonlyInt), false)]
    public void IsConstant_is_set_correctly(string fieldName, bool isConstant)
    {
        // arrange
        var tester = this.CreateTester(fieldName);

        // act & assert
        tester.IsConstant_is_set_correctly(isConstant);
    }

    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.IntField), null)]
    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.ConstStr), "TestStr")]
    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.StaticFloat), null)]
    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.ReadonlyInt), null)]
    public void ConstantValue_is_set_correctly(string fieldName, string value)
    {
        // arrange
        var tester = this.CreateTester(fieldName);

        // act & assert
        tester.ConstantValue_is_set_correctly(value);
    }

    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.IntField), false)]
    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.ConstStr), true)]
    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.StaticFloat), true)]
    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.ReadonlyInt), false)]
    public void IsStatic_is_set_correctly(string fieldName, bool isStatic)
    {
        // arrange
        var tester = this.CreateTester(fieldName);

        // act & assert
        tester.IsStatic_is_set_correctly(isStatic);
    }

    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.IntField), false)]
    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.ConstStr), false)]
    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.StaticFloat), false)]
    [TestCase(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.ReadonlyInt), true)]
    public void IsReadonly_is_set_correctly(string fieldName, bool isReadonly)
    {
        // arrange
        var tester = this.CreateTester(fieldName);

        // act & assert
        tester.IsReadonly_is_set_correctly(isReadonly);
    }

    [TestCaseSource(nameof(_accessModifier_is_set_correctlyCases))]
    public void AccessModifier_is_set_correctly(string fieldName, object accessModifier)
    {
        // arrange
        var tester = this.CreateTester(fieldName);

        // act & assert
        tester.AccessModifier_is_set_correctly(accessModifier);
    }

    [Test]
    public void ReturnTypeDescription_is_set_correctly()
    {
        // act
        var sut = new ItemBuilder<RoslynFieldDescription>()
            .With(p => p.Ctor.fieldSymbol.Value(
                this.GetFieldSymbol(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.IntField))))
            .Build();

        var sut2 = new ItemBuilder<RoslynFieldDescription>()
            .With(p => p.Ctor.fieldSymbol.Value(
                this.GetFieldSymbol(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.ReadonlyInt))))
            .Build();

        // assert
        sut.GetReturnTypeDescription().Should().Be(sut2.GetReturnTypeDescription());
    }

    [Test]
    public void Type_Booleans_for_class_are_set_correctly()
    {
        // arrange
        var tester = this.CreateTester(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.TestClassField));

        // act & assert
        tester.Type_Booleans_for_class_are_set_correctly();
    }

    [Test]
    public void Type_Booleans_for_interfaces_are_set_correctly()
    {
        // arrange
        var tester = this.CreateTester(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.TestInterfaceField));

        // act & assert
        tester.Type_Booleans_for_interfaces_are_set_correctly();
    }

    [Test]
    [Ignore("Fix and merge with reflection in later PR")]
    public void Type_Booleans_for_struct_are_set_correctly()
    {
        // arrange
        var tester = this.CreateTester(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.TestStructField));

        // act & assert
        tester.Type_Booleans_for_struct_are_set_correctly();
    }

    [Test]
    [Ignore("Fix and merge with reflection in later PR")]
    public void Type_Booleans_for_enum_are_set_correctly()
    {
        // arrange
        var tester = this.CreateTester(nameof(FieldDescriptionTester<RoslynFieldDescription>.TestClass.TestEnumField));

        // act & assert
        tester.Type_Booleans_for_enum_are_set_correctly();
    }

    private FieldDescriptionTester<RoslynFieldDescription> CreateTester(string fieldName) =>
        new(new ItemBuilder<RoslynFieldDescription>()
            .With(p => p.Ctor.fieldSymbol.Value(this.GetFieldSymbol(fieldName))));

    private IFieldSymbol GetFieldSymbol(string name) =>
        this._compilation.GetTypeByMetadataName(typeof(FieldDescriptionTester<>.TestClass).FullName)
            .GetMembers()
            .OfType<IFieldSymbol>()
            .FirstOrDefault(symbol => symbol.Name == name);
}
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using NUnit.Framework;

using Twizzar.Analyzer.Core.SourceTextGenerators;
using Twizzar.Analyzer.Core.Tests.Builders;
using Twizzar.Design.Shared.CoreInterfaces.FixtureItem.Description;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.Design.Shared.Infrastructure.PathTree;
using Twizzar.SharedKernel.CoreInterfaces.Failures;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.NLog.Logging;
using Twizzar.TestCommon;
using Twizzar.TestCommon.TypeDescription.Builders;

using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Analyzer.Core.Tests.SourceTextGenerators
{
    [TestFixture]
    public class PathSourceTextMemberGeneratorTests
    {
        private Compilation _compilation;

        [SetUp]
        public void SetUp()
        {
            LoggerFactory.SetConfig(new LoggerConfigurationBuilder());
            this._compilation = CSharpCompilation.Create("Test");
        }

        [Test]
        public void Ctor_parameters_throw_ArgumentNullException_when_null()
        {
            // assert
            TwizzarInternal.Fixture.Verify.Ctor<PathSourceTextMemberGenerator>()
                .ShouldThrowArgumentNullException();
        }

        [Test]
        public void No_members_generate_string_empty()
        {
            // arrange
            var sut = new PathSourceTextMemberGeneratorBuilder()
                .Build();
            var typeDescription = new TypeDescriptionBuilder().Build();

            // act
            var output = sut.GenerateMembers(
                typeDescription,
                Maybe.None(),
                "FixtureItemTypeName",
                new HashSet<string>(),
                Mock.Of<HashSet<string>>(),
                true,
                this._compilation,
                Mock.Of<ITypeSymbol>(),
                new List<MemberVerificationInfo>(),
                "FixtureItemTypeName");

            // assert
            output.Should().Be(string.Empty);
        }

        public class PathSourceTextMemberGeneratorBuilder : ItemBuilder<PathSourceTextMemberGenerator, PathSourceTextMemberGeneratorCustomPaths>
        {
            public PathSourceTextMemberGeneratorBuilder()
            {
                this.With(p => p.Ctor.symbolService.IsSymbolAccessibleWithin.Value(true));
            }
        }

        [Test]
        public void Property_get_generated_correctly()
        {
            // arrange
            var propertyName = GetRandomMemberName();
            var propertyType = TypeFullName.Create("PropertyType");

            var typeDescription = new TypeDescriptionBuilder()
                .WithIsInterface(true)
                .WithDeclaredProperties(
                    Mock.Of<IDesignPropertyDescription>(
                        description =>
                            description.Name == propertyName &&
                            description.TypeFullName == propertyType &&
                            description.GetFriendlyReturnTypeFullName() == propertyType.GetFriendlyCSharpTypeFullName()))
                .Build();

            // act
            var (expected, output) = this.TestMember("Property", typeDescription, propertyName, propertyType);

            // assert
            output.Should().Be(expected);
        }

        [Test]
        public void Field_get_generated_correctly()
        {
            // arrange
            var fieldName = GetRandomMemberName();
            var fieldType = TypeFullName.Create("FieldType");

            var typeDescription = new TypeDescriptionBuilder()
                .WithIsInterface(true)
                .WithDeclaredFields(
                    Mock.Of<IDesignFieldDescription>(
                        description =>
                            description.Name == fieldName &&
                            description.TypeFullName == fieldType &&
                            description.GetFriendlyReturnTypeFullName() == fieldType.GetFriendlyCSharpTypeFullName()))
                .Build();

            // act
            var (expected, output) = this.TestMember("Field", typeDescription, fieldName, fieldType);

            // assert
            output.Should().Be(expected);
        }

        [Test]
        public void BaseTypeProperty_get_generated_correctly()
        {
            // arrange
            var propertyName = GetRandomMemberName();
            var propertyType = TypeFullName.Create("PropertyType");

            var typeDescription = new TypeDescriptionBuilder()
                .WithIsInterface(true)
                .WithDeclaredProperties(
                    Mock.Of<IDesignPropertyDescription>(
                        description =>
                            description.Name == propertyName &&
                            description.TypeFullName == propertyType &&
                            description.IsBaseType == true &&
                            description.GetFriendlyReturnTypeFullName() == propertyType.GetFriendlyCSharpTypeFullName() &&
                            description.GetReturnTypeDescription() == CreateReturnTypeDescription(propertyType)))
                .Build();

            // act
            var (expected, output) = this.TestBaseMember("Property", typeDescription, propertyName, propertyType);

            // assert
            output.Should().Be(expected);
        }

        [Test]
        public void BaseTypeField_get_generated_correctly()
        {
            // arrange
            var fieldName = GetRandomMemberName();
            var fieldType = TypeFullName.Create("FieldType");

            var typeDescription = new TypeDescriptionBuilder()
                .WithIsInterface(true)
                .WithDeclaredFields(
                    Mock.Of<IDesignFieldDescription>(
                        description =>
                            description.Name == fieldName &&
                            description.TypeFullName == fieldType &&
                            description.IsBaseType == true &&
                            description.GetFriendlyReturnTypeFullName() == fieldType.GetFriendlyCSharpTypeFullName() &&
                            description.GetReturnTypeDescription() == CreateReturnTypeDescription(fieldType)))
                .Build();

            // act
            var (expected, output) = this.TestBaseMember("Field", typeDescription, fieldName, fieldType);

            // assert
            output.Should().Be(expected);
        }

        [Test]
        public void CtorParameter_get_generated_correctly()
        {
            // arrange
            var ctorParamName = GetRandomMemberName();
            var ctorParamType = TypeFullName.Create("CtorParamBasetypeMemberPath");

            var typeDescription = new TypeDescriptionBuilder()
                .WithDeclaredConstructorsParams(
                    new ParameterDescriptionBuilder()
                        .WithType(ctorParamType)
                        .WithName(ctorParamName)
                        .WithBaseType(true)
                        .WithReturnTypeDescription(CreateReturnTypeDescription(ctorParamType))
                        .Build())
                .Build();

            // act
            var (expectedInner, output) = this.TestBaseMember(
                "CtorParam",
                typeDescription,
                ctorParamName,
                ctorParamType,
                true,
                null,
                true);

            expectedInner = expectedInner.Replace("RootPath", "TzParent");

            var expected = NormalizeCode(
                @$"
public ConstructorMemberPath Ctor => new ConstructorMemberPath(RootPath);
public class ConstructorMemberPath
{{
    private MemberPath<FixtureItemTypeName> TzParent;
    public ConstructorMemberPath(MemberPath<FixtureItemTypeName> parent)
    {{
        this.TzParent = parent;
    }}

    {expectedInner}
}}
");

            // assert
            output = NormalizeCode(Regex.Replace(output, ".*///.*$", "", RegexOptions.Multiline));
            output.Should().Be(expected);
        }



        [Test]
        public void When_the_MemberName_already_exists_in_the_configClass_add_a_postfix_underline_is_added()
        {
            // arrange
            var sut = new PathSourceTextMemberGeneratorBuilder().Build();
            var aTypeFullName = TypeFullName.Create("a");

            var typeDescription = new TypeDescriptionBuilder()
                .WithIsInterface(true)
                .WithDeclaredProperties(
                    Mock.Of<IDesignPropertyDescription>(
                        description =>
                            description.Name == "a" &&
                            description.TypeFullName == aTypeFullName &&
                            description.GetReturnTypeDescription() == CreateReturnTypeDescription(aTypeFullName)))
                .Build();

            // act
            string CreateOutput(params string[] reserved) =>
                sut.GenerateMembers(
                    typeDescription,
                    Maybe.None(),
                    "any",
                    new HashSet<string>(),
                    reserved.ToHashSet(),
                    true,
                    this._compilation,
                    Mock.Of<ITypeSymbol>(),
                    new List<MemberVerificationInfo>(),
                    "FixtureItemTypeName");

            var output1 = CreateOutput();
            var output2 = CreateOutput("a");
            var output3 = CreateOutput("a", "a_");
            var output4 = CreateOutput("a", "a_", "a__");

            output1.Should().NotBeEmpty();
            output1.Should().Match("*MemberPath a *");

            output2.Should().NotMatch("*MemberPath a *");
            output2.Should().Match("*MemberPath a_ *");

            output3.Should().NotMatch("*MemberPath a_ *");
            output3.Should().Match("*MemberPath a__ *");

            output4.Should().NotMatch("*MemberPath a__ *");
            output4.Should().Match("*MemberPath a___ *");
        }

        [Test]
        public void Illegal_members_get_replaced()
        {
            // arrange
            var sut = new PathSourceTextMemberGeneratorBuilder()
                .Build();

            var methodType = TypeFullName.CreateFromType(typeof(List<int>));
            var propertyDescription = TypeFullName.Create("a");

            var typeDescription = new TypeDescriptionBuilder()
                .WithIsInterface(true)
                .WithDeclaredMethods(
                    new MethodDescriptionBuilder()
                        .WithName("a")
                        .WithType(methodType.FullName)
                        .WithReturnType(CreateReturnTypeDescription(methodType))
                        .Build<IDesignMethodDescription>())
                .WithDeclaredProperties(
                    Mock.Of<IDesignPropertyDescription>(
                        description =>
                            description.Name == "this[]" &&
                            description.TypeFullName == propertyDescription &&
                            description.GetReturnTypeDescription() == CreateReturnTypeDescription(propertyDescription)))
                .Build();

            // act
            var output = sut.GenerateMembers(
                typeDescription,
                Maybe.None(),
                "any",
                new HashSet<string>(),
                new HashSet<string>(),
                true,
                this._compilation,
                Mock.Of<ITypeSymbol>(),
                new List<MemberVerificationInfo>(),
                "FixtureItemTypeName");

            var identifiers = CSharpSyntaxTree.ParseText(output)
                .GetRoot()
                .DescendantNodesAndSelf()
                .OfType<PropertyDeclarationSyntax>()
                .Select(syntax => syntax.Identifier.ValueText)
                .ToList();

            identifiers.Should().HaveCountGreaterThan(1);

            identifiers.Should()
                .NotContain(
                    s =>
                        s.Contains('<') || s.Contains('>') || s.Contains('[') || s.Contains(']') || s.Contains('´')
                );
        }

        [Test]
        public void Member_generation_also_generates_deeper_level()
        {
            // arrange
            var sut = new PathSourceTextMemberGeneratorBuilder().Build();
            var innerPropType = TypeFullName.Create("InnerPropType");
            var outerPropType = TypeFullName.Create("OuterPropType");

            var propDescription = CreateReturnTypeDescription(
                new TypeDescriptionBuilder()
                .WithIsInterface(true)
                .WithDeclaredProperties(
                    Mock.Of<IDesignPropertyDescription>(
                        description =>
                            description.Name == "InnerProp" &&
                            description.TypeFullName == innerPropType &&
                            description.GetReturnTypeDescription() == CreateReturnTypeDescription(innerPropType))),
                "OuterProp");

            var typeDescription = new TypeDescriptionBuilder()
                .WithIsInterface(true)
                .WithDeclaredProperties(
                    Mock.Of<IDesignPropertyDescription>(
                        description =>
                            description.Name == "OuterProp" &&
                            description.TypeFullName == outerPropType &&
                            description.GetReturnTypeDescription() == propDescription))
                .Build();

            // act
            var output = sut.GenerateMembers(
                typeDescription,
                Maybe.Some(
                    PathNode.ConstructRoot(
                        new[]
                        {
                            new[] { "OuterProp" },
                        })),
                "any",
                new HashSet<string>(),
                new HashSet<string>(),
                true,
                this._compilation,
                Mock.Of<ITypeSymbol>(),
                new List<MemberVerificationInfo>(),
                "FixtureItemTypeName");

            // assert
            Console.WriteLine(output);

            var rootNode = CSharpSyntaxTree.ParseText(output).GetRoot();

            // expect one inner classes
            rootNode.DescendantNodesAndSelf()
                .OfType<ClassDeclarationSyntax>()
                .Count()
                .Should()
                .Be(2);

            var identifiers = rootNode.DescendantNodesAndSelf()
                .OfType<PropertyDeclarationSyntax>()
                .Select(syntax => syntax.Identifier.ValueText)
                .ToList();

            identifiers.Should().Contain("OuterProp");
            identifiers.Should().Contain("InnerProp");
        }

        [Test]
        public void When_pathNode_provides_a_typeSymbol_used_it_for_inner_classes()
        {
            // arrange
            var newPropDescription = new TypeDescriptionBuilder()
                .WithIsInterface(true)
                .WithTypeFullName(TypeFullName.CreateFromType(typeof(IList<int>)))
                .WithReturnTypeDescription(
                    CreateReturnTypeDescription(TypeFullName.CreateFromType(typeof(IList<int>))))
                .Build();

            var sut = new PathSourceTextMemberGeneratorBuilder()
                .With(p => p._descriptionFactory.CreateDescription.Value(newPropDescription))
                .Build();

            var propDescription = CreateReturnTypeDescription(new TypeDescriptionBuilder()
                .WithIsInterface(true)
                .WithTypeFullName(TypeFullName.CreateFromType(typeof(IEnumerable<int>))),
                "IEnumerable<int>");

            var typeDescription = new TypeDescriptionBuilder()
                .WithIsInterface(true)
                .WithTypeFullName(TypeFullName.CreateFromType(typeof(IEnumerable<int>)))
                .WithDeclaredProperties(
                    Mock.Of<IDesignPropertyDescription>(
                        description =>
                            description.Name == "Prop" &&
                            description.TypeFullName == TypeFullName.Create("OuterPropType") &&
                            description.GetReturnTypeDescription() == propDescription))
                .Build();

            var typeSymbol = new TypeSymbolBuilder()
                .WithName("NewType")
                .WithNamespace("MyNamespace")
                .Build();

            // act
            var output = sut.GenerateMembers(
                typeDescription,
                Maybe.Some(
                    PathNode.ConstructRoot(
                        new[]
                        {
                            new[] { ("Prop", Maybe.Some(typeSymbol), Maybe.None<InvocationExpressionSyntax>()) },
                        })),
                "any",
                new HashSet<string>(),
                new HashSet<string>(),
                true,
                this._compilation,
                Mock.Of<ITypeSymbol>(),
                new List<MemberVerificationInfo>(),
                "FixtureItemTypeName");

            // assert
            Console.WriteLine(output);

            var rootNode = CSharpSyntaxTree.ParseText(output).GetRoot();

            // expect one inner classes
            rootNode.DescendantNodesAndSelf()
                .OfType<ClassDeclarationSyntax>()
                .Count()
                .Should()
                .Be(1);

            var classNode = rootNode.DescendantNodesAndSelf()
                .OfType<ClassDeclarationSyntax>()
                .First();

            var baseTypeNode = classNode.BaseList.Types
                .First();

            var genericName = baseTypeNode.Type as GenericNameSyntax;
            genericName.TypeArgumentList.Arguments[1].ToFullString()
                .Should().Be("System.Collections.Generic.IList<System.Int32>");
        }

        [Test]
        public void Inner_classes_get_renamed_when_they_whould_be_named_the_same_as_the_parent()
        {
            var builder = new TypeDescriptionBuilder()
                .AsInterface()
                .WithDeclaredMethods(
                    new MethodDescriptionBuilder()
                        .WithName("MyMethod")
                        .WithType((ITypeFullName)TypeFullName.Create("MyClass"))
                        .Build<IDesignMethodDescription>());

            var myClassDescription = CreateReturnTypeDescription(builder, "MyClass");

            var myClassDescription2 = CreateReturnTypeDescription(
                new TypeDescriptionBuilder()
                    .AsInterface()
                    .WithDeclaredMethods(
                        new MethodDescriptionBuilder()
                            .WithName("MyMethod")
                            .WithReturnType(myClassDescription)
                            .WithType((ITypeFullName)TypeFullName.Create("MyClass"))
                            .Build<IDesignMethodDescription>()),
                "MyClass");

            var description = CreateReturnTypeDescription(
                new TypeDescriptionBuilder()
                .AsInterface()
                .WithDeclaredMethods(
                    new MethodDescriptionBuilder()
                        .WithName("MyMethod")
                        .WithReturnType(myClassDescription2)
                        .WithType((ITypeFullName)TypeFullName.Create("MyClass"))
                        .Build<IDesignMethodDescription>()),
                "MyClass");

            var sut = new PathSourceTextMemberGeneratorBuilder()
                .With(p => p._descriptionFactory.CreateDescription.Value(description))
                .Build();

            var parent = PathNode.ConstructRoot(
                new[]
                {
                    new[] { "MyMethod", "MyMethod", "MyMethod" }
                });

            var code = sut.GenerateMembers(
                description,
                Maybe.Some(parent),
                "Test",
                new HashSet<string>(),
                new HashSet<string>(),
                true,
                this._compilation,
                Mock.Of<ITypeSymbol>(),
                new List<MemberVerificationInfo>(),
                "FixtureItemTypeName");

            Console.WriteLine(code);

            code.Should().Contain("Tz_MyMethodMemberPath")
                .And.Contain("Tz_MyMethodMemberPath_");
        }

        [Test(Description = "Bug 2563")]
        public void Method_return_type_get_generated()
        {
            var myClassDescription = CreateReturnTypeDescription(
                new TypeDescriptionBuilder()
                .WithDeclaredProperties(
                    new PropertyDescriptionBuilder()
                        .WithName("Value")
                        .WithReturnType(CreateReturnTypeDescription(TypeFullName.CreateFromType(typeof(int))))
                        .WithType(TypeFullName.CreateFromType(typeof(int)))
                        .Build<IDesignPropertyDescription>()),
                "Int32");

            var description = CreateReturnTypeDescription(
                new TypeDescriptionBuilder()
                .WithDeclaredMethods(
                    new MethodDescriptionBuilder()
                        .WithName("MyMethod")
                        .WithType((ITypeFullName)TypeFullName.Create("MyClass"))
                        .WithReturnType(myClassDescription)
                        .Build<IDesignMethodDescription>())
                .AsInterface(),
                "MyClass");

            var sut = new PathSourceTextMemberGeneratorBuilder()
                .Build();

            var parent = PathNode.ConstructRoot(
                new[]
                {
                    new[] { "MyMethod", "Value" }
                });

            var code = sut.GenerateMembers(
                description,
                Maybe.Some(parent),
                "Test",
                new HashSet<string>(),
                new HashSet<string>(),
                true,
                this._compilation,
                Mock.Of<ITypeSymbol>(),
                new List<MemberVerificationInfo>(),
                "FixtureItemTypeName");

            var syntaxTree = CSharpSyntaxTree.ParseText(code);

            Console.WriteLine(code);

            var memberPathClass = syntaxTree
                .GetRoot()
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .First(syntax => syntax.Identifier.ToString() == "Tz_MyMethodMemberPath");

            var propertyDeclaration = memberPathClass.Members
                .OfType<PropertyDeclarationSyntax>()
                .Where(syntax => syntax.Identifier.ToString() == "Value_");

            propertyDeclaration.Should().HaveCount(1);
        }


        [Test(Description = "Bug 3388")]
        public void Generics_get_resolved_by_their_fullName()
        {
            var type = typeof(IEnumerable<IEnumerable<Task<int>>>);

            var description = CreateReturnTypeDescription(
                new TypeDescriptionBuilder()
                .WithDeclaredProperties(
                    new PropertyDescriptionBuilder()
                        .WithName("Value")
                        .WithReturnType(CreateReturnTypeDescription(TypeFullName.CreateFromType(type)))
                        .WithType(TypeFullName.CreateFromType(type))
                        .Build<IDesignPropertyDescription>()),
                "Int32");

            var sut = new PathSourceTextMemberGeneratorBuilder()
                .Build();

            var parent = PathNode.ConstructRoot(
                new[]
                {
                    new[] {"Value" }
                });

            var code = sut.GenerateMembers(
                description,
                Maybe.Some(parent),
                "Test",
                new HashSet<string>(),
                new HashSet<string>(),
                true,
                this._compilation,
                Mock.Of<ITypeSymbol>(),
                new List<MemberVerificationInfo>(),
                "FixtureItemTypeName");

            Console.WriteLine(code);

            code.Should()
                .Contain(
                    "System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task<System.Int32>>>>");
        }

        [Test]
        public void AccessModifier_is_generated_to_the_lowest_AccesModifierOf_of_the_fixtureItem()
        {
            var type = typeof(IEnumerable<IEnumerable<Task<int>>>);
            var typeFullName = TestHelper.RandomTypeFullName();

            var symbol = Mock.Of<INamedTypeSymbol>(symbol =>
                symbol.MetadataName == typeFullName.FullName &&
                symbol.ContainingNamespace ==
                Mock.Of<INamespaceSymbol>(typeSymbol => typeSymbol.IsGlobalNamespace == true) &&
                symbol.DeclaredAccessibility == Accessibility.Public &&
                symbol.IsGenericType == true &&
                symbol.TypeArguments ==
                ImmutableArray<ITypeSymbol>.Empty
                    .Add(Mock.Of<INamedTypeSymbol>(x =>
                        x.DeclaredAccessibility == Accessibility.Internal))
            );

            var description = new TypeDescriptionBuilder()
                .WithDeclaredProperties(
                    new PropertyDescriptionBuilder()
                        .WithName("Value")
                        .WithReturnType(CreateReturnTypeDescription(TypeFullName.CreateFromType(type), symbol))
                        .WithType(TypeFullName.CreateFromType(type))
                        .Build<IDesignPropertyDescription>()
                )
                .Build();

            var sut = new PathSourceTextMemberGeneratorBuilder().Build();

            var parent = PathNode.ConstructRoot(
                new[]
                {
                    new[] {"Value" }
                });

            var code = sut.GenerateMembers(
                description,
                Maybe.Some(parent),
                "Test",
                new HashSet<string>(),
                new HashSet<string>(),
                true,
                this._compilation,
                Mock.Of<ITypeSymbol>(),
                new List<MemberVerificationInfo>(),
                "FixtureItemTypeName");

            Console.WriteLine(code);

            code.Should()
                .Contain(
                    $"internal Tz_ValueMemberPath Value");
        }

        [Test(Description = "Bug 4380")]
        public void Nested_member_are_not_required_to_be_unique()
        {
            // arrange
            var parameter = new ParameterDescriptionBuilder()
                .WithName("input")
                .Build();

            var successCtorDesc = Result.Success(new ItemBuilder<IMethodDescription>()
                .With(p => p.DeclaredParameters.Value(ImmutableArray<IParameterDescription>.Empty.Add(parameter)))
                .Build());

            var description = new TypeDescriptionBuilder()
                .WithDeclaredProperties(new PropertyDescriptionBuilder()
                    .WithBaseType(true)
                    .WithName("input")
                    .Build())
                .WithDeclaredConstructorsParams(parameter)
                .Build();

            var sut = new ItemBuilder<PathSourceTextMemberGenerator>()
                .With(p => p.Ctor.ctorSelector.GetCtorDescription.Value(successCtorDesc))
                .With(p => p.Ctor.ctorSelector.GetCtorDescription.Value(successCtorDesc))
                .Build();

            // act
            var code = sut.GenerateMembers(
                description,
                Maybe.None(),
                "TestFixture",
                new HashSet<string>(),
                new HashSet<string>(),
                true,
                this._compilation,
                Mock.Of<ITypeSymbol>(),
                new List<MemberVerificationInfo>(),
                "FixtureItemTypeName");

            // assert
            Console.WriteLine(NormalizeCode(code));
            code.Should().NotContain("input_");
        }

        [Test]
        public void When_generic_and_non_generic_methods_with_the_same_name_exists_generate_both()
        {
            // arrange
            var myClassDescription = CreateReturnTypeDescription(TypeFullName.Create("MyClass"));

            var m1 = new MethodDescriptionBuilder()
                .WithName("MyMethod")
                .WithType((ITypeFullName)TypeFullName.Create("MyClass"))
                .WithReturnType(myClassDescription)
                .Build<IDesignMethodDescription>();

            var m2 = new MethodDescriptionBuilder()
                .WithName("MyMethod")
                .WithType((ITypeFullName)TypeFullName.Create("MyClass"))
                .WithReturnType(myClassDescription)
                .WithGenericParameters(new GenericParameterType(Maybe.None(), "T", 0, ImmutableArray<ITypeFullName>.Empty))
                .Build<IDesignMethodDescription>();

            var description = new TypeDescriptionBuilder()
                .WithDeclaredMethods(m1, m2)
                .AsInterface()
                .Build();

            var sut = new PathSourceTextMemberGeneratorBuilder().Build();

            // act
            var code = sut.GenerateMembers(
                description,
                Maybe.None(),
                "TestFixture",
                new HashSet<string>(),
                new HashSet<string>(),
                true,
                this._compilation,
                Mock.Of<ITypeSymbol>(),
                new List<MemberVerificationInfo>(),
                "FixtureItemTypeName");

            // assert
            Console.WriteLine(NormalizeCode(code));
            code.Should().Contain("MyMethod ");
            code.Should().Contain("MyMethodT");
        }

        private (string expected, string output) TestMember(
            string memberKind,
            ITypeDescription typeDescription,
            string memberName,
            TypeFullName memberType,
            bool makeStatic = true,
            string memberPropertyName = null,
            TypeFullName methodReturnType = null)
        {
            memberPropertyName ??= memberName;

            var methodReturnTypeName = (methodReturnType != null)
                ? $", {methodReturnType.GetFriendlyCSharpTypeName()}"
                : string.Empty;

            var memberTypeName = memberType.GetFriendlyCSharpTypeName() + methodReturnTypeName;
            var fixtureItemTypeName = "FixtureItemTypeName";
            var innerClassName = $"Tz_{memberPropertyName}MemberPath";
            var parameters = (memberKind == "Method") ? ", params Type[] parameters" : string.Empty;
            var parametersBase = (memberKind == "Method") ? ", parameters" : string.Empty;

            var attribute =
                memberName != memberPropertyName
                    ? $@"[OriginalName(""{memberName}"")]"
                    : string.Empty;

            var expectedCode = @$"
{attribute}
public {innerClassName} {memberPropertyName} => new {innerClassName}(RootPath);

public class {innerClassName} : {memberKind}MemberPath<{fixtureItemTypeName}, {memberTypeName}>
{{
        public {innerClassName}(MemberPath<{fixtureItemTypeName}> parent{parameters}): base(""{memberName}"", parent{parametersBase})
        {{
        }}
}}
";

            return this.TestMember(typeDescription, fixtureItemTypeName, expectedCode, makeStatic);
        }

        private static string GetRandomMemberName() =>
            Build.New<string>().Replace("-", string.Empty).Replace(" ", string.Empty);

        private static ITypeDescription CreateReturnTypeDescription(ITypeFullName typeFullName) =>
            CreateReturnTypeDescription(
                typeFullName,
                Mock.Of<ITypeSymbol>(
                    symbol =>
                        symbol.MetadataName == typeFullName.FullName &&
                        symbol.ContainingNamespace ==
                        Mock.Of<INamespaceSymbol>(typeSymbol => typeSymbol.IsGlobalNamespace == true)));

        private static ITypeDescription CreateReturnTypeDescription(ITypeFullName typeFullName, ITypeSymbol typeSymbol) =>
            new TypeDescriptionBuilder()
                .WithTypeFullName(typeFullName)
                .Build(
                    m =>
                    {
                        var m2 = m.As<IRoslynTypeDescription>();

                        m2.Setup(description => description.GetTypeSymbol())
                            .Returns(
                                () => typeSymbol);

                        return m2.As<ITypeDescription>();
                    });

        private static ITypeDescription CreateReturnTypeDescription(TypeDescriptionBuilderBase<ITypeDescription> builder, string fullName) =>
            builder
                .Build(
                    m =>
                    {
                        var m2 = m.As<IRoslynTypeDescription>();

                        m2.Setup(description => description.GetTypeSymbol())
                            .Returns(
                                () => Mock.Of<ITypeSymbol>(
                                    symbol =>
                                        symbol.MetadataName == fullName &&
                                        symbol.ContainingNamespace ==
                                        Mock.Of<INamespaceSymbol>(typeSymbol => typeSymbol.IsGlobalNamespace == true)));

                        return m2.As<ITypeDescription>();
                    });

        private (string expected, string output) TestMember(
            ITypeDescription typeDescription,
            string fixtureItemTypeName,
            string expectedCode,
            bool makeStatic,
            bool configCtorSelector = false)
        {
            Result<IMethodDescription, InvalidTypeDescriptionFailure> ToSuccess() =>
                (Result<IMethodDescription, InvalidTypeDescriptionFailure>)
                typeDescription.GetDeclaredConstructors().FirstOrDefault().ToSuccess<IMethodDescription, InvalidTypeDescriptionFailure>();

            var ctorSelector = configCtorSelector
                ? new ItemBuilder<ICtorSelector>()
                    .With(p => p.GetCtorDescription.Value(ToSuccess()))
                    .Build()
                : new ItemBuilder<ICtorSelector>().Build();

            var sut = new PathSourceTextMemberGeneratorBuilder()
                .With(p => p.Ctor.ctorSelector.Value(ctorSelector))
                .Build();

            var output = sut.GenerateMembers(
                typeDescription,
                Maybe.None(),
                fixtureItemTypeName,
                new HashSet<string>(),
                Mock.Of<HashSet<string>>(),
                makeStatic,
                this._compilation,
                Mock.Of<ITypeSymbol>(),
                new List<MemberVerificationInfo>(),
                "FixtureItemTypeName");

            output = Regex.Replace(output, @"^.*\/\/\/.*$", string.Empty, RegexOptions.Multiline);

            output = Regex.Replace(output,
                @"public MemberConfig.*\n.*this\.Delegate.*;",
                string.Empty,
                RegexOptions.Multiline);

            output = Regex.Replace(output,
                @"public MemberConfig.*\n.*this\.Delegate.*;",
                string.Empty,
                RegexOptions.Multiline);

            output = Regex.Replace(output,
                @"public MemberConfig.*\n.*this\.RegisterCallback.*;",
                string.Empty,
                RegexOptions.Multiline);

            return (NormalizeCode(expectedCode), NormalizeCode(output));
        }

        private (string expected, string output) TestBaseMember(
            string memberKind,
            ITypeDescription typeDescription,
            string memberName,
            TypeFullName memberType,
            bool makeStatic = true,
            string memberPropertyName = null,
            bool configCtorSelector = false,
            TypeFullName methodReturnType = null)
        {
            memberPropertyName ??= memberName;

            var methodReturnTypeName = (methodReturnType != null)
                ? $", {methodReturnType.GetFriendlyCSharpTypeName()}"
                : string.Empty;

            var memberTypeName = memberType.GetFriendlyCSharpTypeName() + methodReturnTypeName;
            var fixtureItemTypeName = "FixtureItemTypeName";
            var parent = makeStatic ? "RootPath" : "this";
            var innerClassName = $"Tz_{memberPropertyName}MemberPath";
            var attributeName = memberName != memberPropertyName
                    ? @$"[OriginalName(""{memberName}"")]"
                    : string.Empty;

            if (memberKind == "ConstructorParameter")
            {
                parent = "_parent";
            }

            var expectedCode = @$"
{attributeName}
public {innerClassName} {memberPropertyName} => new {innerClassName}({parent});

public class {innerClassName} : {memberKind}BasetypeMemberPath <{fixtureItemTypeName}, {memberTypeName}>
{{
    public {innerClassName}(MemberPath<{fixtureItemTypeName}> parent) 
        : base(""{memberName}"", parent)
    {{
        
    }}
}}
";

            return this.TestMember(typeDescription, fixtureItemTypeName, expectedCode, makeStatic, configCtorSelector);
        }

        private static string NormalizeCode(string code) =>
            CSharpSyntaxTree.ParseText(code).GetRoot().NormalizeWhitespace().ToString();
    }
}
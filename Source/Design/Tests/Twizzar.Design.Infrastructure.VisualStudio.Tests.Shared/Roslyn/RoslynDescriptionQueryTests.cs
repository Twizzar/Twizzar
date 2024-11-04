using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Dummies;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Shared.Infrastructure.Description;
using Twizzar.SharedKernel.CoreInterfaces.Failures;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.ResultMonad;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Roslyn;

[Category("TwizzarInternal")]
public class RoslynDescriptionQueryTests
{
    #region fields

    private Workspace _roslynWorkspace;
    private string _rootItemName;
    private IReadOnlyList<string> _references;
    private IProjectNameQuery _projectQuery;

    #endregion

    #region members

    [SetUp]
    public void Setup()
    {
        this._rootItemName = RoslynWorkspaceBuilder.ProjectName;

        this._roslynWorkspace = new RoslynWorkspaceBuilder()
            .AddReference(typeof(RoslynDescriptionQueryTests).Assembly.Location)
            .AddReference(typeof(int).Assembly.Location)
            .AddReference(typeof(int[]).Assembly.Location)
            .AddReference(typeof(ITypeDescription).Assembly.Location)
            .AddReference(typeof(Failure).Assembly.Location)
            .Build();

        this._references = new[] {typeof(RoslynDescriptionQueryTests).Assembly.Location, typeof(int).Assembly.Location};

        this._projectQuery = new ItemBuilder<IProjectNameQuery>()
            .With(p => p.GetProjectNameAsync.Value(Result.SuccessAsync<string, Failure>(this._rootItemName)))
            .Build();
    }


    [Test]
    public async Task Returned_typeDescription_contains_members_from_implemented_interfaces()
    {
        var sut = this.CreateSut();

        var typeDescriptionDescriptionResult = await sut.GetTypeDescriptionAsync(TypeFullName.CreateFromType(typeof(ITypeDescription)), this._rootItemName);
        var baseDescriptionDescriptionResult = await sut.GetTypeDescriptionAsync(TypeFullName.CreateFromType(typeof(IBaseDescription)), this._rootItemName);

        var typeDescriptionDescription = AssertAndUnwrapSuccess(typeDescriptionDescriptionResult);
        var baseDescriptionDescription = AssertAndUnwrapSuccess(baseDescriptionDescriptionResult);

        typeDescriptionDescription.GetDeclaredProperties().Select(description => description.Name).Should()
            .Contain(baseDescriptionDescription.GetDeclaredProperties().Select(description => description.Name));
    }

    [Test]
    public async Task Returned_typeDescription_contains_members_from_base_type()
    {
        var sut = this.CreateSut();

        var invalidTypeDescriptionFailureDescriptionResult = await sut.GetTypeDescriptionAsync(TypeFullName.CreateFromType(typeof(InvalidTypeDescriptionFailure)), this._rootItemName);
        var failureDescriptionResult = await sut.GetTypeDescriptionAsync(TypeFullName.CreateFromType(typeof(Failure)), this._rootItemName);

        var invalidTypeDescriptionFailureDescription = AssertAndUnwrapSuccess(invalidTypeDescriptionFailureDescriptionResult);
        var failureDescription = AssertAndUnwrapSuccess(failureDescriptionResult);

        invalidTypeDescriptionFailureDescription.GetDeclaredProperties().Select(description => description.Name).Should()
            .Contain(failureDescription.GetDeclaredProperties().Select(description => description.Name));
    }

    [Test]
    [TestCase(typeof(int))]
    [TestCase(typeof(IComparable<int>))]
    [TestCase(typeof(IComparable<int[]>))]
    [TestCase(typeof(int?))]
    [TestCase(typeof(IComparable<Tuple<int, string>>))]
    [TestCase(typeof((int, string)))]
    public async Task Any_type_know_to_the_project_generate_a_valid_typeDescription(Type type)
    {
        // arrange
        var sut = this.CreateSut();

        // act
        var typeDescriptionResult = await sut.GetTypeDescriptionAsync(TypeFullName.CreateFromType(type), this._rootItemName);

        // assert
        var typeDescription = AssertAndUnwrapSuccess(typeDescriptionResult);

        var allBindingFlags = BindingFlags.Instance |
                              BindingFlags.Static |
                              BindingFlags.Public |
                              BindingFlags.NonPublic;

        // for base types we find one default ctor but reflection finds zero ctors.
        // We also don't find the filed m_value.
        if (!typeDescription.IsBaseType)
        {
            typeDescription.GetDeclaredFields().Select(description => description.Name)
                .Should()
                .BeEquivalentTo(type.GetFields(allBindingFlags).Select(info => info.Name));

            // reflection will not find the struct default constructor. Because it is not a method, only a IL instruction (IIRC).
            // some methods like GetType are not found on structs.
            if (!typeDescription.IsStruct)
            {
                typeDescription.GetDeclaredConstructors().Select(description => description.Name)
                    .Should()
                    .BeEquivalentTo(type.GetConstructors().Select(info => info.Name));

                typeDescription.GetDeclaredMethods()
                    .Select(description => description.Name)
                    .Should()
                    .BeEquivalentTo(type
                        .GetMethods(allBindingFlags)
                        .Select(info => info.Name));
            }
        }

        var expectedProperties = type.GetProperties(allBindingFlags).Select(info => info.Name).ToList();

        if (expectedProperties.Any())
        {
            typeDescription.GetDeclaredProperties().Select(description => description.Name)
                .Should()
                .Contain(
                    type.GetProperties(allBindingFlags).Select(info => info.Name));
        }

        if (!typeDescription.IsGeneric)
        {
            typeDescription.ImplementedInterfaceNames.Should()
                .BeEquivalentTo(type.GetInterfaces().Select(t => t.FullName));
        }

        typeDescription.BaseType.Match(name => name.FullName, () => null).Should().Be(type.BaseType?.FullName);

        typeDescription.GenericTypeArguments.Values.Select(name => name.TypeFullName.GetValueUnsafe().FullName).Should()
            .BeEquivalentTo(type.GenericTypeArguments.Select(t => t.FullName));

        typeDescription.TypeFullName.GetNameSpace().Should().Be(type.Namespace);


        typeDescription.IsAbstract.Should().Be(type.IsAbstract);
        typeDescription.IsClass.Should().Be(type.IsClass);
        typeDescription.IsEnum.Should().Be(type.IsEnum);
        typeDescription.IsGeneric.Should().Be(type.IsGenericType);
        typeDescription.IsInterface.Should().Be(type.IsInterface);
        typeDescription.IsNested.Should().Be(type.IsNested);
        typeDescription.IsSealed.Should().Be(type.IsSealed);
        typeDescription.IsStatic.Should().Be(type.IsAbstract && type.IsSealed);
        typeDescription.IsDelegate.Should().Be(typeof(Delegate).IsAssignableFrom(type));
    }

    [Test]
    public void GetReferencedAssemblies_returns_all_references_paths()
    {
        // arrange
        var sut = this.CreateSut();

        // act
        var assemblies = sut.GetReferencedAssembliesAsync(this._rootItemName);

        // assert
        assemblies.Should().NotBeEquivalentTo(this._references);
    }

    [Test]
    public async Task Enum_all_values_are_stored_in_constantValue()
    {
        // arrange
        var sut = this.CreateSut();

        // act
        var result =
            await sut.GetTypeDescriptionAsync(TypeFullName.CreateFromType(typeof(Numbers)), this._rootItemName);

        // assert
        var description = AssertAndUnwrapSuccess(result);
        description.GetDeclaredFields()
            .Where(fieldDescription => fieldDescription.AccessModifier.IsPublic)
            .Should().HaveCount(3);

        var consts = description.GetDeclaredFields()
            .Where(fieldDescription => fieldDescription.AccessModifier.IsPublic)
            .Where(fieldDescription => fieldDescription.IsConstant)
            .ToList();

        consts.Should().HaveCount(3);

        consts.Should().Contain(
            fieldDescription => 
                fieldDescription.Name == "One" &&
                fieldDescription.ConstantValue.IsSome &&
                (int)fieldDescription.ConstantValue.GetValueUnsafe() == 0);

        consts.Should().Contain(
            fieldDescription =>
                fieldDescription.Name == "Two" &&
                fieldDescription.ConstantValue.IsSome &&
                (int)fieldDescription.ConstantValue.GetValueUnsafe() == 1);

        consts.Should().Contain(
            fieldDescription =>
                fieldDescription.Name == "Three" &&
                fieldDescription.ConstantValue.IsSome &&
                (int)fieldDescription.ConstantValue.GetValueUnsafe() == 2);
    }

    [TestCase(typeof(int[]),  new[] { 1 })]
    [TestCase(typeof(int[][]), new[] { 1, 1 })]
    [TestCase(typeof(int[][][]), new[] { 1, 1, 1 })]
    [TestCase(typeof(int[,]), new[] { 2 })]
    [TestCase(typeof(int[,][,,]), new[] { 3, 2 })]
    [TestCase(typeof(int[,,][]), new[] { 1, 3 })]
    public async Task Is_Array_determined_correctly(Type givenType, int[] dim)
    {
        // arrange
        var sut = this.CreateSut();

        var typeFullName = TypeFullName.CreateFromType(givenType);
        typeFullName.ArrayDimension().Should().ContainInOrder(dim);
        typeFullName.ArrayElementType().GetValueUnsafe().FullName.Should().Be(typeof(int).FullName);

        // act
        var result =
            await sut.GetTypeDescriptionAsync(typeFullName, this._rootItemName);


        result.IsSuccess.Should().BeTrue();

        result.GetSuccessUnsafe().IsArray.Should().BeTrue();
        result.GetSuccessUnsafe().TypeFullName.Should().Be(typeFullName);

        ((RoslynTypeDescription)result.GetSuccessUnsafe()).GetTypeSymbol()
            .Should()
            .BeAssignableTo<IArrayTypeSymbol>();
    }

    private RoslynDescriptionQuery CreateSut() =>
        new ItemBuilder<RoslynDescriptionQuery>()
            .With(p => p.Ctor.workspace.Value(this._roslynWorkspace))
            .With(p => p.Ctor.descriptionFactory.Value(new RoslynDescriptionFactoryDummy()))
            .With(p => p.Ctor.projectNameQuery.Value(this._projectQuery))
            .With(p => p.Ctor.typeSymbolQuery.Value(new TypeSymbolQuery()))
            .Build();

    #endregion
}

public enum Numbers
{
    One = 0,
    Two = 1,
    Three = 2,
}
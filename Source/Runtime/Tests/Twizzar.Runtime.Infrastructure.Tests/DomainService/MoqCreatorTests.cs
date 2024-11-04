using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Twizzar.Runtime.Core.FixtureItem.Definition.ValueDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.MemberDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Services;
using Twizzar.Runtime.Infrastructure.DomainService;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.TestCommon;

using TwizzarInternal.Fixture;

namespace Twizzar.Runtime.Infrastructure.Tests.DomainService;

[Category("TwizzarInternal")]
public partial class MoqCreatorTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<MoqCreator>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void Setup_Property_of_base_interface_is_set_correctly()
    {
        // arrange
        var props = ImmutableArray.Create<IPropertyDefinition>()
            .Add(new IsGenericValueDefinitionBuilder().Build());

        var typeDescription = new EmptyIRuntimeTypeDescriptionBuilder()
            .With(p => p.Type.Value(typeof(IRuntimeTypeDescription)))
            .Build();

        var baseTypeCreator = new TrueIBaseTypeUniqueCreatorBuilder().Build();

        var definitionNode = new ItemBuilder<IMockNode>()
            .With(p => p.TypeDescription.Value(typeDescription))
            .With(p => p.Properties.Value(props))
            .With(p => p.Methods.Value(ImmutableArray<IMethodDefinition>.Empty))
            .With(p => p.FixtureItemId.Value(TestHelper.RandomNamedFixtureItemId()))
            .Build();

        var sut = new MoqCreator(Mock.Of<IInstanceCacheRegistrant>(), baseTypeCreator);

        // act
        var instance = sut.CreateInstance(definitionNode);

        // assert
        var description = instance.Should().BeAssignableTo<IRuntimeTypeDescription>().Subject;
        description.IsGeneric.Should().BeTrue();
    }

    [Test]
    public void Setups_method_defined_by_MethodDefinition()
    {
        // arrange
        var methodDefinitions = ImmutableArray.Create<IMethodDefinition>()
            .Add(new Method1DefinitionWithValue53Builder().Build());

        var typeDescription = new ItemBuilder<IRuntimeTypeDescription>()
            .With(p => p.Type.Value(typeof(ITestInterface2)))
            .With(p => p.GetDeclaredProperties
                .Value(ImmutableArray<IPropertyDescription>.Empty))
            .With(p => p.GetDeclaredMethods.Value(
                methodDefinitions
                    .Select(definition => (IMethodDescription)definition.MethodDescription)
                    .ToImmutableArray()))
            .Build();

        var definitionNone = new ItemBuilder<IMockNode>()
            .With(p => p.TypeDescription.Value(typeDescription))
            .With(p => p.Methods.Value(methodDefinitions))
            .With(p => p.FixtureItemId.Value(TestHelper.RandomNamedFixtureItemId()))
            .With(p => p.Properties.Value(ImmutableArray<IPropertyDefinition>.Empty))
            .Build();

        var sut = new MoqCreatorWithBaseType5Builder()
            .Build();

        //act
        var instance = sut.CreateInstance(definitionNone);

        // assert
        var testInterface = instance.Should().BeAssignableTo<ITestInterface>().Subject;
        testInterface.Method1().Should().Be(5);
    }

    [Test]
    public void Setups_method_defined_by_MethodDefinition2()
    {
        // arrange
        var methodDefinition1 = new Method1DefinitionWithValue53Builder().Build();
        var methodDefinition2 = new Method2DefinitionBuilder().Build();

        var methodDefinitions = ImmutableArray.Create<IMethodDefinition>()
            .Add(methodDefinition1)
            .Add(methodDefinition2);

        var typeDescription = new ItemBuilder<IRuntimeTypeDescription>()
            .With(p => p.Type.Value(typeof(ITestInterface2)))
            .With(p => p.GetDeclaredProperties
                .Value(ImmutableArray<IPropertyDescription>.Empty))
            .With(p => p.GetDeclaredMethods.Value(
                methodDefinitions
                    .Select(definition => (IMethodDescription)definition.MethodDescription)
                    .ToImmutableArray()))
            .Build();

        var definitionNode = new ItemBuilder<IMockNode>()
            .With(p => p.TypeDescription.Value(typeDescription))
            .With(p => p.Properties.Value(ImmutableArray<IPropertyDefinition>.Empty))
            .With(p => p.Methods.Value(methodDefinitions))
            .With(p => p.FixtureItemId.Value(TestHelper.RandomNamedFixtureItemId()))
            .Build();

        var sut = new MoqCreatorBuilder()
            .With(p => p.Ctor.baseTypeCreator.Stub<IBaseTypeCreator>())
            .Build(out var scope);

        //act
        var instance = sut.CreateInstance(definitionNode);

        // assert
        instance.Should().BeAssignableTo<ITestInterface2>();

        scope.Verify(p => p.Ctor.baseTypeCreator.CreateInstance__IValueDefinition_IBaseDescription)
            .WhereValueDefinitionIs(methodDefinition1.ValueDefinition)
            .Called(1);

        scope.Verify(p => p.Ctor.baseTypeCreator.CreateInstance__IValueDefinition_IBaseDescription)
            .WhereValueDefinitionIs(methodDefinition2.ValueDefinition)
            .Called(1);
    }

    [Test]
    public void DelegatesCanBeSetuped()
    {
        // arrange
        var methodDefinition = new Method3DefinitionBuilder()
            .With(p => p.ValueDefinition.Value(new DelegateValueDefinition(new Func<int, int, int>((a, b) => a + b))))
            .Build();

        var methodDefinitions = ImmutableArray.Create<IMethodDefinition>()
            .Add(methodDefinition);

        var typeDescription = new ItemBuilder<IRuntimeTypeDescription>()
            .With(p => p.Type.Value(typeof(ITestInterface3)))
            .With(p => p.GetDeclaredProperties
                .Value(ImmutableArray<IPropertyDescription>.Empty))
            .With(p => p.GetDeclaredMethods.Value(
                methodDefinitions
                    .Select(definition => (IMethodDescription)definition.MethodDescription)
                    .ToImmutableArray()))
            .Build();

        var definitionNode = new ItemBuilder<IMockNode>()
            .With(p => p.TypeDescription.Value(typeDescription))
            .With(p => p.Properties.Value(ImmutableArray<IPropertyDefinition>.Empty))
            .With(p => p.Methods.Value(methodDefinitions))
            .With(p => p.FixtureItemId.Value(TestHelper.RandomNamedFixtureItemId()))
            .Build();

        var sut = new MoqCreatorBuilder()
            .With(p => p.Ctor.baseTypeCreator.Stub<IBaseTypeCreator>())
            .Build();

        //act
        var instance = sut.CreateInstance(definitionNode);

        // assert
        var testInterface = instance.Should().BeAssignableTo<ITestInterface3>().Subject;
        var a = TestHelper.RandomInt();
        var b = TestHelper.RandomInt();
        testInterface.Method(a, b).Should().Be(a + b);
    }

    [Test]
    public void ObjectDelegatesCanBeSetuped()
    {
        // arrange
        var methodDefinition = new Method32DefinitionBuilder()
            .With(p => p.ValueDefinition.Value(new DelegateValueDefinition(
                new Func<object, List<int>>(x => new List<int>()))))
            .Build();

        var methodDefinitions = ImmutableArray.Create<IMethodDefinition>()
            .Add(methodDefinition);

        var typeDescription = new ItemBuilder<IRuntimeTypeDescription>()
            .With(p => p.Type.Value(typeof(ITestInterface3)))
            .With(p => p.GetDeclaredProperties
                .Value(ImmutableArray<IPropertyDescription>.Empty))
            .With(p => p.GetDeclaredMethods.Value(
                methodDefinitions
                    .Select(definition => (IMethodDescription)definition.MethodDescription)
                    .ToImmutableArray()))
            .Build();

        var definitionNode = new ItemBuilder<IMockNode>()
            .With(p => p.TypeDescription.Value(typeDescription))
            .With(p => p.Properties.Value(ImmutableArray<IPropertyDefinition>.Empty))
            .With(p => p.Methods.Value(methodDefinitions))
            .With(p => p.FixtureItemId.Value(TestHelper.RandomNamedFixtureItemId()))
            .Build();

        var sut = new MoqCreatorBuilder()
            .With(p => p.Ctor.baseTypeCreator.Stub<IBaseTypeCreator>())
            .Build();

        //act
        var instance = sut.CreateInstance(definitionNode);

        // assert
        var testInterface = instance.Should().BeAssignableTo<ITestInterface3>().Subject;
        testInterface.Method2<int>().Should().NotBeNull();
    }

    [Test]
    public void Callbacks_are_setup_correctly()
    {
        // arrange
        var receivedCallbacks = new List<int>();

        var methodDefinition = new Method3DefinitionBuilder()
            .With(p => p.Callbacks.Value(new object[] { new Action<int, int>((a, b) => receivedCallbacks.Add(a + b)) }))
            .With(p => p.ValueDefinition.Value(Mock.Of<IUndefinedDefinition>()))
            .Build();

        var methodDefinitions = ImmutableArray.Create<IMethodDefinition>()
            .Add(methodDefinition);

        var typeDescription = new ItemBuilder<IRuntimeTypeDescription>()
            .With(p => p.Type.Value(typeof(ITestInterface3)))
            .With(p => p.GetDeclaredProperties
                .Value(ImmutableArray<IPropertyDescription>.Empty))
            .With(p => p.GetDeclaredMethods.Value(
                methodDefinitions
                    .Select(definition => (IMethodDescription)definition.MethodDescription)
                    .ToImmutableArray()))
            .Build();

        var definitionNode = new ItemBuilder<IMockNode>()
            .With(p => p.TypeDescription.Value(typeDescription))
            .With(p => p.Properties.Value(ImmutableArray<IPropertyDefinition>.Empty))
            .With(p => p.Methods.Value(methodDefinitions))
            .With(p => p.FixtureItemId.Value(TestHelper.RandomNamedFixtureItemId()))
            .Build();

        var sut = new MoqCreatorBuilder()
            .With(p => p.Ctor.baseTypeCreator.Stub<IBaseTypeCreator>())
            .Build();

        //act
        var instance = sut.CreateInstance(definitionNode);

        // assert
        var testInterface = instance.Should().BeAssignableTo<ITestInterface3>().Subject;
        var a = TestHelper.RandomInt();
        var b = TestHelper.RandomInt();
        testInterface.Method(a, b);
        receivedCallbacks.Should().HaveCount(1);
        receivedCallbacks.Should().Contain(a + b);
    }

    public interface ITestInterface
    {
        int Method1();
    }

    public interface ITestInterface2 : ITestInterface
    {
        int Method2();
    }

    public interface ITestInterface3
    {
        int Method(int a, int b);
        IList<T> Method2<T>(params T[] a);
    }
}
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Autofac;
using Autofac.Core;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.Runtime.Core.FixtureItem.Definition.ValueDefinitions;
using Twizzar.Runtime.CoreInterfaces.Exceptions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.MemberDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.Runtime.Infrastructure.DomainService;
using Twizzar.Runtime.Infrastructure.Tests.Builder;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;

namespace Twizzar.Runtime.Infrastructure.Tests.DomainService
{
    public partial class ConcreteTypeCreatorTests
    {
        private string _currentRootFixturePath;

        [SetUp]
        public void SetUp()
        {
            this._currentRootFixturePath = TestHelper.RandomString();
        }

        [Test]
        public void When_definition_is_not_a_class_throw_ResolveTypeException()
        {
            // arrange
            var definition = new ItemBuilder<IMockNode>().Build();
            var sut = new ItemBuilder<ConcreteTypeCreator>().Build();

            // act
            Action action = () => sut.CreateInstance(definition);

            // assert
            action.Should().Throw<ResolveTypeException>();
        }

        [Test]
        public void When_update_is_not_called_throw_ResolveTypeException()
        {
            // arrange
            var definition = new ItemBuilder<IClassNode>().Build();
            var sut = new ItemBuilder<ConcreteTypeCreator>().Build();

            // act
            Action action = () => sut.CreateInstance(definition);

            // assert
            action.Should().Throw<ResolveTypeException>();
        }

        [Test]
        public void Ctor_parameters_throw_ArgumentNullException_when_null()
        {
            // assert
            Verify.Ctor<ConcreteTypeCreator>()
                .ShouldThrowArgumentNullException();
        }

        private IClassNode CreateEmptyClassDefinition(IRuntimeTypeDescription typeDescription = null)
        {
            var classNode = new ItemBuilder<IClassNode>()
                .With(p => p.ConstructorParameters.Value(ImmutableArray<IParameterDefinition>.Empty))
                .With(p => p.FixtureItemId.Value(TestHelper.RandomNamelessFixtureItemId()))
                .With(p => p.Properties.Value(ImmutableArray<IPropertyDefinition>.Empty))
                .With(p => p.Fields.Value(ImmutableArray<IFieldDefinition>.Empty))
                .With(p => p.TypeDescription.Value(typeDescription))
                .Build();
            return classNode;
        }

        [Test]
        public void EmptyClass_returns_instance_of_EmptyClass()
        {
            var type = typeof(EmptyTestClass);
            var typeDescription = new RuntimeTypeDescriptionBuilder()
                .WithType(type)
                .AsClass()
                .Build();

            var sut = new ItemBuilder<ConcreteTypeCreator>().Build();
            sut.Update(
                new ItemBuilder<IComponentContext>().Build(),
                new ItemBuilder<IEnumerable<Parameter>>().Build(),
                (s, type) => null,
                type => null);

            var definitionNode = this.CreateEmptyClassDefinition(typeDescription: typeDescription);

            var instance = sut.CreateInstance(definitionNode);

            instance.Should().BeOfType<EmptyTestClass>();
        }

        [Test]
        public void Private_ctors_can_be_used()
        {
            var type = typeof(ClassWithPrivateCtor);
            var typeDescription = new RuntimeTypeDescriptionBuilder()
                .WithType(type)
                .AsClass()
                .Build();

            var sut = new ItemBuilder<ConcreteTypeCreator>().Build();
            sut.Update(
                new ItemBuilder<IComponentContext>().Build(),
                new ItemBuilder<IEnumerable<Parameter>>().Build(),
                (s, type) => null,
                type => null);

            var definitionNode = this.CreateEmptyClassDefinition(typeDescription: typeDescription);

            Mock.Get(definitionNode)
                .Setup(node => node.ConstructorParameters)
                .Returns(ImmutableArray<IParameterDefinition>.Empty
                    .Add(new ItemBuilder<IParameterDefinition>()
                        .With(p => p.ParameterDescription.Type.Value(typeof(int)))
                        .With(p => p.ValueDefinition.InstanceOf<UniqueDefinition>())
                        .Build())
                    .Add(new ItemBuilder<IParameterDefinition>()
                        .With(p => p.ParameterDescription.Type.Value(typeof(string)))
                        .With(p => p.ValueDefinition.InstanceOf<UniqueDefinition>())
                        .Build()));

            var instance = sut.CreateInstance(definitionNode);

            instance.Should().NotBeNull();
            instance.Should().BeOfType<ClassWithPrivateCtor>();
        }
    }

    #region Test Types

#pragma warning disable S3453 // Classes should not have only "private" constructors
#pragma warning disable S1144 // Unused private types or members should be removed
#pragma warning disable IDE0051 // Remove unused private members

    public class EmptyTestClass
    {

    }

    public class ClassWithPrivateCtor
    {
        public int A { get; }
        public string B { get; }

        private ClassWithPrivateCtor(int a, string b)
        {
            this.A = a;
            this.B = b;
        }
    }

    #endregion
}
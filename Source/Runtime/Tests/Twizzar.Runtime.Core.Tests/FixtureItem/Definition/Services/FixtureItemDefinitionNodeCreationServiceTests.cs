//using System;
//using System.Collections.Immutable;
//using System.Linq;
//using FluentAssertions;
//using Moq;
//using NUnit.Framework;
//using Twizzar.Runtime.Core.FixtureItem.Definition.Services;
//using Twizzar.Runtime.Core.FixtureItem.Definition.ValueDefinitions;
//using Twizzar.Runtime.Core.Tests.Builder;
//using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
//using Twizzar.SharedKernel.Core.FixtureItem.Configuration;
//using Twizzar.SharedKernel.CoreInterfaces.Failures;
//using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
//using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
//using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.FixtureConfigurations;
//using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
//using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
//using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
//using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
//using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
//using TwizzarInternal.Fixture;
//using ViCommon.Functional.Monads.MaybeMonad;
//using static ViCommon.Functional.FunctionalCommon;
//using static Twizzar.TestCommon.TestHelper;

//namespace Twizzar.Runtime.Core.Tests.FixtureItem.Definition.Services
//{
//    [Category("ViTestInternal")]
//    public partial class FixtureItemDefinitionNodeCreationServiceTests
//    {
//        #region fields

//        private static readonly IConfigurationSource Source = Mock.Of<IConfigurationSource>();

//        private FixtureItemDefinitionNodeCreationService _sut;

//        #endregion

//        #region FixtureDefinitionNodeFactoryMethod enum

//        public enum FixtureDefinitionNodeFactoryMethod
//        {
//            CreateBaseType,
//            CreateInterfaceNode,
//            CreateClassNode,
//        }

//        #endregion

//        #region members

//        [SetUp]
//        public void Setup()
//        {
//            this._sut = new ItemBuilder<FixtureItemDefinitionNodeCreationService>()
//                .With(p => p.Ctor.factory.Value(new FixtureDefinitionFactoryBuilder().Build()))
//                .Build();
//        }

//        [TestCase(FixtureDefinitionNodeFactoryMethod.CreateBaseType)]
//        [TestCase(FixtureDefinitionNodeFactoryMethod.CreateInterfaceNode)]
//        [TestCase(FixtureDefinitionNodeFactoryMethod.CreateClassNode)]
//        public void All_arguments_are_null_checked(FixtureDefinitionNodeFactoryMethod methodType)
//        {
//            // arrange
//            var typeDescription = new ItemBuilder<IRuntimeTypeDescription>().Build();
//            var id = RandomNamelessFixtureItemId();
//            var configItem = new ItemBuilder<IConfigurationItem>().Build();

//            Action<IRuntimeTypeDescription, FixtureItemId, IConfigurationItem> _sutMethod = methodType switch
//            {
//                FixtureDefinitionNodeFactoryMethod.CreateBaseType =>
//                    (description, itemId, config) => this._sut.CreateBaseType(null, id, configItem),
//                FixtureDefinitionNodeFactoryMethod.CreateInterfaceNode =>
//                    (description, itemId, config) => this._sut.CreateInterfaceNode(null, id, configItem),
//                FixtureDefinitionNodeFactoryMethod.CreateClassNode =>
//                    (description, itemId, config) => this._sut.CreateClassNode(null, id, configItem),
//                _ => throw new ArgumentOutOfRangeException(nameof(methodType), methodType, null),
//            };

//            // act
//            var typeDescriptionIsNull = () => _sutMethod(null, id, configItem);
//            var fixtureItemIdIsNull = () => _sutMethod(typeDescription, null, configItem);
//            var configurationItemIsNull = () => _sutMethod(typeDescription, id, null);

//            // assert
//            typeDescriptionIsNull.Should().Throw<ArgumentNullException>();
//            fixtureItemIdIsNull.Should().Throw<ArgumentNullException>();
//            configurationItemIsNull.Should().Throw<ArgumentNullException>();
//        }

//        #endregion

//        [Test]
//        public void CreateBaseType_description_of_baseTypeKind_with_uniqueMemberConfig_maps_to_uniqueDefinition()
//        {
//            // arrange
//            var typeDescription = new NullableBaseTypeIRuntimeTypeDescriptionBuilder()
//                .Build();

//            var id = RandomNamelessFixtureItemId();

//            var uniqueValueMemberConfiguration = new UniqueValueMemberConfiguration(ConfigurationConstants.BaseTypeMemberName, Source);
//            var memberConfigurations = ImmutableDictionary.Create<string, IMemberConfiguration>()
//                .Add(uniqueValueMemberConfiguration.Name, uniqueValueMemberConfiguration);

//            var configItem = new BaseTypeIConfigurationItemBuilder()
//                .With(p => p.MemberConfigurations.Value(memberConfigurations))
//                .Build();

//            // act
//            var baseTypeNodeResult = this._sut.CreateBaseType(typeDescription, id, configItem);

//            // assert
//            var baseTypeNode = AssertAndUnwrapSuccess(baseTypeNodeResult);
//            baseTypeNode.IsNullable.Should().BeTrue();
//            baseTypeNode.TypeDescription.Should().BeEquivalentTo(typeDescription);
//            baseTypeNode.ValueDefinition.Should().BeAssignableTo<UniqueDefinition>();
//        }

//        [Test]
//        public void CreateBaseType_description_of_BaseTypeKind_with_valueMemberConfig_maps_to_uniqueDefinition()
//        {
//            //this.Property(IsBaseType).Value(true);
//            //this.Property(DefaultFixtureKind).Value(FixtureKind.BaseType);
//            //this.Property(IsClass).Value(false);
//            //this.Property(IsInterface).Value(false);
//            //this.Property(IsNullableBaseType).Value(true);


//            // arrange
//            var typeDescription = new NullableBaseTypeIRuntimeTypeDescriptionBuilder()
//                .Build();

//            var id = RandomNamelessFixtureItemId();

//            var valueMemberConfiguration = new ItemBuilder<ValueMemberConfiguration>().Build();
//            var memberConfigurations = ImmutableDictionary.Create<string, IMemberConfiguration>()
//                .Add(valueMemberConfiguration.Name, valueMemberConfiguration);

//            var configItem = new BaseTypeIConfigurationItemBuilder()
//                .With(p => p.MemberConfigurations.Value(memberConfigurations))
//                .Build();

//            // act
//            var baseTypeNodeResult = this._sut.CreateBaseType(typeDescription, id, configItem);

//            // assert
//            var baseTypeNode = AssertAndUnwrapSuccess(baseTypeNodeResult);
//            baseTypeNode.IsNullable.Should().BeTrue();
//            baseTypeNode.TypeDescription.Should().BeEquivalentTo(typeDescription);
//            var definition = baseTypeNode.ValueDefinition.Should().BeAssignableTo<RawValueDefinition>().Subject;
//            definition.Value.Should().Be(valueMemberConfiguration.Value);
//        }

//        [Test]
//        public void CreateBaseType_Non_baseTypeDescription_throws_argumentException()
//        {
//            // arrange
//            var typeDescription = new ItemBuilder<IRuntimeTypeDescription>().Build();

//            var id = RandomNamelessFixtureItemId();

//            var configItem = new ItemBuilder<IConfigurationItem>().Build();

//            // act
//            Action a = () => this._sut.CreateBaseType(typeDescription, id, configItem);

//            // assert
//            a.Should().Throw<ArgumentException>();
//        }

//        [Test]
//        public void CreateBaseType_More_than_one_MemberConfiguration_returns_InvalidConfigurationFailure()
//        {
//            // arrange
//            var typeDescription = new NullableBaseTypeIRuntimeTypeDescriptionBuilder().Build();

//            var id = RandomNamelessFixtureItemId();

//            var memberConfigurations = new ItemBuilder<UniqueValueMemberConfiguration>().BuildMany(2)
//                .Select(configuration => (IMemberConfiguration)configuration)
//                .ToImmutableDictionary(configuration => configuration.Name, Identity);

//            var configItem = new BaseTypeIConfigurationItemBuilder()
//                .With(p => p.MemberConfigurations.Value(memberConfigurations))
//                .Build();

//            // act
//            var result = this._sut.CreateBaseType(typeDescription, id, configItem);

//            // assert
//            result.IsFailure.Should().BeTrue();
//            var failure = result.GetFailureUnsafe().Should().BeOfType<InvalidConfigurationFailure>().Subject;
//            failure.ConfigurationItem.Should().Be(configItem);
//        }

//        [Test]
//        public void CreateClassNode_ctor_parameter_are_set_in_classNode()
//        {
//            // arrange
//            var (ctorArray, ctorDescription, memberConfigurations) = SetupCtorConfiguration();

//            var ctorSelector = new ItemBuilder<ICtorSelector>()
//                .With(p => p.FindCtor_MaybeIMethodDescription.Value(Maybe.Some<IMethodDescription>(ctorDescription)))
//                .Build();

//            var sut = new ItemBuilder<FixtureItemDefinitionNodeCreationService>()
//                .With(p => p.Ctor.ctorSelector.Value(ctorSelector))
//                .With(p => p.Ctor.factory.Value(new FixtureDefinitionFactoryBuilder().Build()))
//                .Build();

//            var (typeDescription, id, configItem) = SetupTypeDescAndConfig(ctorDescription, ctorArray);

//            // act
//            var result = sut.CreateClassNode(typeDescription, id, configItem);

//            // assert
//            var node = AssertAndUnwrapSuccess(result);
//            node.ConstructorParameters.Should().HaveCount(2);
//            node.ConstructorParameters.Should().Contain(definition => definition.Name == memberConfigurations[0].Name);
//            node.ConstructorParameters.Should().Contain(definition => definition.Name == memberConfigurations[1].Name);
//        }

//        [Test]
//        public void CreateClassNode_ctor_no_matching_ctorParameter_throws_argumentException()
//        {
//            // arrange
//            var (ctorArray, ctorDescription, memberConfigurations) = SetupCtorConfiguration();

//            var ctorSelector = new ItemBuilder<ICtorSelector>()
//                .With(p => p.FindCtor_MaybeIMethodDescription.Value(Maybe.None<IMethodDescription>()))
//                .Build();

//            var sut = new ItemBuilder<FixtureItemDefinitionNodeCreationService>()
//                .With(p => p.Ctor.ctorSelector.Value(ctorSelector))
//                .With(p => p.Ctor.factory.Value(new FixtureDefinitionFactoryBuilder().Build()))
//                .Build();

//            var (typeDescription, id, configItem) = SetupTypeDescAndConfig(ctorDescription, ctorArray);

//            // act
//            var result = sut.CreateClassNode(typeDescription, id, configItem);

//            // assert
//            result.IsFailure.Should().BeTrue();
//            result.GetFailureUnsafe().Should().BeAssignableTo<InvalidConfigurationFailure>();
//        }

//        private static (IRuntimeTypeDescription typeDescription, FixtureItemId id, ConfigurationItem configItem) SetupTypeDescAndConfig(
//            IRuntimeMethodDescription ctorDescription,
//            ImmutableDictionary<string, IMemberConfiguration> ctorArray)
//        {
//            var typeDescription = new ClassIRuntimeTypeDescriptionBuilder()
//                .With(p => p.GetDeclaredConstructors_ImmutableArrayIMethodDescription.Value(
//                    ImmutableArray<IMethodDescription>.Empty.Add(ctorDescription)))
//                .With(p => p.GetDeclaredProperties_ImmutableArrayIPropertyDescription.Value(
//                    ImmutableArray<IPropertyDescription>.Empty))
//                .With(p => p.GetDeclaredMethods_ImmutableArrayIMethodDescription.Value(
//                    ImmutableArray<IMethodDescription>.Empty))
//                .With(p => p.GetDeclaredFields_ImmutableArrayIFieldDescription.Value(
//                    ImmutableArray<IFieldDescription>.Empty))
//                .Build();

//            var id = RandomNamelessFixtureItemId();

//            var configItem = new ItemBuilder<ConfigurationItem>()
//                .With(p => p.Ctor.memberConfigurations.Value(ctorArray))
//                .With(p  => p.Ctor.fixtureConfigurations.Value(ImmutableDictionary<string, IFixtureConfiguration>.Empty))
//                .With(p => p.Ctor.id.Value(id))
//                .Build();

//            return (typeDescription, id, configItem);
//        }

//        private static (ImmutableDictionary<string, IMemberConfiguration> ctorArray, IRuntimeMethodDescription ctorDescription, ImmutableArray<IMemberConfiguration> memberConfigurations) 
//            SetupCtorConfiguration()
//        {
//            var memberConfigurations = new ItemBuilder<ValueMemberConfiguration>()
//                .BuildMany(2)
//                .Cast<IMemberConfiguration>()
//                .ToImmutableArray();

//            var parameters = memberConfigurations
//                .Select(
//                    configuration =>
//                        Mock.Of<IRuntimeParameterDescription>(description =>
//                            description.Name == configuration.Name &&
//                            description.Type == typeof(int)))
//                .Cast<IParameterDescription>()
//                .ToImmutableArray();

//            var ctorConfiguration = new CtorMemberConfiguration(
//                memberConfigurations,
//                ImmutableArray<ITypeFullName>.Empty,
//                Source);

//            var ctorArray = ImmutableDictionary.Create<string, IMemberConfiguration>()
//                .Add(ctorConfiguration.Name, ctorConfiguration);

//            var ctorDescription = new ItemBuilder<IRuntimeMethodDescription>()
//                .With(p => p.DeclaredParameters.Value(parameters))
//                .Build();

//            return (ctorArray, ctorDescription, memberConfigurations);
//        }
//    }
//}
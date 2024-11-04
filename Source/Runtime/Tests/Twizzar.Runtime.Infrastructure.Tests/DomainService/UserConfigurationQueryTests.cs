using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.Fixture;
using Twizzar.Fixture.Member;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Name;
using Twizzar.Runtime.Infrastructure.DomainService;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.TestCommon;
using Twizzar.TestCommon.Configuration.Builders;
using ViCommon.Functional.Monads.MaybeMonad;
using static Twizzar.Fixture.Member.ItemMemberConfigFactory;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;
using Verify = TwizzarInternal.Fixture.Verify;

namespace Twizzar.Runtime.Infrastructure.Tests.DomainService
{
    [TestFixture]
    public partial class UserConfigurationQueryTests
    {
        #region members

        [Test]
        public void Ctor_parameters_throw_ArgumentNullException_when_null()
        {
            // assert
            Verify.Ctor<UserConfigurationQuery<int>>()
                .ShouldThrowArgumentNullException();
        }

        [Test]
        public async Task Simple_value_config_is_initialized_correctly()
        {
            // arrange
            const string configName = "MySuperIntConfig";
            const string memberName = "Value";
            const int value = 5;
            var id = FixtureItemId.CreateNamed(configName, TypeFullName.Create(typeof(int)));

            var path = new PropertyMemberPath<int, int>(memberName, new RootPath<int>(configName));
            
            var config = CreateItemConfig(Value(path, value));
            var sut = new UserConfigurationQuery<int>(Some(config), new ConfigurationItemFactoryBuilder().Build());

            // act
            var result = await sut.GetNamedConfig(id);

            // assert
            result.IsSome.Should().BeTrue();

            // assert config item
            var configurationItem = result.GetValueUnsafe();
            configurationItem.Id.Name.Should().Be(Some(configName));
            configurationItem.Id.TypeFullName.Should().Be(typeof(int).ToTypeFullName());

            // assert members
            var memberConfigurations = configurationItem.MemberConfigurations;
            memberConfigurations.Should().HaveCount(1);
            var (_, memberConfiguration) = memberConfigurations.First();
            memberConfiguration.Name.Should().Be(memberName);

            var valueMemberConfiguration =
                memberConfiguration.Should().BeAssignableTo<ValueMemberConfiguration>().Subject;

            valueMemberConfiguration.Value.Should().Be(value);
        }

        [Test]
        public async Task TestNestedPaths()
        {
            // arrange
            const string configName = "MySuperIntConfig";
            const string memberName = "Value";
            const int value = 5;

            var carId = FixtureItemId.CreateNamed(configName, TypeFullName.Create(typeof(ICar)));

            var engineId =
                FixtureItemId.CreateNamed($"{configName}.{nameof(IEngine)}", typeof(IEngine).ToTypeFullName());

            var root = new RootPath<ICar>(configName);
            var enginePath = new PropertyMemberPath<ICar, IEngine>(nameof(IEngine), root);
            var path = new PropertyMemberPath<ICar, int>(memberName, enginePath);

            var config = CreateItemConfig(Value(path, value));

            var sut = new UserConfigurationQuery<ICar>(Some(config), new ConfigurationItemFactoryBuilder().Build());

            // act
            var carResult = await sut.GetNamedConfig(carId);
            var engineResult = await sut.GetNamedConfig(engineId);

            // assert

            // assert config item
            var carConfig = carResult.AsMaybeValue()
                .Should()
                .BeAssignableTo<SomeValue<IConfigurationItem>>()
                .Subject.Value;

            carConfig.Id.Name.Should().Be(Some(configName));

            var engineConfig = engineResult.AsMaybeValue()
                .Should()
                .BeAssignableTo<SomeValue<IConfigurationItem>>()
                .Subject.Value;

            engineConfig.Id.Name.Should().Be(Some(enginePath.ToString()));

            carConfig.MemberConfigurations.Should().HaveCount(1);

            // Engine link
            var linkMemberConfiguration =
                carConfig.MemberConfigurations.Values.OfType<LinkMemberConfiguration>().First();

            linkMemberConfiguration.ConfigurationLink.TypeFullName.FullName.Should().Be(typeof(IEngine).FullName);
            linkMemberConfiguration.ConfigurationLink.Name.Should().Be(Some(enginePath.ToString()));

            // Value config
            engineConfig.MemberConfigurations.Should().HaveCount(1);
            var valueConfig = engineConfig.MemberConfigurations.Values.OfType<ValueMemberConfiguration>().First();
            valueConfig.Value.Should().Be(value);
        }

        [Test]
        public async Task TestDeepNestedPaths()
        {
            // arrange
            const string configName = "MySuperIntConfig";
            const string memberName = "Value";
            const int value = 5;

            var root = new RootPath<ICar>(configName);
            var path1 = new PropertyMemberPath<ICar, IEngine>(nameof(IEngine), root);
            var path2 = new PropertyMemberPath<ICar, IEngine>(nameof(IEngine), path1);
            var path3 = new PropertyMemberPath<ICar, IEngine>(nameof(IEngine), path2);
            var path = new PropertyMemberPath<ICar, int>(memberName, path3);

            var carId = ToId(root);
            var engine1Id = ToId(path1);
            var engine2Id = ToId(path2);
            var engine3Id = ToId(path3);

            var config = CreateItemConfig(Value(path, value));

            var sut = new UserConfigurationQuery<ICar>(Some(config), new ConfigurationItemFactoryBuilder().Build());

            // act
            var result =
                new[]
                {
                    await sut.GetNamedConfig(carId),
                    await sut.GetNamedConfig(engine1Id),
                    await sut.GetNamedConfig(engine2Id),
                    await sut.GetNamedConfig(engine3Id),
                }.Somes();

            // assert
            result.Should().HaveCount(4);

            // assert config item
            var carConfig = result.First(item => item.Id.TypeFullName.FullName == typeof(ICar).FullName);
            carConfig.Id.Name.Should().Be(Some(configName));

            var engineConfigs = result.Where(item => item.Id.TypeFullName.FullName == typeof(IEngine).FullName)
                .ToList();

            engineConfigs.Should().HaveCount(3);

            carConfig.MemberConfigurations.Should().HaveCount(1);

            var lastConfig = engineConfigs.First(
                item => item.MemberConfigurations.ContainsKey(memberName));

            lastConfig.MemberConfigurations.Should().HaveCount(1);
            var valueConfig = lastConfig.MemberConfigurations.Values.OfType<ValueMemberConfiguration>().First();
            valueConfig.Value.Should().Be(value);
        }

        [Test]
        public async Task TestCtor()
        {
            // arrange
            const string configName = "MySuperIntConfig";
            const string prameterName1 = "a";
            const string prameterName2 = "b";
            const int value1 = 5;

            var root = new RootPath<ICar>(configName);
            var path1 = new CtorParamMemberPath<ICar, int>(prameterName1, root);
            var path2 = new CtorParamMemberPath<ICar, string>(prameterName2, root);

            var config = CreateItemConfig(Value(path1, value1), Unique(path2));

            var sut = new UserConfigurationQuery<ICar>(Some(config), new ConfigurationItemFactoryBuilder().Build());

            // act
            var result = await sut.GetNamedConfig(ToId(root));

            // assert
            var configurationItem = result.AsMaybeValue()
                .Should()
                .BeAssignableTo<SomeValue<IConfigurationItem>>()
                .Subject.Value;

            // assert config item
            configurationItem.Id.Name.Should().Be(Some(configName));
            configurationItem.Id.TypeFullName.Should().Be(typeof(ICar).ToTypeFullName());

            // assert members
            var memberConfigurations = configurationItem.MemberConfigurations;
            memberConfigurations.Should().HaveCount(1);
            var (_, memberConfiguration) = memberConfigurations.First();
            memberConfiguration.Name.Should().Be(".ctor");

            var ctorMemberConfiguration =
                memberConfiguration.Should().BeAssignableTo<CtorMemberConfiguration>().Subject;

            ctorMemberConfiguration.ConstructorParameters.Should().HaveCount(2);

            var valueMemberConfiguration = ctorMemberConfiguration.ConstructorParameters[prameterName1]
                .Should()
                .BeAssignableTo<ValueMemberConfiguration>()
                .Subject;

            valueMemberConfiguration.Name.Should().Be(prameterName1);
            valueMemberConfiguration.Value.Should().Be(value1);

            var uniqueValueMemberConfiguration = ctorMemberConfiguration.ConstructorParameters[prameterName2]
                .Should()
                .BeAssignableTo<UniqueValueMemberConfiguration>()
                .Subject;

            uniqueValueMemberConfiguration.Name.Should().Be(prameterName2);
        }

        [Test]
        public async Task Method_configured_as_stub_is_configured_as_base_type()
        {
            // arrange
            const string configName = "TestConfig";
            const string methodName = nameof(IMethodTest.GetCar);
            var root = new RootPath<IMethodTest>(configName);
            var path1 = new MethodMemberPath<IMethodTest, Car, ICar>(methodName, root);

            var config = CreateItemConfig(ViType<IMethodTest, Car>(path1));

            // act
            var sut = new UserConfigurationQuery<IMethodTest>(Some(config),
                new ConfigurationItemFactoryBuilder().Build());

            // assert
            var result = await sut.GetNamedConfig(ToId(root));

            result.IsSome.Should().BeTrue();
            var memberConfiguration = result.GetValueUnsafe().MemberConfigurations[methodName];

            var methodConfig = memberConfiguration.Should().BeAssignableTo<MethodConfiguration>().Subject;

            var linkMemberConfiguration = methodConfig.ReturnValue.Should()
                .BeAssignableTo<LinkMemberConfiguration>()
                .Subject;

            linkMemberConfiguration.ConfigurationLink.TypeFullName.FullName.Should().Be(typeof(Car).FullName);
        }

        [Test]
        public async Task Links_get_generated_correctly()
        {
            // arrange
            const string configName = "TestConfig";
            const string methodName = nameof(IMethodTest.GetCar);
            var root = new RootPath<IMethodTest>(configName);
            var path1 = new MethodMemberPath<IMethodTest, Car, ICar>(methodName, root);
            var path2 = new PropertyMemberPath<IMethodTest, int>("Speed", path1);

            var config = CreateItemConfig(ViType<IMethodTest, Car>(path1), Value(path2, 4));

            // act
            var sut = new UserConfigurationQuery<IMethodTest>(Some(config),
                new ConfigurationItemFactoryBuilder().Build());

            // assert
            var result = await sut.GetNamedConfig(ToId(root));

            result.IsSome.Should().BeTrue();
            var memberConfiguration = result.GetValueUnsafe().MemberConfigurations[methodName];

            var methodConfig = memberConfiguration.Should().BeAssignableTo<MethodConfiguration>().Subject;

            var linkMemberConfiguration = methodConfig.ReturnValue.Should()
                .BeAssignableTo<LinkMemberConfiguration>()
                .Subject;

            linkMemberConfiguration.ConfigurationLink.TypeFullName.FullName.Should().Be(typeof(Car).FullName);
        }

        [Test]
        public async Task Callbacks_get_mapped_correctly()
        {
            const string configName = "TestConfig";
            const string methodName = nameof(IMethodTest.GetCar);
            var root = new RootPath<IMethodTest>(configName);
            var callback = () => new Car();

            var path = new MethodMemberPath<IMethodTest, ICar, ICar>(methodName, root);

            var callbacks = ImmutableDictionary.Create<IMemberPath<IMethodTest>, IList<object>>()
                .Add(path, new List<object>() { callback });

            var config = new IItemConfig16e8bBuilder()
                .With(p => p.Name.Value(configName))
                .With(p => p.Callbacks.Value(callbacks))
                .Build();

            var sut = new UserConfigurationQuery<IMethodTest>(
                Some(config),
                new ConfigurationItemFactoryBuilder().Build());

            var namedConfig = (await sut.GetNamedConfig(ToId(root))).GetValueUnsafe();
            namedConfig.Callbacks.Should().HaveCount(1);
        }

        [Test]
        public async Task Generic_methods_are_mapped_correctly()
        {
            const string configName = "TestConfig";
            const string methodName = nameof(IMethodTest.GenericMethod);
            var root = new RootPath<IMethodTest>(configName);

            var path = new MethodMemberPath<IMethodTest, object, object>(methodName, root, new[] {"T"}, new TzParameter("a", "T", typeof(object)));

            var config = CreateItemConfig(Value(path, 5));

            var sut = new UserConfigurationQuery<IMethodTest>(Some(config),
                new ConfigurationItemFactoryBuilder().Build());

            var namedConfig = (await sut.GetNamedConfig(ToId(root))).GetValueUnsafe();

            namedConfig.MemberConfigurations.Should().HaveCount(1);
            namedConfig.MemberConfigurations.First().Key.Should().Be("GenericMethodT__T");
            namedConfig.MemberConfigurations.First().Value.Name.Should().Be("GenericMethodT__T");
            var methodConfiguration = namedConfig.MemberConfigurations.First().Value.Should().BeAssignableTo<MethodConfiguration>().Subject;

            methodConfiguration.MethodName.Should().Be("GenericMethod");
            methodConfiguration.ParameterTypes.Single().Should().Be("T");
            var valueMemberConfiguration = methodConfiguration.ReturnValue.Should().BeAssignableTo<ValueMemberConfiguration>().Subject;
            valueMemberConfiguration.Value.Should().Be(5);
        }

        [Test]
        public async Task Generic_nested_methods_are_mapped_correctly()
        {
            const string configName = "TestConfig";
            const string methodName = nameof(IMethodTest.GenericNestedMethod);
            var root = new RootPath<IMethodTest>(configName);

            var path = new MethodMemberPath<IMethodTest, object, object>(methodName, root, new[] { "T" }, new TzParameter("a", "IList<T>", typeof(IList<TzAnyStruct>)));

            var value = new List<int>();
            var config = CreateItemConfig(Value(path, value));

            var sut = new UserConfigurationQuery<IMethodTest>(Some(config),
                new ConfigurationItemFactoryBuilder().Build());

            var namedConfig = (await sut.GetNamedConfig(ToId(root))).GetValueUnsafe();

            namedConfig.MemberConfigurations.Should().HaveCount(1);
            namedConfig.MemberConfigurations.First().Key.Should().Be("GenericNestedMethodT__IListT");
            namedConfig.MemberConfigurations.First().Value.Name.Should().Be("GenericNestedMethodT__IListT");
            var methodConfiguration = namedConfig.MemberConfigurations.First().Value.Should().BeAssignableTo<MethodConfiguration>().Subject;

            methodConfiguration.MethodName.Should().Be("GenericNestedMethod");
            methodConfiguration.ParameterTypes.Single().Should().Be("IList<T>");
            var valueMemberConfiguration = methodConfiguration.ReturnValue.Should().BeAssignableTo<ValueMemberConfiguration>().Subject;
            valueMemberConfiguration.Value.Should().Be(value);
        }

        private static FixtureItemId ToId<T1>(IMemberPath<T1> path) =>
            FixtureItemId.CreateNamed(path.Name, GetPathType(path));

        private static ITypeFullName GetPathType<T>(IMemberPath<T> path) =>
            path switch
            {
                IMethodMemberPath<T> m => m.DeclaredType.ToTypeFullName(),
                _ => path.ReturnType.ToTypeFullName(),
            };

        private static IItemConfig<TFixtureItem> CreateItemConfig<TFixtureItem>(
            params MemberConfig<TFixtureItem>[] configs)
        {
            var configMock = new Mock<IItemConfig<TFixtureItem>>();

            configMock.Setup(config => config.Name)
                .Returns(TestHelper.RandomString());

            configMock.Setup(itemConfig => itemConfig.MemberConfigurations)
                .Returns(
                    configs.Select(
                            config =>
                                new KeyValuePair<IMemberPath<TFixtureItem>, IItemMemberConfig<TFixtureItem>>(
                                    config.ItemMemberConfig.Path,
                                    config.ItemMemberConfig))
                        .ToImmutableDictionary());

            configMock.Setup(config => config.Callbacks)
                .Returns(ImmutableDictionary.Create<IMemberPath<TFixtureItem>, IList<object>>());

             return configMock.Object;
        }

        #endregion
    }

    public interface ICar
    {
    }

    public class Car : ICar
    {
    }

    public interface IEngine
    {
    }

    public interface IMethodTest
    {
        #region members

        ICar GetCar();

        T GenericMethod<T>(T a);

        IList<T> GenericNestedMethod<T>(IList<T> a);

        #endregion
    }
}
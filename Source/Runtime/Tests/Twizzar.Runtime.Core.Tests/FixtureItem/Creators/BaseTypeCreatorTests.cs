//using FluentAssertions;
//using NUnit.Framework;
//using Twizzar.Runtime.Core.FixtureItem.Creators;
//using Twizzar.Runtime.Core.FixtureItem.Definition;
//using Twizzar.Runtime.Core.FixtureItem.Description;
//using Twizzar.SharedKernel.Core.FixtureItem.Description.Services;
//using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
//using TwizzarInternal.Fixture;
//using static Twizzar.TestCommon.TestHelper;
//using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;

//namespace Twizzar.Runtime.Core.Tests.FixtureItem.Creators
//{
//    public partial class BaseTypeCreatorTests
//    {
//        [Test]
//        public void RawValue_enum_returns_the_correct_instance()
//        {
//            // arrange
//            var typeDescription = new ReflectionTypeDescription(typeof(Numbers), new BaseTypeService());

//            var node = new RawValueBaseTypeNodeBuilder()
//                .With(p => p.Ctor.typeDescription.Value(typeDescription))
//                //.With(p => p.Ctor.valueDefinition.Value_.Value(Numbers.One))
//                .Build();

//            var sut = new ItemBuilder<BaseTypeCreator>().Build();

//            // act
//            var instance = sut.CreateInstance(node);
//            instance.Should().Be(Numbers.One);
//        }

//        [Test]
//        public void Unique_enum_return_an_enum_instance()
//        {
//            // arrange
//            var typeDescription = new ReflectionTypeDescription(typeof(Numbers), new BaseTypeService());

//            var node = new ItemBuilder<BaseTypeNode>()
//                .With(p => p.Ctor.typeDescription.Value(typeDescription))
//                .With(p => p.Ctor.fixtureId.Value(RandomNamedFixtureItemId()))
//                .With(p => p.Ctor.isNullable.Value(false))
//                .With(p => p.Ctor.valueDefinition.Stub<IUniqueDefinition>())
//                .Build();

//            var sut = new ItemBuilder<BaseTypeCreator>()
//                .With(p => p.Ctor.baseTypeUniqueCreator.GetNextValue_Object.Value(Numbers.One))
//                .With(p => p.Ctor.baseTypeUniqueCreator.Stub<IBaseTypeUniqueCreator>())
//                .Build();

//            // act
//            var instance = sut.CreateInstance(node);
//            instance.Should().BeAssignableTo<Numbers>();
//        }
//    }

//    public enum Numbers
//    {
//        Zero,
//        One,
//        Two,
//        Three,
//    }
//}
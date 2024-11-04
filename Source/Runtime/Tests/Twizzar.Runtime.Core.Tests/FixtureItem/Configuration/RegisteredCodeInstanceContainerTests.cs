using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Runtime.Core.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Runtime.Core.Tests.FixtureItem.Configuration
{
    [Category("TwizzarInternal")]
    public partial class RegisteredCodeInstanceContainerTests
    {
        private static T Build<T>() => new ItemBuilder<T>().Build();

        [Test]
        public void Ctor_should_throw_ArgumentNullException_when_parameter_is_Null()
        {
            // assert
            Verify.Ctor<RegisteredCodeInstanceContainer>()
                .ShouldThrowArgumentNullException();
        }

        [Test]
        public void Add_when_id_null_throw_ArgumentNullException()
        {
            // arrange
            var sut = Build<RegisteredCodeInstanceContainer>();

            // act
            Action action = () => sut.Add(null, new object());

            // assert
            var exception = action.Should().Throw<ArgumentNullException>().Subject.First();
            exception.ParamName.Should().Be("id");
        }

        [Test]
        public void Get_when_id_null_throw_ArgumentNullException()
        {
            // arrange
            var sut = Build<RegisteredCodeInstanceContainer>();

            // act
            Action action = () => sut.Get(null);

            // assert
            var exception = action.Should().Throw<ArgumentNullException>().Subject.First();
            exception.ParamName.Should().Be("id");
        }

        [Test]
        public void When_adding_an_instance_it_can_be_read_with_get()
        {
            // arrange
            var sut = Build<RegisteredCodeInstanceContainer>();
            var fixtureItem = RandomNamedFixtureItemId();
            var instance = new object();

            // act
            sut.Add(fixtureItem, instance);
            var result = sut.Get(fixtureItem);

            // assert
            var someValue = result.AsMaybeValue().Should().BeAssignableTo<SomeValue<object>>().Subject;
            someValue.Value.Should().Be(instance);
        }

        [Test]
        public void Adding_id_twice_results_in_a_InvalidOperationException()
        {
            // arrange
            var sut = Build<RegisteredCodeInstanceContainer>();
            var id = RandomNamedFixtureItemId();

            // act
            sut.Add(id, new object());
            Action action = () => sut.Add(id, new object());

            // assert
            action.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void Adding_baseType_results_in_a_InvalidOperationException()
        {
            // arrange
            var fullName = RandomTypeFullName();

            var baseTypeService = new ItemBuilder<IBaseTypeService>()
                .With(p => p.IsBaseType.Value(true))
                .Build();
                
            var id = RandomNamedFixtureItemId().WithType(fullName);

            var sut = new RegisteredCodeInstanceContainer(baseTypeService);
            
            // act
            var action = () => sut.Add(id, new object());

            // assert
            action.Should().Throw<InvalidOperationException>();
        }
    }
}
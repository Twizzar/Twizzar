using System;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.TestCommon;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;

#pragma warning disable S1172 // Unused method parameters should be removed
#pragma warning disable CS0649

namespace Twizzar.SharedKernel.Core.Tests.MoveToViCommon
{
    [TestFixture]
    public class MaybeTests
    {
        [Test]
        public void Maybe_default_IsNone()
        {
            // Arrange
            Maybe<int> m = default;

            // Act
            Action a = () => m.GetValueUnsafe();

            // Assert
            m.IsNone.Should().BeTrue();
            a.Should().Throw<NullReferenceException>();
        }

        [Test]
        public void When_some_then_IsSome_is_true_and_IsNone_is_false()
        {
            // Arrange
            var (some, _) = RandomSome();

            // Assert
            some.IsSome.Should().BeTrue();
            some.IsNone.Should().BeFalse();
        }

        [Test]
        public void When_none_then_IsSome_is_true_and_IsNone_is_false()
        {
            // Arrange
            var some = None<string>();

            // Assert
            some.IsSome.Should().BeFalse();
            some.IsNone.Should().BeTrue();
        }

        [Test]
        public void Match_on_some_should_return_some_case()
        {
            // Arrange
            var (some, value) = RandomSome();

            // Act
            var result = some.Match(
                    some: s => s, 
                    none: () => "None");

            // Assert
            result.Should().Be(value);
        }

        [Test]
        public void Match_on_none_should_return_none_case()
        {
            // Arrange
            var none = None<string>();

            // Act
            var result = none.Match(
                some: s => s,
                none: () => "None");

            // Assert
            result.Should().Be("None");
        }

        [Test]
        public void None_without_generic_casts_successfully_to_the_requested_type()
        {
            //Arrange
            static Maybe<string> Get() => None();
            
            // Act
            var none = Get();

            none.IsNone.Should().BeTrue();
        }

        [Test]
        public void Map_which_concats_two_strings_should_return_the_expected_result()
        {
            //Arrange
            var (some, value) = RandomSome();
            var prefix = "test";

            // Act
            some = some.Map(s => prefix + s);

            some.GetValueUnsafe().Should().NotBeNullOrEmpty();
            some.GetValueUnsafe().Should().Be(prefix + value);
        }

        [Test]
        public void Bind_and_linqExpression_and_selectMany_result_in_the_same_value()
        {
            // Arrange
            static Maybe<Person> GetPerson(string name) =>
                Some(new Person(name));

            static Maybe<Address> GetAddress(Person p) =>
                Some(new Address("test street"));

            // Act
            var resultBind = GetPerson("hans")
                .Bind(person => GetAddress(person))
                .Bind<string>(address => address.street);

            var resultLinq = 
                from person in GetPerson("hans")
                from address in GetAddress(person)
                select address.street;

            var resultSelectMany =
                GetPerson("hans").SelectMany(
                    person => GetAddress(person),
                    (person, address) => address.street);

            // Assert
            resultBind.Should().BeEquivalentTo(resultLinq);
            resultLinq.Should().BeEquivalentTo(resultSelectMany);
        }

        [Test]
        public void Flatten_some_returns_inner_value()
        {
            // Arrange
            var (some, value) = RandomSome();
            var packedMany = Some(some);

            // Act
            var flatten = packedMany.Flatten();

            // Assert
            flatten.IsSome.Should().BeTrue();
            flatten.GetValueUnsafe().Should().Be(value);
        }

        [Test]
        public void Flatten_none_is_some()
        {
            // Arrange
            var packedMany = Some(None<string>());

            // Act
            var flatten = packedMany.Flatten();

            // Assert
            flatten.IsSome.Should().BeFalse();
        }

        [Test]
        public void IfSome_on_some_is_executed_but_not_IfNone()
        {
            // Arrange
            int noneCounter = 0;
            string someValue = null;
            var (some, value) = RandomSome();

            // Act
            some.IfNone(() => noneCounter++);
            some.IfSome(s => someValue = value);

            // Assert
            noneCounter.Should().Be(0);
            someValue.Should().Be(value);
        }

        [Test]
        public void IfNone_on_none_is_executed_but_not_IfSome()
        {
            // Arrange
            var noneCounter = 0;
            var someCounter = 0;
            var none = None<string>();

            // Act
            none.IfNone(() => noneCounter++);
            none.IfSome(s => someCounter++);

            // Assert
            noneCounter.Should().Be(1);
            someCounter.Should().Be(0);
        }

        [Test]
        public void GetValueUnsafe_throws_NullReferenceException_on_none()
        {
            // Arrange
            var none = None<string>();

            // Act
            Action action = () => none.GetValueUnsafe();

            // Assert
            action.Should().Throw<NullReferenceException>();
        }

        [Test]
        public void Some_on_same_value_should_be_equal()
        {
            // Arrange
            var value1 = Guid.NewGuid().ToString();
            var value2 = Guid.NewGuid().ToString();

            var some11 = Some(value1);
            var some12 = Some(value1);
            var some21 = Some(value2);

            // Act
            var some11_and_some12_are_equal = some11.Equals(some12);
            var some11_and_some21_are_equal = some11.Equals(some21);

            // Assert
            some11_and_some12_are_equal.Should().BeTrue();
            some11_and_some21_are_equal.Should().BeFalse();
        }

        [Test]
        public void Some_is_not_equal_to_none()
        {
            // Arrange
            var (some, _) = RandomSome();
            var none = None<string>();

            // Act
            var some_and_none_are_equal = some.Equals(none);

            //Assert
            some_and_none_are_equal.Should().BeFalse();
        }

        [Test]
        public void None_is_equal_to_none()
        {
            // Arrange
            var none1 = None<string>();
            var none2 = None<string>();

            // Act
            var nones_are_equal = none1.Equals(none2);

            // Assert
            nones_are_equal.Should().BeTrue();
        }

        [Test]
        public void Maybe_ToString_is_converting_correctly()
        {
            // arrange
            var none = None<string>();
            var some = Some<string>("Test");
            var msg = TestHelper.RandomString();

            // act
            var nonResult = none.ToResult(new Failure(msg));
            var someResult = some.ToResult(new Failure(msg));

            // assert
            nonResult.IsFailure.Should().BeTrue();
            nonResult.GetFailureUnsafe().Message.Should().Be(msg);

            someResult.IsSuccess.Should().BeTrue();
        }

        [Test]
        public void AsPart_is_of_type_None_when_none()
        {
            // arrange
            var none = None<string>();

            // act
            var part = none.AsMaybeValue();

            // assert
            part.Should().BeOfType<NoneValue>();
        }

        [Test]
        public void AsPart_is_of_type_Some_when_some()
        {
            // arrange
            var (some, value) = RandomSome();

            // act
            var part = some.AsMaybeValue();

            // assert
            var somePart = part.Should().BeOfType<SomeValue<string>>().Subject;
            somePart.Value.Should().Be(value);
        }

        private static (Maybe<string> maybe, string value) RandomSome()
        {
            var value = Guid.NewGuid().ToString();
            return (value, value);
        }

        class Person
        {
            public Person(string name) => this.Name = name;

            public readonly string Name;
        }

        class Address
        {
            public Address(string street) => this.street = street;

            public readonly string street;
        }
    }
}

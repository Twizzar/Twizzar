using System;
using FluentAssertions;
using NUnit.Framework;
using ViCommon.Functional.Monads.ResultMonad;
using static ViCommon.Functional.FunctionalCommon;
using static ViCommon.Functional.Monads.ResultMonad.Result;

#pragma warning disable CS0649

namespace Twizzar.SharedKernel.Core.Tests.MoveToViCommon
{
    [TestFixture]
    public class ResultTests
    {
        [Test]
        public void When_success_then_IsSuccess_is_true_and_IsFailure_is_false()
        {
            // Arrange
            var (result, _) = RandomSuccess();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
        }

        [Test]
        public void When_failure_then_IsSuccess_is_true_and_IsFailure_is_false()
        {
            // Arrange
            var (result, _) = RandomFailure();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Match_on_success_should_return_success_case()
        {
            // Arrange
            var (success, value) = RandomSuccess();

            // Act
            var result = success.Match(
                    s => s,
                    f => "None");

            // Assert
            result.Should().Be(value);
        }

        [Test]
        public void Match_on_failure_should_return_failure_case()
        {
            // Arrange
            var (resultFailure, failure) = RandomFailure();

            // Act
            var result = resultFailure.Match(
                s => new Failure("test"),
                Identity);

            // Assert
            result.Should().Be(failure);
        }

        [Test]
        public void Fail_without_generic_casts_successfully_to_the_requested_type()
        {
            // Arrange
            static Result<string, Failure> Get() => Failure(new Failure(Guid.NewGuid().ToString()));

            // Act
            var failure = Get();

            failure.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Map_which_concats_two_strings_should_return_the_expected_result()
        {
            // Arrange
            var (success, value) = RandomSuccess();
            var prefix = "test";

            // Act
            success = success.MapSuccess(s => prefix + s);

            success.GetSuccessUnsafe().Should().NotBeNullOrEmpty();
            success.GetSuccessUnsafe().Should().Be(prefix + value);
        }

        [Test]
        public void Bind_and_linqExpression_and_selectMany_result_in_the_same_value()
        {
            // Arrange
            static IResult<Person, Failure> GetPerson(string name) =>
                Success<Person, Failure>(new Person(name));

            static IResult<Address, Failure> GetAddress(Person p) =>
                Success<Address, Failure>(new Address("test street"));

            // Act
            var resultBind = GetPerson("hans")
                .Bind(person => GetAddress(person))
                .Bind(address => Success<string, Failure>(Success(address.street)));


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
        public void Flatten_success_returns_inner_value()
        {
            // Arrange
            var (success, value) = RandomSuccess();
            var packedResult = Success<IResult<string, Failure>, Failure>(success);

            // Act
            var flatten = packedResult.Flatten();

            // Assert
            flatten.IsSuccess.Should().BeTrue();
            flatten.GetSuccessUnsafe().Should().Be(value);
        }

        [Test]
        public void Flatten_failure_is_failure()
        {
            // Arrange
            var packedMany = 
                Success<IResult<string, Failure>, Failure>(RandomFailure().result);

            // Act
            var flatten = packedMany.Flatten();

            // Assert
            flatten.IsSuccess.Should().BeFalse();
        }

        [Test]
        public void IfSucces_on_success_is_executed_but_not_IfFailure()
        {
            // Arrange
            int failureCounter = 0;
            string successValue = null;
            var (success, value) = RandomSuccess();

            // Act
            success.IfFailure(f => failureCounter++);
            success.IfSuccess(s => successValue = value);

            // Assert
            failureCounter.Should().Be(0);
            successValue.Should().Be(value);
        }

        [Test]
        public void IfFailure_on_failure_is_executed_but_not_IfSuccess()
        {
            // Arrange
            int successCounter = 0;
            Failure failureValue = null;
            var (failure, value) = RandomFailure();

            // Act
            failure.IfFailure(f => failureValue = f);
            failure.IfSuccess(s => successCounter++);

            // Assert
            successCounter.Should().Be(0);
            failureValue.Should().Be(value);
        }

        [Test]
        public void Success_on_same_success_should_be_equal()
        {
            // Arrange
            var value1 = Guid.NewGuid().ToString();
            var value2 = Guid.NewGuid().ToString();

            Result<string, Failure> success11 = Success(value1);
            Result<string, Failure> success12 = Success(value1);
            Result<string, Failure> success21 = Success(value2);

            // Act
            var success11_and_success12_are_equal = success11.Equals(success12);
            var success11_and_success21_are_equal = success11.Equals(success21);

            //Assert
            success11_and_success12_are_equal.Should().BeTrue();
            success11_and_success21_are_equal.Should().BeFalse();
        }

        [Test]
        public void Failure_on_same_failure_should_be_equal()
        {
            // Arrange
            var value1 = Guid.NewGuid().ToString();
            var value2 = Guid.NewGuid().ToString();

            Result<string, Failure> failure11 = new Failure(value1);
            Result<string, Failure> failure12 = new Failure(value1);
            Result<string, Failure> failure21 = new Failure(value2);

            // Act
            var failure11_and_failure12_are_equal = failure11.Equals(failure12);
            var failure11_and_failure21_are_equal = failure11.Equals(failure21);

            //Assert
            failure11_and_failure12_are_equal.Should().BeTrue();
            failure11_and_failure21_are_equal.Should().BeFalse();
        }

        [Test]
        public void Some_is_not_equal_to_failure()
        {
            // Arrange
            var (success, _) = RandomSuccess();
            var (failure, _) = RandomFailure();

            // Act
            var success_and_failure_are_equal = success.Equals(failure);

            //Assert
            success_and_failure_are_equal.Should().BeFalse();
        }

        [Test]
        public void Combine_success_and_failure_results_in_failure()
        {
            // arrange
            var (failure, f) = RandomFailure();
            var (success, _) = RandomSuccess();

            // act
            var combined = success.Combine(failure, (s1, s2) => s1 + s2);

            // assert
            combined.IsFailure.Should().BeTrue();
            combined.GetFailureUnsafe().Should().Be(f);
        }

        [Test]
        public void Combine_failure_and_failure_results_in_first_failure()
        {
            // arrange
            var (f1, f) = RandomFailure();
            var (f2, _) = RandomFailure();

            // act
            var combined = f1.Combine(f2, (s1, s2) => s1 + s2);

            // assert
            combined.IsFailure.Should().BeTrue();
            combined.GetFailureUnsafe().Should().Be(f);
        }

        [Test]
        public void Combine_success_and_success_results_in_combined_success()
        {
            // arrange
            var (success1, v1) = RandomSuccess();
            var (success2, v2) = RandomSuccess();

            // act
            var combined = success1.Combine(success2, (s1, s2) => s1 + s2);

            // assert
            combined.IsSuccess.Should().BeTrue();
            combined.GetSuccessUnsafe().Should().Be(v1 + v2);
        }

        [Test]
        public void Combine_failure_and_failure_results_in_combined_failure()
        {
            // arrange
            var (failure1, v1) = RandomFailure();
            var (failure2, v2) = RandomFailure();

            // act
            var combined = failure1.Combine(failure2, (a, b) => a + b, (f1, f2) => new Failure(f1.Message + f2.Message));

            // assert
            combined.IsFailure.Should().BeTrue();
            combined.GetFailureUnsafe().Message.Should().Be(v1.Message + v2.Message);
        }

        private static (IResult<string, Failure> result, string value) RandomSuccess()
        {
            var value = Guid.NewGuid().ToString();
            return (Success<string, Failure>(value), value);
        }

        private static (IResult<string, Failure> result, Failure failure) RandomFailure()
        {
            var value = Guid.NewGuid().ToString();
            var failure = new Failure(value);
            return (Failure<string, Failure>(failure), failure);
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

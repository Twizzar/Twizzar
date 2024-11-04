using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.Runtime.Core.FixtureItem.Creators.SetupFilters;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using TwizzarInternal.Fixture;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;

namespace Twizzar.Runtime.Core.Tests.FixtureItem.Creators.SetupFilters
{
    public partial class MethodSetupFilterTests
    {
        [Test]
        public void Ctor_parameters_throw_ArgumentNullException_when_null()
        {
            // assert
            Verify.Ctor<MethodSetupFilter>()
                .IgnoreParameter("returnValue")
                .SetupParameter("parameters", Some(Type.EmptyTypes))
                .SetupParameter("returnType", Some(typeof(int)))
                .SetupParameter("methodName", Some("Test"))
                .ShouldThrowArgumentNullException();
        }

        [Test]
        public void Same_methodName_is_matching_when_other_are_none()
        {
            // arrange
            var methodName = new ItemBuilder<string>().Build();
            var sut = new MethodSetupFilter(methodName, null, None());

            var matchingDescription = new ItemBuilder<IRuntimeMethodDescription>()
                .With(p => p.Name.Value(methodName))
                .With(p => p.Type.Value(typeof(string)))
                .Build();

            // act
            var isMatchingDefault = sut.IsMatching(
                new StringIRuntimeMethodDescriptionBuilder()
                    .Build());
            var isMatchingMock = sut.IsMatching(matchingDescription);

            // assert
            isMatchingDefault.Should().BeFalse();
            isMatchingMock.Should().BeTrue();
        }

        [Test]
        public void Same_parameters_is_matching_when_other_are_none()
        {
            // arrange
            var parameters = new[] {typeof(int), typeof(string), typeof(double)};
            var sut = new MethodSetupFilter(None(), null, parameters);

            // act
            var isMatchingWrongParameters =
                sut.IsMatching(
                    new MethodDescriptionParameterBuilder()
                        .WithParameterTypes(typeof(string), typeof(double))
                        .Build());

            var isMatchingRightParameters =
                sut.IsMatching(
                    new MethodDescriptionParameterBuilder()
                        .WithParameterTypes(parameters)
                        .Build());

            // assert
            isMatchingWrongParameters.Should().BeFalse();
            isMatchingRightParameters.Should().BeTrue();
        }

        [Test]
        public void Same_returnType_is_matching_when_other_are_none()
        {
            // arrange
            var returnType = typeof(int);
            var sut = new MethodSetupFilter(None(), 5, None());

            // act
            var isMatchingDefault = sut.IsMatching(new StringIRuntimeMethodDescriptionBuilder().Build());

            var isMatchingCorrectReturnType =
                sut.IsMatching(
                    new ItemBuilder<IRuntimeMethodDescription>()
                        .With(p => p.Type.Value(returnType))
                        .Build());

            // assert
            isMatchingDefault.Should().BeFalse();
            isMatchingCorrectReturnType.Should().BeTrue();
        }

        [Test]
        public void Assignable_returnType_is_matching()
        {
            // arrange
            var returnType = typeof(ITestInterface);
            var sut = new MethodSetupFilter(None(), new TestClass(), None());

            // act
            var isMatchingDefault = sut.IsMatching(new StringIRuntimeMethodDescriptionBuilder().Build());
            var isMatchingAssignableReturnType =
                sut.IsMatching(Mock.Of<IRuntimeMethodDescription>(description => description.Type == returnType));

            // assert
            isMatchingDefault.Should().BeFalse();
            isMatchingAssignableReturnType.Should().BeTrue();
        }

        private interface ITestInterface { }
        private class TestClass : ITestInterface { }
    }
}
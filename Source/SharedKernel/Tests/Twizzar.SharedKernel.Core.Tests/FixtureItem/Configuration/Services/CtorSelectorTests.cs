using System.Collections.Immutable;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.SharedKernel.Core.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.Failures;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.SharedKernel.Core.Tests.FixtureItem.Configuration.Services
{
    public partial class CtorSelectorTests
    {
        #region members

        [Test]
        public void Returns_InvalidTypeDescriptionFailure_when_no_ctor_is_declared()
        {
            // arrange
            var typeDescription = new ItemBuilder<ITypeDescription>().Build();

            var ctorSelector = new ItemBuilder<CtorSelector>().Build();

            // act
            var result = ctorSelector.GetCtorDescription(typeDescription, CtorSelectionBehavior.Max);

            // assert
            result.IsFailure.Should().BeTrue();

            var failure = result.AsResultValue()
                .Should()
                .BeAssignableTo<FailureValue<InvalidTypeDescriptionFailure>>()
                .Subject;

            failure.Value.TypeDescription.Should().Be(typeDescription);
        }

        [Test]
        public void Returns_the_private_ctor_when_no_public_one_is_declared()
        {
            // arrange
            var privateMethodDescription = new PrivateIMethodDescriptionBuilder()
                .With(p =>
                    p.DeclaredParameters.Value(ImmutableArray<IParameterDescription>.Empty))
                .Build();

            var typeDescription = new ClassITypeDescriptionBuilder()
                .With(p =>
                    p.GetDeclaredConstructors.Value(
                        ImmutableArray.Create(privateMethodDescription)))
                .Build();

            // act
            var ctorSelector = new ItemBuilder<CtorSelector>().Build();
            var result = ctorSelector.GetCtorDescription(typeDescription, CtorSelectionBehavior.Max);

            // assert
            var description = TestHelper.AssertAndUnwrapSuccess(result);
            description.Should().Be(privateMethodDescription);
        }

        [Test]
        public void Returns_the_biggest_private_ctor_when_many_are_declared()
        {
            // arrange
            var privateMethodDescription2 = new PrivateIMethodDescriptionBuilder()
                .With(p =>
                    p.DeclaredParameters.Value(ImmutableArray<IParameterDescription>.Empty
                        .AddRange(new ItemBuilder<IParameterDescription>().BuildMany(2))))
                .Build();

            var privateMethodDescription3 = new PrivateIMethodDescriptionBuilder()
                .With(p =>
                    p.DeclaredParameters.Value(ImmutableArray<IParameterDescription>.Empty
                        .AddRange(new ItemBuilder<IParameterDescription>().BuildMany(3))))
                .Build();

            var typeDescription = new ClassITypeDescriptionBuilder()
                .With(p =>
                    p.GetDeclaredConstructors
                        .Value(ImmutableArray.Create(
                            privateMethodDescription2,
                            privateMethodDescription3)))
                .Build();

            // act
            var ctorSelector = new ItemBuilder<CtorSelector>().Build();
            var result = ctorSelector.GetCtorDescription(typeDescription, CtorSelectionBehavior.Max);

            // assert
            var description = TestHelper.AssertAndUnwrapSuccess(result);
            description.Should().Be(privateMethodDescription3);
        }

        [Test]
        public void When_public_and_private_mixed_always_take_public()
        {
            // arrange
            var privateMethodDescription = new PrivateIMethodDescriptionBuilder()
                .With(p => p.DeclaredParameters.Value(ImmutableArray<IParameterDescription>.Empty
                    .AddRange(new ItemBuilder<IParameterDescription>().BuildMany(2))))
                .Build();

            var publicMethodDescription = new IMethodDescriptiond2eeBuilder()
                .With(p =>
                    p.DeclaredParameters.Value(ImmutableArray<IParameterDescription>.Empty
                        .AddRange(new ItemBuilder<IParameterDescription>().BuildMany(3))))
                .Build();

            var typeDescription = new ClassITypeDescriptionBuilder()
                .With(p =>
                    p.GetDeclaredConstructors.Value(ImmutableArray<IMethodDescription>
                        .Empty
                        .Add(privateMethodDescription)
                        .Add(publicMethodDescription)))
                .Build();

            var ctorSelector = new ItemBuilder<CtorSelector>().Build();

            // act
            var result = ctorSelector.GetCtorDescription(typeDescription, CtorSelectionBehavior.Max);

            // assert
            var description = TestHelper.AssertAndUnwrapSuccess(result);
            description.Should().Be(publicMethodDescription);
        }

        #endregion
    }
}
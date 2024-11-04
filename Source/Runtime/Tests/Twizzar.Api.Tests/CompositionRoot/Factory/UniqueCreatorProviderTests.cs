using System;
using System.Data;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.CompositionRoot.Factory;
using TwizzarInternal.Fixture;

namespace Twizzar.Api.Tests.CompositionRoot.Factory
{
    [Category("TwizzarInternal")]
    public class UniqueCreatorProviderTests
    {
        [Test]
        public void GetUniqueCreator_throws_InvalidConstraintException_when_type_is_not_baseType()
        {
            // arrange
            var sut = new ItemBuilder<UniqueCreatorProvider>()
                .Build();

            // act
            Action action = () => sut.GetUniqueCreator<UniqueCreatorProviderTests>();

            // assert
            action.Should().Throw<InvalidConstraintException>();
        }
    }
}
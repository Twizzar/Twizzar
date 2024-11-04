using FluentAssertions;
using NUnit.Framework;
using Twizzar.Runtime.Core.FixtureItem.Creators.BaseTypeUniqueCreators;

namespace Twizzar.Runtime.Core.Tests.FixtureItem.Creators.BaseTypeUniqueCreators
{
    public class UniqueEnumCreatorTests
    {
        [Test]
        public void Unique_creation_cycles_throw_the_enum_values()
        {
            var sut = new UniqueEnumCreator<Numbers>();

            sut.GetNextValue().Should().Be(Numbers.Zero);
            sut.GetNextValue().Should().Be(Numbers.One);
            sut.GetNextValue().Should().Be(Numbers.Two);
            sut.GetNextValue().Should().Be(Numbers.Zero);
        }

        private enum Numbers
        {
            Zero,
            One,
            Two
        }
    }
}
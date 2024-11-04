using System.Collections.Immutable;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.SharedKernel.Core.Tests.FixtureItem
{
    [TestFixture]
    public class ImmutableDictionaryExtensionMergeTests
    {
        [Test]
        public void Merge_takes_all_key_from_a_and_b_when_no_intersection()
        {
            // arrange
            var a = ImmutableDictionary.Create<int, string>()
                .Add(1, RandomString())
                .Add(2, RandomString())
                .Add(3, RandomString());

            var b = ImmutableDictionary.Create<int, string>()
                .Add(4, RandomString())
                .Add(5, RandomString())
                .Add(6, RandomString());

            // act
            var result = a.Merge(b);

            // assert
            result.Should().HaveCount(6);
            result.Keys.Should().Contain(new [] {1, 2, 3, 4, 5, 6});
        }

        [Test]
        public void Merge_takes_values_from_be_on_intersecting_keys()
        {
            // arrange
            var v1 = RandomString();
            var v2 = RandomString();

            var a = ImmutableDictionary.Create<int, string>()
                .Add(1, RandomString())
                .Add(2, RandomString())
                .Add(3, RandomString());

            var b = ImmutableDictionary.Create<int, string>()
                .Add(2, v1)
                .Add(3, v2)
                .Add(4, RandomString());

            // act
            var result = a.Merge(b);

            // assert
            result.Should().HaveCount(4);
            result.Keys.Should().Contain(new[] { 1, 2, 3, 4});
            result[2].Should().Be(v1);
            result[3].Should().Be(v2);
        }

        [Test]
        public void When_b_empty_returns_a()
        {
            // arrange
            var a = ImmutableDictionary.Create<int, string>()
                .Add(1, RandomString())
                .Add(2, RandomString())
                .Add(3, RandomString());

            // act
            var b = ImmutableDictionary.Create<int, string>();

            // assert
            var result = a.Merge(b);
            result.Should().HaveCount(3);
            result.Keys.Should().Contain(a.Keys);
        }

        [Test]
        public void When_a_empty_returns_b()
        {
            // arrange
            var a = ImmutableDictionary.Create<int, string>();

            // act
            var b = ImmutableDictionary.Create<int, string>()
                .Add(1, RandomString())
                .Add(2, RandomString())
                .Add(3, RandomString());
            // assert
            var result = a.Merge(b);
            result.Should().HaveCount(3);
            result.Keys.Should().Contain(b.Keys);
        }

        [Test]
        public void When_both_empty_returns_empty_dict()
        {
            // arrange
            var a = ImmutableDictionary.Create<int, string>();

            // act
            var b = ImmutableDictionary.Create<int, string>();
            // assert
            var result = a.Merge(b);
            result.Should().HaveCount(0);
        }
    }
}

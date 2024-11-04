using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.TestCommon;

namespace Twizzar.SharedKernel.Core.Tests.MoveToViCommon
{
    [TestFixture]
    public class StructuralExtensionsTests
    {
        [Test]
        public void Two_immutableArrays_with_same_values_are_structural_equal()
        {
            // arrange
            var a = ImmutableArray.Create<int>()
                .Add(5)
                .Add(10)
                .Add(15)
                .Add(-5);

            var b= ImmutableArray.Create<int>()
                .Add(5)
                .Add(10)
                .Add(15)
                .Add(-5);

            // act
            var result = a.StructuralEquals(b);

            // assert
            result.Should().BeTrue();
        }

        [Test]
        public void Two_immutableArrays_with_same_values_but_different_oder_are_not_structural_equal()
        {
            // arrange
            var a = ImmutableArray.Create<int>()
                .Add(5)
                .Add(10)
                .Add(15)
                .Add(-5);

            var b = ImmutableArray.Create<int>()
                .Add(15)
                .Add(5)
                .Add(10)
                .Add(-5);

            // act
            var result = a.StructuralEquals(b);

            // assert
            result.Should().BeFalse();
        }

        [Test]
        public void Two_immutableArrays_with_different_values_are_not_structural_equal()
        {
            // arrange
            var a = ImmutableArray.Create<int>()
                .Add(-8)
                .Add(10)
                .Add(15)
                .Add(52)
                .Add(-5);

            var b = ImmutableArray.Create<int>()
                .Add(15)
                .Add(5)
                .Add(10)
                .Add(-5);

            // act
            var result = a.StructuralEquals(b);

            // assert
            result.Should().BeFalse();
        }

        [Test]
        public void Two_immutableDicts_with_same_content_are_structural_equal()
        {
            // arrange
            var values = Enumerable.Range(0, 50)
                .Select(i => new KeyValuePair<string, int>(TestHelper.RandomString(), TestHelper.RandomInt()))
                .ToList();

            var a = ImmutableDictionary.Create<string, int>()
                .AddRange(values);

            values.Reverse();

            var b = ImmutableDictionary.Create<string, int>()
                .AddRange(values);

            // act
            var result = a.StructuralEquals(b);

            // assert
            result.Should().BeTrue();
        }

        [Test]
        public void Two_immutableDicts_with_different_content_are_not_structural_equal()
        {
            // arrange
            var valuesA = Enumerable.Range(0, 50)
                .Select(i => new KeyValuePair<string, int>(TestHelper.RandomString(), TestHelper.RandomInt()))
                .ToList();

            var valuesB = Enumerable.Range(0, 30)
                .Select(i => new KeyValuePair<string, int>(TestHelper.RandomString(), TestHelper.RandomInt()))
                .ToList();

            var a = ImmutableDictionary.Create<string, int>()
                .AddRange(valuesA);

            var b = ImmutableDictionary.Create<string, int>()
                .AddRange(valuesB);

            // act
            var result = a.StructuralEquals(b);

            // assert
            result.Should().BeFalse();
        }
    }
}
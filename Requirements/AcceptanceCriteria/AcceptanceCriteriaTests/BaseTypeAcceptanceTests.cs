using System;
using System.Collections.Generic;

using DemoCode.Car;

using FluentAssertions;

using NUnit.Framework;
using Twizzar.Fixture;

namespace AcceptanceCriteriaTests
{
    [TestFixture]
    [Category("AcceptanceTest")]
    public partial class BaseTypeAcceptanceTests
    {
        [Test]
        [Category("PBI 710")]
        public void All_numeric_base_types_return_a_value_on_requested()
        {
            // Act
            Action action = () =>
            {
                new ItemBuilder<sbyte>().Build();
                new ItemBuilder<byte>().Build();
                new ItemBuilder<short>().Build();
                new ItemBuilder<ushort>().Build();
                new ItemBuilder<int>().Build();
                new ItemBuilder<uint>().Build();
                new ItemBuilder<long>().Build();
                new ItemBuilder<ulong>().Build();

                new ItemBuilder<decimal>().Build();
                new ItemBuilder<double>().Build();
                new ItemBuilder<float>().Build();
            };

            // Assert
            action.Should().NotThrow();
        }

        [Test]
        [Category("PBI 710")]
        public void All_nullable_numeric_base_types_have_a_value_when_requested()
        {
            // Act
            var sbyteNull = new ItemBuilder<sbyte?>().Build();
            var byteNull = new ItemBuilder<byte?>().Build();
            var shortNull = new ItemBuilder<short?>().Build();
            var ushortNull = new ItemBuilder<ushort?>().Build();
            var intNull = new ItemBuilder<int?>().Build();
            var unitNull = new ItemBuilder<uint?>().Build();
            var longNull = new ItemBuilder<long?>().Build();
            var ulongNull = new ItemBuilder<ulong?>().Build();

            var decimalNull = new ItemBuilder<decimal?>().Build();
            var doubleNull = new ItemBuilder<double?>().Build();
            var floatNull = new ItemBuilder<float?>().Build();

            // Assert
            sbyteNull.Should().HaveValue();
            byteNull.Should().HaveValue();
            shortNull.Should().HaveValue();
            ushortNull.Should().HaveValue();
            intNull.Should().HaveValue();
            unitNull.Should().HaveValue();
            longNull.Should().HaveValue();
            ulongNull.Should().HaveValue();

            decimalNull.Should().HaveValue();
            floatNull.Should().HaveValue();
            doubleNull.Should().HaveValue();
        }


        [Test]
        [Category("PBI 712")]
        public void Char_base_type_return_a_value_on_requested()
        {
            // Act
            Action action = () => new ItemBuilder<char>().Build();

            // Assert
            action.Should().NotThrow();
        }

        [Test]
        [Category("PBI 712")]
        public void Nullable_char_base_type_has_a_value_requested()
        {
            // Act
            var value = new ItemBuilder<char?>().Build();

            // Assert
            value.HasValue.Should().BeTrue();
        }

        [Test]
        [Category("PBI 715")]
        public void String_is_not_null_and_a_valid_guid_on_requested()
        {
            // Act
            var value = new ItemBuilder<string>().Build();

            // Assert
            bool IsValidGuid(string s) => 
                Guid.TryParse(s, out _);

            value.Should().NotBeNullOrEmpty();
            IsValidGuid(value).Should().BeTrue();
        }

        [Test]
        [Category("PBI 714")]
        public void Bool_based_types_are_true_on_requested()
        {
            // Act
            var value = new ItemBuilder<bool>().Build();
            var valueNullable = new ItemBuilder<bool?>().Build();

            // Assert
            value.Should().BeTrue();
            valueNullable.Should().HaveValue()
                .And.BeTrue();
        }

        [Test]
        [Category("PBI 1503")]
        public void string_base_type_with_escape_sequence()
        {
            // Act
            var stringContainer = new ItemBuilder<StringContainer>()
                .With(p => p.Value_.Value("TheseAreTheSupportedCharacters\"\'\\\n\r\t"))
                .Build();
            
            // assert
            stringContainer.Value.Should().Be("TheseAreTheSupportedCharacters\"\'\\\n\r\t");
        }

        [Test]
        [Category("PBI 711")]
        public void Bytes_are_not_duplicates_till_more_than_byte_max_value_are_requested_of_the_container()
        {
            // Arrange
            var values = new List<byte>();

            // Act

            for (var i = 0; i <= byte.MaxValue - 1; i++)
            {
                var car = new ItemBuilder<Car>().Build();
                values.Add(car.WheelCount);
            }

            // Assert
            values.Should().HaveCount(byte.MaxValue);
            values.Should().OnlyHaveUniqueItems();
        }

        [Test]
        [Category("PBI 713")]
        [Ignore("This test takes to long.")]
        public void Requesting_many_chars_returns_unique_chars()
        {
            // Arrange
            const int count = char.MaxValue / 20; // 3'276

            // Act
            IEnumerable<char> GetChars()
            {
                for (var i = 0; i < count; i++)
                {
                    yield return new ItemBuilder<char>().Build();
                }
            }

            // Assert
            GetChars().Should().HaveCount(count)
                .And.OnlyHaveUniqueItems();
        }
    }

    internal class StringContainer
    {
        public string Value { get; set; }
    }
}

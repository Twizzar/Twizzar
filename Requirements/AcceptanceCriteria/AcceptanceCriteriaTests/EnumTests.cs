using System;

using FluentAssertions;

using NUnit.Framework;
using Twizzar.Fixture;

namespace AcceptanceCriteriaTests
{
    [TestFixture]
    [Category("AcceptanceTest")]
    public partial class EnumTests
    {
        [Test]
        [Category("PBI 1214")]
        public void Requesting_enum_returns_not_null()
        {
            // act
            var instance = new ItemBuilder<Numbers>().Build();

            // assert
            instance.Should().NotBeNull();
        }

        [Category("PBI 1214")]
        [TestCase(typeof(Numbers), Numbers.One)]
        [TestCase(typeof(Alphabet), Alphabet.E)]
        public void First_unique_value_is_first_enum_value(Type enumType, object expectedResult)
        {
            // arrange
            static object Build<T>() => new ItemBuilder<T>().Build();
            var method = ((Func<object>)Build<object>).Method.GetGenericMethodDefinition();
            var genericMethod = method.MakeGenericMethod(enumType);

            // act
            var instance = genericMethod.Invoke(null, new object[0]);

            // assert
            instance.Should().Be(expectedResult);
        }

        [Test]
        [Category("PBI 1214")]
        public void GetInstances_of_enum_length_returns_all_enum_values()
        {
            var expectedValues = Enum.GetValues(typeof(Alphabet));

            // act
            var instances = new ItemBuilder<Alphabet>()
                .BuildMany(expectedValues.Length);

            // assert
            instances.Should().BeEquivalentTo(expectedValues);
        }

        [Test]
        [TestCase(Numbers.One)]
        [TestCase(Numbers.Three)]
        [TestCase(Numbers.Two)]
        public void Value_configured_in_yaml_returns_correct_instance(Numbers expected)
        {
            // act
            var instance = new ItemBuilder<EnumContainer>()
                .With(p => p.Value_.Value(expected))
                .Build().Value;

            // assert
            instance.Should().Be(expected);
        }
    }

    #region Test Types

    public class EnumContainer
    {
        public Numbers Value { get; set; }
    }

    public enum Numbers
    {
        One,
        Two,
        Three,
    }

    public enum Alphabet
    {
        A = 55,
        B = 54,
        C = 53,
        D = 52,
        E = 51,
        F = 66,
        G,
        H,
        I = -10,
    }

    #endregion
}
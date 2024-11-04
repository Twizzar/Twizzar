using FluentAssertions;

using NUnit.Framework;
using Twizzar.Fixture;

namespace AcceptanceCriteriaTests
{
    [TestFixture]
    public class StringTests
    {

        [TestCase("back\\slash")]
        [TestCase("single\'quote")]
        [TestCase("double\"quote")]
        [TestCase("new\nline")]
        [TestCase("carriage\rreturn")]
        [TestCase("vertical\ttab")]
        [TestCase("  C r\\ \'\"4z< >\n\t\r \\ \" \\ ing ")]
        [TestCase("WhatIs\nexpected\there\r\'\"")]
        public void string_with_escape_characters(string expectedValue)
        {
            var value = new ItemBuilder<StringContainer>()
                .With(p => p.Value_.Value(expectedValue))
                .Build()
                .Value;

            //var value = Build.New<StringContainer>(
            //    new StringContainerConfig().Property(StringContainerConfig.Value).Value(expectedValue)).Value;

            value.Should().Be(expectedValue);
        }

        [TestCase("back\\slash")]
        [TestCase("single\'quote")]
        [TestCase("double\"quote")]
        [TestCase("new\nline")]
        [TestCase("carriage\rreturn")]
        [TestCase("vertical\ttab")]
        [TestCase("  C r\\ \'\"4z< >\n\t\r \\ \" \\ ing ")]
        [TestCase("WhatIs\nexpected\there\r\'\"")]
        public void String_with_escape_sequence_resolved_correctly_for_method(string expectedValue)
        {
            var sut = new ItemBuilder<IStringTest>()
                .With(p => p.StringProp.Value(expectedValue))
                .With(p => p.StringMethod.Value(expectedValue))
                .Build();

            sut.StringProp.Should().Be(expectedValue);
            sut.StringMethod().Should().Be(expectedValue);
        }
    }


    public interface IStringTest
    {
        string StringProp { get; set; }

        string StringMethod();
    }
}

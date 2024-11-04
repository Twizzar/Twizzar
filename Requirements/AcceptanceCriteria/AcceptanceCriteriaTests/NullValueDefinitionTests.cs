using System;
using System.Collections.Generic;
using FluentAssertions;

using NUnit.Framework;
using Twizzar.Fixture;

namespace AcceptanceCriteriaTests
{
    [TestFixture]
    [Category("AcceptanceTest")]
    public partial class NullValueDefinitionTests
    {
        [Test]
        public void ConcreteComponentComponent_sets_property_and_field_to_null_when_configured()
        {
            // act
            var instance = new ItemBuilder<TestNullClass>()
                .With(p => p.Str.Value(null))
                .With(p => p._readonlyStr.Value(null))
                .With(p => p.List.Value(null))
                .With(p => p.NullableInt.Value(null))
                .Build();

            // assert
            instance.Str.Should().BeNull();
            instance.List.Should().BeNull();
            instance.ReadonlyStr.Should().BeNull();
            instance.NullableInt.Should().BeNull();
        }

        [Test]
        public void Mock_sets_property_and_field_to_null_when_configured()
        {
            // act
            var instance = new ItemBuilder<ITestNullClass>().Build();

            // assert
            instance.Str.Should().BeNull();
            instance.List.Should().BeNull();
            instance.NullableInt.Should().BeNull();
        }

        [Test]
        public void BaseType_is_set_to_null_when_configured()
        {
            // act
            var instance = new ItemBuilder<StringContainer>()
                .With(p => p.Value_.Value(null))
                .Build()
                .Value;

            // assert
            instance.Should().BeNull();
        }
    }

    internal class TestNullClass : ITestNullClass
    {
        private readonly string _readonlyStr;

        public TestNullClass()
        {
            this.Str = Guid.NewGuid().ToString();
            this.List = new List<int>();
            this._readonlyStr = Guid.NewGuid().ToString();
            this.NullableInt = new Random().Next();
        }

        public string Str { get; set; }

        public int Int { get; set; }

        public int? NullableInt { get; set; }

        public List<int> List { get; set; }

        public string ReadonlyStr => this._readonlyStr;
    }

    public interface ITestNullClass
    {
        string Str { get; set; }

        int Int { get; set; }

        int? NullableInt { get; set; }

        List<int> List { get; set; }
    }
}

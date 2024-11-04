using DemoCode.ExampleCode;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Fixture;

namespace AcceptanceCriteriaTests
{
    [TestFixture]
    public partial class PropertiesTests
    {
        [Test]
        public void Set_property_with_backing_field_over_code()
        {
            var sut = new ItemBuilder<PropertiesTest>()
                .With(p => p.PropertyWithBackingField.Value(5))
                .Build();

            sut.PropertyWithBackingField.Should().Be(5);
        }

        [Test]
        public void Set_property_with_backing_field_over_ui()
        {
            var sut = new PropertiesTest320bBuilder()
                .Build();

            sut.PropertyWithBackingField.Should().Be(5);
        }
    }
}

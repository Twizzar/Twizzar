using DemoCode;
using DemoCode.Interfaces;

using FluentAssertions;

using NUnit.Framework;
using Twizzar.Fixture;

namespace AcceptanceCriteriaTests
{
    [TestFixture]
    [Category("AcceptanceTest")]
    public partial class ContainerWithBuilderDependencyTests
    {
        [Test]
        [Category("PBI 680")]
        public void Container_AcceptanceCriteria680_PropertyValueOfInterface()
        {

            // act
            var logger = new ItemBuilder<ILogger>()
                .With(p => p.LoggerName.Value("BestLogger"))
                .With(p => p.ValidationService.InstanceOf<ValidationService>())
                .Build();

            // assert
            logger.LoggerName.Should().Be("BestLogger");
            logger.ValidationService.Should().BeAssignableTo(typeof(ValidationService));
        }

        [Test]
        [Category("PBI 842")]
        public void ConstructorParameters_are_set_according_to_yaml_file()
        {
            // act
            var testClass = new ItemBuilder<ConstructorTest>()
                .With(p => p.Ctor.intValue.Value(42))
                .With(p => p.Ctor.stringValue.Value("testValue"))
                .With(p => p.Ctor.nullable.Value(5))
                .Build();

            // assert
            testClass.IntValue.Should().Be(42);
            testClass.StringValue.Should().Be("testValue");
            testClass.NullableValue.Should().Be(5);
        }
    }

    public class ConstructorTest
    {
        public int IntValue { get; set; }
        public string StringValue { get; set; }
        public int? NullableValue { get; set; }

        public ConstructorTest(int intValue, string stringValue)
        {
            this.IntValue = intValue;
            this.StringValue = stringValue;
        }

        public ConstructorTest(int intValue, string stringValue, int? nullable)
        {
            this.IntValue = intValue;
            this.StringValue = stringValue;
            this.NullableValue = nullable;
        }
    }

    public class ClassA
    {

    }
}

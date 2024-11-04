using DemoCode.ExampleCode;

using FluentAssertions;

using NUnit.Framework;
using Twizzar.Fixture;

namespace AcceptanceCriteriaTests
{
    [TestFixture]
    public partial class MethodSetupTests
    {
        [Test]
        public void Simple_methodSetup_returns_expected_value_for_int()
        {
            // arrange
            var methodTest = new ItemBuilder<IMethodTest>()
                .With(p => p.MethodWithOverloads__Int32.Value(42))
                .Build();

            // act
            var result = methodTest.MethodWithOverloads(2);

            // assert
            result.Should().Be(42);
        }

        [Test]
        public void Simple_methodSetup_returns_expected_value_for_string()
        {
            // arrange
            var methodTest = new SimpleStringMethodBuilder()
                .Build();

            // act
            var result = methodTest.MethodWithOverloads("");

            // assert
            result.Should().Be("Einen Test");
        }

        [Test]
        public void Method_setup_returns_same_value_for_every_parameter_value()
        {
            // arrange
            var methodTest = new SimpleStringMethodBuilder().Build();

            for (var i = 0; i < 100; i++)
            {
                var result = methodTest.MethodWithOverloads(new ItemBuilder<string>().Build());
                result.Should().Be("Einen Test");
            }
        }

        [Test]
        public void Method_return_type_enum_works_correctly()
        {
            // arrange & act
            var methodTest = new ItemBuilder<IMethodTest>()
                .With(p => p.MethodEnum.Value(MyEnum.Two))
                .Build();

            // assert
            methodTest.MethodEnum().Should().Be(MyEnum.Two);
        }

        [Test]
        public void Method_return_type_complex_works_correctly()
        {
            // arrange
            var expected = new FastAfCarBuilder().Build();
            var methodTest = new ItemBuilder<IMethodTest>()
                .With(p => p.GetMyVehicle.Value(expected))
                .Build();

            // act
            var result = methodTest.GetMyVehicle(string.Empty);

            // assert
            result.Should().Be(expected);
        }

        [Test]
        public void Method_setup_with_null_returns_null()
        {
            // arrange
            var methodTest = new ItemBuilder<IMethodTest>()
                .With(p => p.GetMyVehicle.Value(null))
                .Build();

            // act
            var result = methodTest.GetMyVehicle(string.Empty);

            // assert
            result.Should().Be(null);
        }

        [Test]
        public void GenericClass_method_with_generic_return_type_can_be_configured()
        {
            // arrange
            var methodTest = new ItemBuilder<IGenericClassMethodTest<int>>()
                .With(p => p.GenericMethod.Value(5))
                .Build();

            // act
            var result = methodTest.GenericMethod();

            // assert
            result.Should().Be(5);
        }

        [Test]
        public void Method_return_mock_is_setup_correctly()
        {
            // arrange
            var methodTest = new ItemBuilder<IMethodTest>()
                .With(p => p.GetMyVehicle__String.Speed.Value(3))
                .Build();

            var myVehicle = methodTest.GetMyVehicle("");

            // assert
            myVehicle.Speed.Should().Be(3);
        }
    }
}
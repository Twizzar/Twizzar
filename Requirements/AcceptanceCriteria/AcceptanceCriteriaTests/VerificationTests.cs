using DemoCode.Car;
using DemoCode.ExampleCode;
using NUnit.Framework;
using Twizzar.Fixture;

namespace AcceptanceCriteriaTests
{
    [TestFixture]
    public partial class VerificationTests
    {
        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(0, 1)]
        [TestCase(1, 0)]
        public void Void_method_is_verifiedCorrectly(int timesCalled, int timeVerified)
        {
            // arrange
            var sut = new IMethodTestBuilder()
                .Build(out var scope);

            // act
            for (int i = 0; i < timesCalled; i++)
            {
                sut.VoidMethod();
            }

            // assert
            void action() =>
                scope.Verify(p => p.VoidMethod).Called(timeVerified);

            if (timesCalled == timeVerified)
            {
                Assert.DoesNotThrow(action);
            }
            else
            {
                Assert.Throws<VerificationException>(action);
            }
        }

        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(0, 1)]
        [TestCase(1, 0)]
        public void Method_with_overloads_is_verifiedCorrectly(int timesCalled, int timeVerified)
        {
            // arrange
            var sut = new IMethodTestBuilder()
                .Build(out var scope);

            // act
            for (int i = 0; i < timesCalled; i++)
            {
                sut.MethodWithOverloads(5);
            }

            // assert
            void action() =>
                scope.Verify(p => p.MethodWithOverloads__Int32).Called(timeVerified);

            if (timesCalled == timeVerified)
            {
                Assert.DoesNotThrow(action);
            }
            else
            {
                Assert.Throws<VerificationException>(action);
            }
        }

        [TestCase("test", "test")]
        [TestCase("", "")]
        [TestCase("", "test")]
        [TestCase("test", "")]
        [TestCase("test", "test2")]
        public void Where_param_is_validated_correctly(string methodParam, string verifyParam)
        {
            // arrange
            var sut = new IMethodTestBuilder()
                .Build(out var scope);

            sut.MethodWithOverloads(methodParam);

            void action() =>
                scope.Verify(p => p.MethodWithOverloads__String)
                    .WhereAIs(verifyParam)
                    .CalledAtLeastOnce();

            if (methodParam == verifyParam)
            {
                Assert.DoesNotThrow(action);
            }
            else
            {
                Assert.Throws<VerificationException>(action);
            }
        }

        [TestCase("test", "test")]
        [TestCase("", "")]
        [TestCase("", "test")]
        [TestCase("test", "")]
        [TestCase("test", "test2")]
        public void Method_verification_also_works_with_custom_builder(string methodParam, string verifyParam)
        {
            // arrange
            var sut = new IMethodTest85adBuilder()
                .Build(out var scope);

            sut.MethodWithOverloads(methodParam);

            void action() =>
                scope.Verify(p => p.MethodWithOverloads__String)
                    .WhereAIs(verifyParam)
                    .CalledAtLeastOnce();

            if (methodParam == verifyParam)
            {
                Assert.DoesNotThrow(action);
            }
            else
            {
                Assert.Throws<VerificationException>(action);
            }
        }

        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(0, 1)]
        [TestCase(1, 0)]
        public void Verify_property_getter_is_validated_correctly(int timesCalled, int timeVerified)
        {
            // arrange
            var sut = new IPassengerBuilder()
                .Build(out var scope);

            // act
            for (int i = 0; i < timesCalled; i++)
            {
                var _ = sut.Name;
            }

            // assert
            void action() => 
                scope.Verify(p => p.Name)
                    .Get().Called(timeVerified);

            if (timesCalled == timeVerified)
            {
                Assert.DoesNotThrow(action);
            }
            else
            {
                Assert.Throws<VerificationException>(action);
            }
        }

        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(0, 1)]
        [TestCase(1, 0)]
        public void Verify_property_setter_with_setValue_is_validated_correctly(int timesCalled, int timeVerified)
        {
            // arrange
            var sut = new IPassengerBuilder()
                .Build(out var scope);

            // act
            for (int i = 0; i < timesCalled; i++)
            {
                sut.Name = "test";
            }

            // assert
            void action() =>
                scope.Verify(p => p.Name)
                    .Set().Called(timeVerified);

            if (timesCalled == timeVerified)
            {
                Assert.DoesNotThrow(action);
            }
            else
            {
                Assert.Throws<VerificationException>(action);
            }
        }

        [TestCase("test", "test")]
        [TestCase("", "")]
        [TestCase("", "test")]
        [TestCase("test", "")]
        [TestCase("test", "test2")]
        public void Verify_property_setter_is_validated_correctly(string setValue, string validatedValue)
        {
            // arrange
            var sut = new IPassengerBuilder()
                .Build(out var scope);

            // act
            sut.Name = setValue;
            
            // assert
            void action() =>
                scope.Verify(p => p.Name)
                    .Set(validatedValue).CalledAtLeastOnce();

            if (setValue == validatedValue)
            {
                Assert.DoesNotThrow(action);
            }
            else
            {
                Assert.Throws<VerificationException>(action);
            }
        }
    }
}

using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Common.Util.Routine;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.Tests.Common.Util.Routine
{
    [TestFixture]
    public class RoutineContextTests
    {
        private RoutineContext _sut;

        [SetUp]
        public void SetUp()
        {
            this._sut = Build.New<RoutineContext>();
        }

        [Test]
        public void Ctor_parameters_throw_ArgumentNullException_when_null()
        {
            // assert
            Verify.Ctor<RoutineContext>()
                .ShouldThrowArgumentNullException();
        }

        [Test]
        public void Set_stores_primitive_value()
        {
            // arrange
            var (key, expectedValue) = Build.New<(string, int)>();
            
            // act
            this._sut.Set(key, expectedValue);

            // assert
            this._sut.Get<int>(key).Should().Be(Maybe.Some(expectedValue));
        }

        [Test]
        public void Set_store_complex_value()
        {
            // arrange
            var (key, expectedValue) = Build.New<(string, RoutineRunner)>();
            
            // act
            this._sut.Set(key, expectedValue);

            // assert
            this._sut.Get<RoutineRunner>(key).Should().Be(Maybe.Some(expectedValue));
        }

        [Test]
        public void Set_overrides_value()
        {
            // arrange
            var (key, initialValue, newValue) = Build.New<(string, int, int)>();
            this._sut.Set(key, initialValue);
            this._sut.Get<int>(key).Should().Be(Maybe.Some(initialValue));

            // act
            this._sut.Set(key, newValue);

            // assert
            this._sut.Get<int>(key).Should().Be(Maybe.Some(newValue));
        }

        [Test]
        public void Get_returns_none_when_not_set()
        {
            // arrange
            var key = Build.New<string>();

            // act
            var result = this._sut.Get<string>(key);

            // assert
            result.IsNone.Should().Be(true);
        }
    }
}
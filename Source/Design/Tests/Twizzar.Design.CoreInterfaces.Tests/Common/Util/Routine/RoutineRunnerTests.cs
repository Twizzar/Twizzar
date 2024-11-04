using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Common.Util.Routine;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.CoreInterfaces.Tests.Common.Util.Routine
{
    [TestFixture]
    public class RoutineRunnerTests
    {
        private RoutineRunner _sut;

        [SetUp]
        public void SetUp()
        {
            this._sut = Build.New<RoutineRunner>();
        }

        [Test]
        public void Ctor_parameters_throw_ArgumentNullException_when_null()
        {
            // assert
            Verify.Ctor<RoutineRunner>()
                .ShouldThrowArgumentNullException();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void When_CancelWhen_is_true_cancel_the_routine(bool cancel)
        {
            // arrange
            var isCalled = false;

            void Action()
            {
                isCalled = true;
            }

            // act
            this._sut.Run(TestCancelWhenInstruction(cancel, Action));

            // assert
            isCalled.Should().Be(!cancel);
        }

        [Test]
        public async Task PreventSpam_is_called_when_delta_is_awaited()
        {
            // arrange
            var isCalled = false;

            void Action()
            {
                isCalled = true;
            }

            // act
            this._sut.Run(TestPreventSpamInstruction(TimeSpan.FromMilliseconds(10), () => { }));
            await Task.Delay(11);
            this._sut.Run(TestPreventSpamInstruction(TimeSpan.FromMilliseconds(10), Action));

            // assert
            isCalled.Should().Be(true);
        }

        [Test]
        public void PreventSpam_is_not_called_when_delta_is_not_awaited()
        {
            // arrange
            var isCalled = false;

            void Action()
            {
                isCalled = true;
            }

            // act
            this._sut.Run(TestPreventSpamInstruction(TimeSpan.FromMilliseconds(20), () => { }));
            this._sut.Run(TestPreventSpamInstruction(TimeSpan.FromMilliseconds(20), Action));

            // assert
            isCalled.Should().Be(false);
        }

        private static IEnumerable<ICancelInstruction> TestCancelWhenInstruction(bool cancel, Action afterCancelAction)
        {
            yield return new CancelWhen(() => cancel);
            afterCancelAction();
        }

        private static IEnumerable<ICancelInstruction> TestPreventSpamInstruction(TimeSpan delta, Action afterCancelAction)
        {
            yield return new PreventSpam(delta);
            afterCancelAction();
        }
    }
}
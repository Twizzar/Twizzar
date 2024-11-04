using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.Fixture;
using Twizzar.Fixture.Verifier;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Services;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.Api.Tests.Utils.Verifier
{
    [Category("TwizzarInternal")]
    public class CtorVerifierTests
    {
        private IFixtureItemContainer _fixtureItemContainer;

        [SetUp]
        public void SetUp()
        {
            this._fixtureItemContainer = new ItemBuilder<IFixtureItemContainer>().Build();
        }

        [Test]
        public void Ctor()
        {
            // act
            Action action = () => new CtorVerifier<VerifyTestClass>(this._fixtureItemContainer)
                    .ShouldThrowArgumentNullException();

            // assert
            action.Should().Throw<Exception>();

            Mock.Get(this._fixtureItemContainer)
                .Verify(container => container.GetInstance<string>(), Times.AtLeastOnce);
        }

        [Test]
        public void IgnoreThisParameter_ignore_will_ignore_the_parameter()
        {
            // arrange
            var sut = new CtorVerifier<VerifyTestClass>(this._fixtureItemContainer);

            // act
            Action action = () =>
                sut
                    .IgnoreParameter("p1")
                    .IgnoreParameter("p2")
                    .IgnoreParameter("p3")
                    .IgnoreParameter("p4")
                    .ShouldThrowArgumentNullException();

            // assert
            action.Should().Throw<Exception>();

            var fixtureItemContainerMock = Mock.Get(this._fixtureItemContainer);

            fixtureItemContainerMock.Verify(container => container.GetInstance<byte>(), Times.Never);
            fixtureItemContainerMock.Verify(container => container.GetInstance<char>(), Times.Never);
            fixtureItemContainerMock.Verify(container => container.GetInstance<bool>(), Times.Never);
            fixtureItemContainerMock.Verify(container => container.GetInstance<string>(), Times.Never);
            fixtureItemContainerMock.Verify(container => container.GetInstance<IVerifyTestInterface>(), Times.Once);
        }

        [Test]
        public void AddInstance_uses_the_provided_instance_for_the_parameter()
        {
            // arrange
            var sut = new CtorVerifier<CtorVerifierTestsSpy>(this._fixtureItemContainer);
            var VerifyTestClass = new ItemBuilder<VerifyTestClass>().Build();
            CtorVerifierTestsSpy.Reset();

            // act
            sut
                .SetupParameter("a", 5)
                .SetupParameter("b", "test")
                .SetupParameter("c", VerifyTestClass)
                .ShouldThrowArgumentNullException();

            // assert
            var fixtureItemContainerMock = Mock.Get(this._fixtureItemContainer);

            fixtureItemContainerMock.Verify(container => container.GetInstance<byte>(), Times.Never);
            fixtureItemContainerMock.Verify(container => container.GetInstance<string>(), Times.Never);
            fixtureItemContainerMock.Verify(container => container.GetInstance<VerifyTestClass>(), Times.Never);

            CtorVerifierTestsSpy.UsedCtorCalls.Count.Should().Be(3);
            CtorVerifierTestsSpy.UsedCtorCalls.Should().Contain((5, null, VerifyTestClass));
            CtorVerifierTestsSpy.UsedCtorCalls.Should().Contain((5, "test", null));
            CtorVerifierTestsSpy.UsedCtorCalls.Should().Contain((5, "test", VerifyTestClass));
        }


        [Test]
        public void IgnoreThisParameter_with_default_value_uses_this_default_value_in_parameter()
        {
            // arrange
            var fixtureItemContainer = new Mock<IFixtureItemContainer>();
            fixtureItemContainer.Setup(container => container.GetInstance<string>())
                .Returns("test");

            var sut = new CtorVerifier<CtorVerifierTestsSpy>(fixtureItemContainer.Object);
            var VerifyTestClass = new ItemBuilder<VerifyTestClass>().Build();
            CtorVerifierTestsSpy.Reset();

            // act
            sut
                .IgnoreParameter("c", VerifyTestClass)
                .ShouldThrowArgumentNullException();

            // assert
            fixtureItemContainer.Verify(container => container.GetInstance<byte>(), Times.Never);
            fixtureItemContainer.Verify(container => container.GetInstance<string>(), Times.Once);
            fixtureItemContainer.Verify(container => container.GetInstance<VerifyTestClass>(), Times.Never);

            CtorVerifierTestsSpy.UsedCtorCalls.Count.Should().Be(2);
            CtorVerifierTestsSpy.UsedCtorCalls.Should().Contain((0, null, VerifyTestClass));
            CtorVerifierTestsSpy.UsedCtorCalls.Should().Contain((0, "test", VerifyTestClass));
        }

        #region Test Types

        public class VerifyTestClass
        {
            public VerifyTestClass(byte p1, char p2, bool p3, string p4, IVerifyTestInterface p5)
            {
                if (p4 == null)
                {
                    throw new ArgumentNullException(nameof(p4));
                }

                if (p5 == null)
                {
                    throw new ArgumentNullException(nameof(p5));
                }
            }
        }

        public interface IVerifyTestInterface
        {

        }

        private class NoExceptionClass
        {
            public NoExceptionClass(int a, string b, VerifyTestClass c)
            {
            }
        }

        private class OtherExceptionClass
        {
            public OtherExceptionClass(int a, string b, VerifyTestClass c)
            {
                if (b is null)
                {
                    throw new ArgumentException();
                }
            }
        }

        private class WrongMessageException
        {
            public WrongMessageException(int a, string b, VerifyTestClass c)
            {
                if (b is null)
                {
                    throw new ArgumentNullException("c");
                }
            }
        }

        private class InstanceExceptionClass
        {
            public InstanceExceptionClass(int a, string b, VerifyTestClass c)
            {
                if (b is null)
                {
                    throw new ArgumentNullException(nameof(b));
                }

                throw new InvalidOperationException();
            }
        }

        #endregion

        private class CtorVerifierTestsSpy
        {
            private static readonly List<(int a, string b, VerifyTestClass c)> UsedCtors = new List<(int a, string b, VerifyTestClass c)>();

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1118:Utility classes should not have public constructors", Justification = "Used for test spy.")]
            public CtorVerifierTestsSpy(int a, string b, VerifyTestClass c)
            {
                UsedCtors.Add((a, b, c));

                EnsureHelper.GetDefault.Many()
                    .Parameter(b, nameof(b))
                    .Parameter(c, nameof(c))
                    .ThrowWhenNull();
            }

            public static void Reset() => UsedCtors.Clear();

            public static IReadOnlyList<(int a, string b, VerifyTestClass c)> UsedCtorCalls => UsedCtors;
        }
    }
}
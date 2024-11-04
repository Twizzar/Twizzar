using FluentAssertions;

using NUnit.Framework;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.TestCommon;

namespace Twizzar.SharedKernel.Core.Tests.MoveToViCommon
{
    [TestFixture]
    public class DefaultDictionaryTests
    {
        [Test]
        public void Int_keyIndex_returns_default_when_not_exists()
        {
            // arrange
            var (key, value) = (TestHelper.RandomString(), TestHelper.RandomInt());

            var sut = new DefaultDictionary<string, int> { { key, value } };

            // act
            sut[key].Should().Be(value);
            sut[TestHelper.RandomString()].Should().Be(default);
        }

        [Test]
        public void String_keyIndex_returns_default_when_not_exists()
        {
            // arrange
            var (key, value) = (TestHelper.RandomString(), TestHelper.RandomString());

            var sut = new DefaultDictionary<string, string> { { key, value } };

            // act
            sut[key].Should().Be(value);
            sut[TestHelper.RandomString()].Should().Be(default);
        }

        [Test]
        public void Int_with_factory_returns_correct_value()
        {
            // arrange
            var (key, value) = (TestHelper.RandomInt(), TestHelper.RandomInt());

            var sut = new DefaultDictionary<int, int>(i => i + 1) { { key, value } };

            // act
            sut[key].Should().Be(value);
            var rKey = TestHelper.RandomInt();
            sut[rKey].Should().Be(rKey + 1);
        }
    }
}
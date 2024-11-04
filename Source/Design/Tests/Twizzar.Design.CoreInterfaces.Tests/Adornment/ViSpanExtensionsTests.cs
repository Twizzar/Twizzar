using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.TestCommon;

namespace Twizzar.Design.CoreInterfaces.Tests.Adornment
{
    [TestFixture]
    public class ViSpanExtensionsTests
    {
        [Test]
        public void Test_End()
        {
            // arrange
            var span = Build.New<IViSpan>();

            // act
            var end = span.End();

            // arrange
            end.Should().Be(span.Start + span.Length);
        }
    }
}
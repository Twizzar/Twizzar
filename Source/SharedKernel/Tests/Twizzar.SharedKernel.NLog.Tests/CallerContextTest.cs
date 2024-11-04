using System.Runtime.CompilerServices;

using FluentAssertions;

using NUnit.Framework;

using Twizzar.SharedKernel.NLog.Logging;

namespace Twizzar.SharedKernel.NLog.Tests
{
    [TestFixture]
    public class CallerContextTest
    {
        [Test]
        public void CallerContext_create_captures_current_context_correctly()
        {
            // act
            var context = CallerContext.Create();

            // assert
            context.CallerMemberName.Should().Be(nameof(this.CallerContext_create_captures_current_context_correctly));
            context.FilePath.Should().Be(GetFileName());
            context.Line.Should().Be(15);
        }

        [Test]
        public void Format_formats_the_information_correctly()
        {
            // arrange
            var memberName = "TestMember";
            var path = @"C\log.log";
            var line = 5;
            var expectedFormat = $"Method {memberName} in {path} at line {line}";

            var context = CallerContext.Create(memberName, path, line);

            // act 
            var format = context.Format();

            // assert
            format.Should().Be(expectedFormat);
        }

        private static string GetFileName([CallerFilePath] string file = "") => file;

    }
}
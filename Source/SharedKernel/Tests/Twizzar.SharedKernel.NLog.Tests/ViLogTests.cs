using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using Twizzar.SharedKernel.NLog.Logging;

namespace Twizzar.SharedKernel.NLog.Tests
{
    [TestFixture]
    public class ViLogTests
    {
        [Test]
        public void Test_log_T1_msg()
        {
            // arrange
            var targetMock = new LoggerTargetMock();
            LoggerFactory.SetConfig(new LoggerConfigurationBuilder().AddTarget(targetMock));
            var msg = "this is a test";

            // act
            ViLog.Log<ViLogTests>(msg);

            // assert
            targetMock.Logs.Should().HaveCount(1);
            var info = targetMock.Logs.First();

            info.LoggerName.Should().Be(typeof(ViLogTests).FullName);
            info.Message.Should().Be(msg);
        }

        [Test]
        public void Test_log_T1_self_msg()
        {
            // arrange
            var targetMock = new LoggerTargetMock();
            LoggerFactory.SetConfig(new LoggerConfigurationBuilder().AddTarget(targetMock));
            var msg = "this is a test";

            // act
            ViLog.Log(this, msg);

            // assert
            targetMock.Logs.Should().HaveCount(1);
            var info = targetMock.Logs.First();

            info.LoggerName.Should().Be(typeof(ViLogTests).FullName);
            info.Message.Should().Be(msg);
        }

        [Test]
        public void Test_log_T1_callerContext_msg()
        {
            // arrange
            var callerContext = CallerContext.Create();
            var targetMock = new LoggerTargetMock();
            LoggerFactory.SetConfig(new LoggerConfigurationBuilder().AddTarget(targetMock));
            var msg = "this is a test";

            // act
            ViLog.Log(callerContext, msg);

            // assert
            targetMock.Logs.Should().HaveCount(1);
            var info = targetMock.Logs.First();

            info.LoggerName.Should().Be(callerContext.Format());
            info.Message.Should().Be(msg);
        }
    }
}
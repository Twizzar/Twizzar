using System.Collections.Generic;

using FluentAssertions;

using NUnit.Framework;

using Twizzar.SharedKernel.NLog.Logging;

using LogLevel = NLog.LogLevel;

namespace Twizzar.SharedKernel.NLog.Tests
{
    [TestFixture]
    public class LogLevelMapperTests
    {
        [Theory]
        [TestCaseSource(nameof(GetData))]
        public void MapLogLevelTest(Interfaces.LogLevel viLogLevel, LogLevel nLogLogLevel)
        {
            // act
            var logLevel = LogLevelMapper.MapLogLevel(viLogLevel);

            // assert
            logLevel.Should().Be(nLogLogLevel);
        }

        private static IEnumerable<object[]> GetData()
        {
            yield return new object[] { Interfaces.LogLevel.Error, LogLevel.Error, };
            yield return new object[] { Interfaces.LogLevel.Debug, LogLevel.Debug, };
            yield return new object[] { Interfaces.LogLevel.Fatal, LogLevel.Fatal, };
            yield return new object[] { Interfaces.LogLevel.Info, LogLevel.Info, };
            yield return new object[] { Interfaces.LogLevel.Trace, LogLevel.Trace, };
            yield return new object[] { Interfaces.LogLevel.Warn, LogLevel.Warn, };
        }
    }
}
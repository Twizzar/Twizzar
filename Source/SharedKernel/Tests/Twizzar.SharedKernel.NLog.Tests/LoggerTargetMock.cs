using System.Collections.Generic;
using NLog;
using NLog.Targets;

namespace Twizzar.SharedKernel.NLog.Tests
{
    public class LoggerTargetMock : Target
    {
        public IList<LogEventInfo> Logs { get; } = new List<LogEventInfo>();

        #region Overrides of Target

        /// <inheritdoc />
        protected override void Write(LogEventInfo logEvent)
        {
            this.Logs.Add(logEvent);
        }

        #endregion
    }
}
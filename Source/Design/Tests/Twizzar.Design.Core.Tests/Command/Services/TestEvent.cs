using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;

namespace Twizzar.Design.Core.Tests.Command.Services
{
    [ExcludeFromCodeCoverage]
    public class TestEvent : ITestEvent
    {
        /// <inheritdoc />
        public FixtureItemId FixtureItemId { get; set; }

        #region Implementation of IEvent

        /// <inheritdoc />
        string IEvent.ToLogString() => this.ToString();

        #endregion
    }

    [ExcludeFromCodeCoverage]
    public class TestEvent2 : IEvent<TestEvent2>, IFixtureItemEvent
    {
        /// <inheritdoc />
        public FixtureItemId FixtureItemId { get; set; }

        #region Implementation of IEvent

        /// <inheritdoc />
        string IEvent.ToLogString() => this.ToString();

        #endregion
    }

    public interface ITestEvent : IEvent<ITestEvent>, IFixtureItemEvent { }
}

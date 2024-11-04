using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Command.Services;

namespace Twizzar.Design.Core.Tests.Command.Services
{
    [ExcludeFromCodeCoverage]
    public class FakeEvent : IEvent<FakeEvent>
    {
        #region Implementation of IEvent

        /// <inheritdoc />
        string IEvent.ToLogString() => this.ToString();

        #endregion
    }
}
using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;

namespace Twizzar.Design.Core.Tests.Command.Services
{
    [ExcludeFromCodeCoverage]
    public class FixtureItemEvent : IEvent<FixtureItemEvent>, IFixtureItemEvent
    {
        #region properties

        /// <inheritdoc />

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public FixtureItemId FixtureItemId { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        string IEvent.ToLogString() => this.ToString();

        #endregion
    }
}
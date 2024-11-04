using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Command.Services;

namespace Twizzar.Design.CoreInterfaces.Command.Events
{
    /// <summary>
    /// Raised when the event store is cleared for specific project.
    /// </summary>
    /// <seealso cref="FixtureItemConfigurationEndedEvent" />
    [ExcludeFromCodeCoverage]
    public class FixtureItemConfigurationEndedEvent : IEvent<FixtureItemConfigurationEndedEvent>
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemConfigurationEndedEvent"/> class.
        /// </summary>
        /// <param name="rootFixtureItemPath">The event store root fixture item path.</param>
        public FixtureItemConfigurationEndedEvent(string rootFixtureItemPath)
        {
            this.RootFixtureItemPath = rootFixtureItemPath;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the event store project name.
        /// </summary>
        public string RootFixtureItemPath { get; }

        #endregion

        #region members

        /// <inheritdoc />
        string IEvent.ToLogString() =>
            $"{nameof(FixtureItemConfigurationEndedEvent)} {{ RootFixtureItemPath = {this.RootFixtureItemPath.GetHashCode()} }}";

        #endregion
    }
}
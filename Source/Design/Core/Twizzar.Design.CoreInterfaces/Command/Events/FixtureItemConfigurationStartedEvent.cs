using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;

namespace Twizzar.Design.CoreInterfaces.Command.Events
{
    /// <summary>
    /// Represents the event of setting the project name for an adornment.
    /// </summary>
    /// <param name="ProjectName">The ProjectName.</param>
    /// <param name="FixtureItemId">The RootFixtureItemId of the root fixture item.</param>
    /// <param name="DocumentFilePath">the document file path.</param>
    /// <param name="InvocationSpan">The the span of a type invocation.</param>
    [ExcludeFromCodeCoverage]
    public record FixtureItemConfigurationStartedEvent(
        FixtureItemId FixtureItemId,
        string ProjectName,
        string DocumentFilePath,
        IViSpan InvocationSpan) :
        IFixtureItemEvent,
        IEvent<FixtureItemConfigurationStartedEvent>
    {
        #region members

        /// <inheritdoc />
        string IEvent.ToLogString() =>
            $"{nameof(FixtureItemConfigurationStartedEvent)} {{ " +
            $"FixtureItemId = {this.FixtureItemId.GetHashCode()}," +
            $" ProjectName = {this.ProjectName.GetHashCode()}," +
            $" DocumentFilePath = {this.DocumentFilePath.GetHashCode()}," +
            $" InvocationSpan = {this.InvocationSpan} }}";

        #endregion
    }
}
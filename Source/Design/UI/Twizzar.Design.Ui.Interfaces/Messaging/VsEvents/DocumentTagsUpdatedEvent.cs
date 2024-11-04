using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Common.Util;

namespace Twizzar.Design.Ui.Interfaces.Messaging.VsEvents
{
    /// <summary>
    /// Event fired when the tags got updated.
    /// </summary>
    /// <param name="DocumentFilePath">The document name.</param>
    [ExcludeFromCodeCoverage]
    public record DocumentTagsUpdatedEvent(string DocumentFilePath) : IUiEvent;
}
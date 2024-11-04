using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable SA1009 // Closing parenthesis should be spaced correctly

namespace Twizzar.Design.Ui.Interfaces.Messaging.VsEvents
{
    /// <summary>
    /// Event triggered when the focus of a Fixture Item Node is changed not by the User.
    /// </summary>
    /// <param name="NodeId"></param>
    [ExcludeFromCodeCoverage]
    public record FixtureItemNodeFocusedEvent(NodeId NodeId) : IUiEvent
    {
    }
}
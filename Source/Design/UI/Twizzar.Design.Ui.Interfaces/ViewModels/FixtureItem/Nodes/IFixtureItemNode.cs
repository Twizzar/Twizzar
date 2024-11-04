using System.Collections.Generic;
using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.Ui.Interfaces.Controller;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes
{
    /// <summary>
    /// Represents a logical node for a FixtureItem.
    /// This can be the root node (fixture item itself) or a Member of the Fixture Item
    /// like fields, properties, methods, constructor, parameters, etc.
    /// </summary>
    public interface IFixtureItemNode : IHasEnsureHelper, IHasLogger
    {
        #region properties

        /// <summary>
        /// Gets the id of the node.
        /// </summary>
        NodeId Id { get; }

        /// <summary>
        /// Gets the fixture information.
        /// </summary>
        IFixtureItemInformation FixtureItemInformation { get; }

        /// <summary>
        /// Gets the value controller.
        /// </summary>
        IFixtureItemNodeValueController NodeValueController { get; }

        /// <summary>
        /// Gets the parent of the node. None when root.
        /// </summary>
        Maybe<IFixtureItemNode> Parent { get; }

        /// <summary>
        /// Gets the children of this node.
        /// </summary>
        IEnumerable<IFixtureItemNode> Children { get; }

        #endregion

        #region members

        /// <summary>
        /// Commit the member config and send it to the domain logic.
        /// This will not update the configuration of the node.
        /// The only way to update the configuration is with a successful event.
        /// </summary>
        /// <param name="memberConfiguration"></param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task CommitMemberConfig(IMemberConfiguration memberConfiguration);

        /// <summary>
        /// Update the fixture item id of the node.
        /// </summary>
        /// <param name="fixtureItemId"></param>
        void UpdateFixtureItemId(FixtureItemId fixtureItemId);

        /// <summary>
        /// Refresh the view to show the new information.
        /// </summary>
        /// <param name="fixtureItemInformation"></param>
        void RefreshFixtureInformation(IFixtureItemInformation fixtureItemInformation);

        /// <summary>
        /// Display an error when the member change has failed.
        /// </summary>
        /// <param name="e">The failed event.</param>
        void DisplayOnMemberChangedFailed(FixtureItemMemberChangedFailedEvent e);

        #endregion
    }
}
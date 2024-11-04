using System.Threading.Tasks;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes
{
    /// <summary>
    /// Service for sending updates form the view model node to the Domain logic.
    /// This service is stateless.
    /// </summary>
    public interface IFixtureItemNodeSender : IService
    {
        /// <summary>
        /// Update the member configuration.
        /// </summary>
        /// <param name="current">The current node.</param>
        /// <param name="memberConfig">The new member configuration.</param>
        /// <param name="fixtureItemInformation">The current fixture information.</param>
        /// <param name="parent">The parent node.</param>
        /// <returns>A task.</returns>
        Task UpdateMemberConfigAsync(
            IFixtureItemNode current,
            IMemberConfiguration memberConfig,
            IFixtureItemInformation fixtureItemInformation,
            Maybe<IFixtureItemNode> parent);

        /// <summary>
        /// Change the value corresponding to a validated token.
        /// </summary>
        /// <param name="current">The current node.</param>
        /// <param name="token">The validated token.</param>
        /// <param name="fixtureItemInformation">The current fixture information.</param>
        /// <param name="parent">The parent node.</param>
        /// <returns>A task.</returns>
        Task ChangeValue(
            IFixtureItemNode current,
            IViToken token,
            IFixtureItemInformation fixtureItemInformation,
            Maybe<IFixtureItemNode> parent);
    }
}
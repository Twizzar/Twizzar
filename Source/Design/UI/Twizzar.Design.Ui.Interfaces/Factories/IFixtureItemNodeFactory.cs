using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.SharedKernel.CoreInterfaces;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Ui.Interfaces.Factories
{
    /// <summary>
    /// Factory for creating:
    /// <see cref="IFixtureItemNode"/>,
    /// <see cref="IFixtureItemNodeSender"/> and
    /// <see cref="IFixtureItemNodeReceiver"/>.
    /// </summary>
    public interface IFixtureItemNodeFactory : IFactory
    {
        /// <summary>
        /// Factory for creating node children.
        /// </summary>
        /// <param name="parent">The parent of the generated nodes.</param>
        /// <param name="fixtureItemInformation">The fixture information of the paren.</param>
        /// <returns>A new sequence of node view models.</returns>
        public delegate Task<IResult<IEnumerable<IFixtureItemNodeViewModel>, Failure>> CreateChildrenFactory(
            Maybe<IFixtureItemNode> parent,
            IFixtureItemInformation fixtureItemInformation);

        #region members

        /// <summary>
        /// Create a view model node.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="receiverFactory">The receiverFactory.</param>
        /// <param name="createChildrenQuery">Function for querying the children of the node.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="fixtureItemInformation">The fixture information.</param>
        /// <param name="fixtureItemNodeValueViewModel">The value view model.</param>
        /// <param name="parent">The parent node or none.</param>
        /// <returns>A new instance of <see cref="IFixtureItemNodeViewModel"/>.</returns>
        IFixtureItemNodeViewModel CreateViewModelNode(
            NodeId id,
            Func<IFixtureItemNode, IFixtureItemNodeReceiver> receiverFactory,
            CreateChildrenFactory createChildrenQuery,
            IFixtureItemNodeSender sender,
            IFixtureItemInformation fixtureItemInformation,
            IFixtureItemNodeValueViewModel fixtureItemNodeValueViewModel,
            Maybe<IFixtureItemNode> parent);

        /// <summary>
        /// Creates a function for creating a <see cref="IFixtureItemNodeReceiver"/>.
        /// </summary>
        /// <param name="isListening">Value indicating if the receiver is listening.</param>
        /// <returns>Function for creating a <see cref="IFixtureItemNodeReceiver"/>.</returns>
        Func<IFixtureItemNode, IFixtureItemNodeReceiver> CreateReceiverFunc(bool isListening);

        /// <summary>
        /// Creates a function for creating a <see cref="IFixtureItemNodeReceiver"/> for a constructor node.
        /// </summary>
        /// <param name="isListening">Value indicating if the receiver is listening.</param>
        /// <returns>Function for creating a <see cref="IFixtureItemNodeReceiver"/>.</returns>
        Func<IFixtureItemNode, IFixtureItemNodeReceiver> CreateCtorReceiverFunc(bool isListening);

        /// <summary>
        /// Create a new instance of <see cref="IFixtureItemNodeSender"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="IFixtureItemNodeSender"/>.</returns>
        IFixtureItemNodeSender CreateSender();

        /// <summary>
        /// Create a new instance of <see cref="IFixtureItemNodeSender"/> for a parameter node.
        /// </summary>
        /// <returns>A new instance of <see cref="IFixtureItemNodeSender"/>.</returns>
        IFixtureItemNodeSender CreateParameterSender();

        /// <summary>
        /// Create a new instance of <see cref="IFixtureItemNodeSender"/> for a method.
        /// </summary>
        /// <returns>A new instance of <see cref="IFixtureItemNodeSender"/>.</returns>
        IFixtureItemNodeSender CreateMethodSender();

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViCommon.EnsureHelper;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Interfaces;
using Twizzar.Fixture.Design.CoreInterfaces.Command.Services;
using Twizzar.Fixture.Design.CoreInterfaces.Query.Services.ReadModel;
using Twizzar.Fixture.Design.Ui.Interfaces.Factories;
using Twizzar.Fixture.Design.Ui.Interfaces.ViewModels;
using Twizzar.Fixture.Design.Ui.Interfaces.ViewModels.FixtureItemViewModelNodes;
using Twizzar.Fixture.Design.Ui.ViewModels.Nodes;
using Twizzar.Fixture.SharedKernelInterfaces.FixtureItem;
using IFixtureItemNode = Twizzar.Fixture.Design.Ui.Interfaces.ViewModels.FixtureItemViewModelNodes.IFixtureItemNode;

namespace Twizzar.VsAddin.Factory
{
    public class FixtureItemNodeViewModelFactory : IFixtureItemNodeViewModelFactory
    {
        private readonly ReceiverFactory _receiverFactory;

        private readonly DefaultSenderFactory _defaultSenderFactory;

        private readonly FixtureItemViewModelNodeFactory _fixtureItemViewModelNodeFactory;

        private readonly IEventSourcingRegisterService _registerService;


        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        public delegate FixtureItemNodeRecevier ReceiverFactory(IFixtureItemNode fixtureItemNode);

        public delegate FixtureItemNodeSender DefaultSenderFactory();

        public delegate FixtureItemViewModelNode FixtureItemViewModelNodeFactory(
            Func<IFixtureItemNode, IFixtureItemNodeRecevier> receiverFactory,
            Func<Maybe<IFixtureItemNode>, IFixtureInformation, Task<IEnumerable<IFixtureItemViewModelNode>>>
                createChildrenQuery,
            IFixtureItemNodeSender sender,
            IFixtureInformation fixtureInformation,
            IFixtureItemValueViewModel fixtureItemValueViewModel,
            Maybe<IFixtureItemNode> parent);

        #region Implementation of IFixtureItemNodeViewModelFactory

        /// <inheritdoc />
        public IFixtureItemViewModelNode CreateBaseTypeFixtureItemViewModel(
            IFixtureInformation fixtureInformation,
            IFixtureItemValueViewModel fixtureItemValueViewModel,
            Maybe<IFixtureItemNode> parent) =>
            this._fixtureItemViewModelNodeFactory(
                this.CreateRecevier,
                (maybe, information) => Task.FromResult(Enumerable.Empty<IFixtureItemViewModelNode>()),
                this._defaultSenderFactory(),
                fixtureInformation,
                fixtureItemValueViewModel,
                parent);

        /// <inheritdoc />
        public IFixtureItemViewModelNode CreateConstructorViewModel(
            FixtureItemConstructorModel ctorModel,
            FixtureItemId id,
            Maybe<IFixtureItemNode> parent) =>
            throw new System.NotImplementedException();

        /// <inheritdoc />
        public IFixtureItemViewModelNode CreateMemberViewModel(
            Func<Maybe<IFixtureItemNode>, IFixtureInformation, Task<IEnumerable<IFixtureItemViewModelNode>>> createChildrenFunc,
            IFixtureInformation fixtureInformation,
            IFixtureItemValueViewModel fixtureItemValueViewModel,
            Maybe<IFixtureItemNode> parent) =>
            this._fixtureItemViewModelNodeFactory(
                this.CreateRecevier,
                createChildrenFunc,
                this._defaultSenderFactory(),
                fixtureInformation,
                fixtureItemValueViewModel,
                parent);

        /// <inheritdoc />
        public IFixtureItemViewModelNode CreateParameterViewModel(
            FixtureItemParameterModel parameterModel,
            FixtureItemId id,
            Maybe<IFixtureItemNode> parent) =>
            throw new System.NotImplementedException();

        private IFixtureItemNodeRecevier CreateRecevier(IFixtureItemNode node)
        {
            var recevier = this._receiverFactory(node);
            this.RegisterListener(recevier);
            return recevier;
        }
        private void RegisterListener<TEvent>(IEventListener<TEvent> listener)
            where TEvent : IEvent
        {
            this._registerService.RegisterListener(listener);
        }
        #endregion
    }
}
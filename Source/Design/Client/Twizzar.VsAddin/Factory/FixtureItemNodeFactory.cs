using System;
using System.Diagnostics.CodeAnalysis;

using Autofac;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.Ui.Interfaces.Factories;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes;
using Twizzar.SharedKernel.Factories;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.VsAddin.Factory
{
    /// <inheritdoc cref="IFixtureItemNodeFactory" />
    [ExcludeFromCodeCoverage]
    public class FixtureItemNodeFactory : FactoryBase, IFixtureItemNodeFactory
    {
        #region fields

        private readonly ViewModelFactory _viewModelFactory;
        private readonly ReceiverFactory _receiverFactory;
        private readonly CtorReceiverFactory _ctorNodeReceiverFactory;
        private readonly NodeSenderFactory _nodeSenderFactory;
        private readonly ParameterNodeSenderFactory _parameterNodeSenderFactory;
        private readonly MethodNodeSenderFactory _methodNodeSenderFactory;
        private readonly IEventSourcingRegisterService _registerService;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemNodeFactory"/> class.
        /// </summary>
        /// <param name="componentContext"></param>
        /// <param name="viewModelFactory"></param>
        /// <param name="receiverFactory"></param>
        /// <param name="ctorNodeReceiverFactory"></param>
        /// <param name="nodeSenderFactory"></param>
        /// <param name="parameterNodeSenderFactory"></param>
        /// <param name="registerService"></param>
        /// <param name="methodNodeSenderFactory"></param>
        public FixtureItemNodeFactory(
            IComponentContext componentContext,
            ViewModelFactory viewModelFactory,
            ReceiverFactory receiverFactory,
            CtorReceiverFactory ctorNodeReceiverFactory,
            NodeSenderFactory nodeSenderFactory,
            ParameterNodeSenderFactory parameterNodeSenderFactory,
            IEventSourcingRegisterService registerService,
            MethodNodeSenderFactory methodNodeSenderFactory)
            : base(componentContext)
        {
            this.EnsureMany()
                .Parameter(viewModelFactory, nameof(viewModelFactory))
                .Parameter(receiverFactory, nameof(receiverFactory))
                .Parameter(ctorNodeReceiverFactory, nameof(ctorNodeReceiverFactory))
                .Parameter(nodeSenderFactory, nameof(nodeSenderFactory))
                .Parameter(parameterNodeSenderFactory, nameof(parameterNodeSenderFactory))
                .Parameter(registerService, nameof(registerService))
                .Parameter(methodNodeSenderFactory, nameof(methodNodeSenderFactory))
                .ThrowWhenNull();

            this._viewModelFactory = viewModelFactory;
            this._receiverFactory = receiverFactory;
            this._ctorNodeReceiverFactory = ctorNodeReceiverFactory;
            this._nodeSenderFactory = nodeSenderFactory;
            this._parameterNodeSenderFactory = parameterNodeSenderFactory;
            this._registerService = registerService;
            this._methodNodeSenderFactory = methodNodeSenderFactory;
        }

        #endregion

        #region delegates

        /// <summary>
        /// Delegate factory for autofac.
        /// </summary>
        /// <param name="fixtureItemNode"></param>
        /// <param name="isListening"></param>
        /// <returns>A new instance of <see cref="CtorFixtureItemNodeReceiver"/>.</returns>
        public delegate CtorFixtureItemNodeReceiver CtorReceiverFactory(IFixtureItemNode fixtureItemNode, bool isListening);

        /// <summary>
        /// Delegate factory for autofac.
        /// </summary>
        /// <returns>A new instance of <see cref="FixtureItemNodeSender"/>.</returns>
        public delegate FixtureItemNodeSender NodeSenderFactory();

        /// <summary>
        /// Delegate factory for autofac.
        /// </summary>
        /// <returns>A new instance of <see cref="ParameterNodeSender"/>.</returns>
        public delegate ParameterNodeSender ParameterNodeSenderFactory();

        /// <summary>
        /// Delegate factory for autofac.
        /// </summary>
        /// <returns>A new instance of <see cref="MethodNodeSender"/>.</returns>
        public delegate MethodNodeSender MethodNodeSenderFactory();

        /// <summary>
        /// Delegate factory for autofac.
        /// </summary>
        /// <param name="fixtureItemNode"></param>
        /// <param name="isListening"></param>
        /// <returns>A new instance of <see cref="FixtureItemNodeReceiver"/>.</returns>
        public delegate FixtureItemNodeReceiver ReceiverFactory(IFixtureItemNode fixtureItemNode, bool isListening);

        /// <summary>
        /// Delegate factory for autofac.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="receiverFactory"></param>
        /// <param name="createChildrenQuery"></param>
        /// <param name="sender"></param>
        /// <param name="fixtureItemInformation"></param>
        /// <param name="fixtureItemNodeValueViewModel"></param>
        /// <param name="parent"></param>
        /// <returns>A new instance of <see cref="FixtureItemNodeViewModel"/>.</returns>
        public delegate FixtureItemNodeViewModel ViewModelFactory(
            NodeId id,
            Func<IFixtureItemNode, IFixtureItemNodeReceiver> receiverFactory,
            IFixtureItemNodeFactory.CreateChildrenFactory createChildrenQuery,
            IFixtureItemNodeSender sender,
            IFixtureItemInformation fixtureItemInformation,
            IFixtureItemNodeValueViewModel fixtureItemNodeValueViewModel,
            Maybe<IFixtureItemNode> parent);

        #endregion

        #region members

        /// <inheritdoc />
        public IFixtureItemNodeViewModel CreateViewModelNode(
            NodeId id,
            Func<IFixtureItemNode, IFixtureItemNodeReceiver> receiverFactory,
            IFixtureItemNodeFactory.CreateChildrenFactory createChildrenQuery,
            IFixtureItemNodeSender sender,
            IFixtureItemInformation fixtureItemInformation,
            IFixtureItemNodeValueViewModel fixtureItemNodeValueViewModel,
            Maybe<IFixtureItemNode> parent) =>
            this._viewModelFactory(
                id,
                receiverFactory,
                createChildrenQuery,
                sender,
                fixtureItemInformation,
                fixtureItemNodeValueViewModel,
                parent);

        /// <inheritdoc />
        public Func<IFixtureItemNode, IFixtureItemNodeReceiver> CreateReceiverFunc(bool isListening)
        {
            return node =>
            {
                var instance = this._receiverFactory(node, isListening);
                this.RegisterListener(instance);
                return instance;
            };
        }

        /// <inheritdoc />
        public Func<IFixtureItemNode, IFixtureItemNodeReceiver> CreateCtorReceiverFunc(bool isListening)
        {
            return node =>
            {
                var instance = this._ctorNodeReceiverFactory(node, isListening);
                this.RegisterListener(instance);
                return instance;
            };
        }

        /// <inheritdoc />
        public IFixtureItemNodeSender CreateSender() =>
            this._nodeSenderFactory();

        /// <inheritdoc />
        public IFixtureItemNodeSender CreateParameterSender() =>
            this._parameterNodeSenderFactory();

        /// <inheritdoc />
        public IFixtureItemNodeSender CreateMethodSender() =>
            this._methodNodeSenderFactory();

        private void RegisterListener(IFixtureItemNodeReceiver node)
        {
            this._registerService.RegisterListener<FixtureItemMemberChangedEvent>(node);
            this._registerService.RegisterListener<FixtureItemMemberChangedFailedEvent>(node);
        }

        #endregion
    }
}
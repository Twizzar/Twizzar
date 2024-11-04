using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using GalaSoft.MvvmLight.CommandWpf;

using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.Ui.Interfaces.Controller;
using Twizzar.Design.Ui.Interfaces.Factories;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.ViewModels;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.Status;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes
{
    /// <inheritdoc cref="IFixtureItemNodeViewModel" />
    [ExcludeFromCodeCoverage]
    public sealed class FixtureItemNodeViewModel : ViewModelBase,
        IFixtureItemNodeViewModel,
        IFixtureItemNode
    {
        #region fields

        private readonly IFixtureItemNodeFactory.CreateChildrenFactory _createChildrenQuery;
        private readonly IFixtureItemNodeSender _sender;
        private readonly IFixtureItemNodeStatusFactory _fixtureItemNodeStatusFactory;
        private readonly IFixtureItemNodeReceiver _receiver;
        private bool _isExpanded;

        private IFixtureItemInformation _fixtureItemInformation;
        private Maybe<IStatusIconViewModel> _memberChangedFailedIcon;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemNodeViewModel"/> class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="receiverFactory"></param>
        /// <param name="createChildrenQuery"></param>
        /// <param name="sender"></param>
        /// <param name="fixtureItemInformation"></param>
        /// <param name="fixtureItemNodeValueViewModel"></param>
        /// <param name="parent"></param>
        /// <param name="uiEventHub"></param>
        /// <param name="fixtureItemNodeStatusFactory"></param>
        public FixtureItemNodeViewModel(
            NodeId id,
            Func<IFixtureItemNode, IFixtureItemNodeReceiver> receiverFactory,
            IFixtureItemNodeFactory.CreateChildrenFactory createChildrenQuery,
            IFixtureItemNodeSender sender,
            IFixtureItemInformation fixtureItemInformation,
            IFixtureItemNodeValueViewModel fixtureItemNodeValueViewModel,
            Maybe<IFixtureItemNode> parent,
            IUiEventHub uiEventHub,
            IFixtureItemNodeStatusFactory fixtureItemNodeStatusFactory)
        {
            this.EnsureMany()
                .Parameter(receiverFactory, nameof(receiverFactory))
                .Parameter(createChildrenQuery, nameof(createChildrenQuery))
                .Parameter(sender, nameof(sender))
                .Parameter(fixtureItemInformation, nameof(fixtureItemInformation))
                .Parameter(fixtureItemNodeValueViewModel, nameof(fixtureItemNodeValueViewModel))
                .Parameter(parent, nameof(parent))
                .Parameter(uiEventHub, nameof(uiEventHub))
                .Parameter(fixtureItemNodeStatusFactory, nameof(fixtureItemNodeStatusFactory))
                .ThrowWhenNull();

            this.Id = id;
            this._createChildrenQuery = createChildrenQuery;
            this._sender = sender;
            this._fixtureItemNodeStatusFactory = fixtureItemNodeStatusFactory;
            this.FixtureItemInformation = fixtureItemInformation;
            this.Parent = parent;
            this.UiEventHub = uiEventHub;
            this.Value = fixtureItemNodeValueViewModel;
            this.Value.FixtureNodeVM = this;

            this.NodeValueController.SetValueAsync(
                    this.FixtureItemInformation.DisplayValue,
                    this.FixtureItemInformation.IsDefault)
                .Forget();

            this.ExpandChildMemberDefinition = new RelayCommand(
                this.CreateChildren,
                this.FixtureItemInformation.CanBeExpanded);

            this.StatusPanelViewModel = this._fixtureItemNodeStatusFactory.CreateStatusPanelViewModel();

            this._receiver = receiverFactory(this);
            this._receiver.FixtureInformationChanged += this.RefreshFixtureInformation;
            this.NodeValueController.ValueChanged += this.ValueControllerOnValueChanged;

            // Ctor nodes should be expanded by default.
            if (this.FixtureItemInformation.MemberConfiguration is CtorMemberConfiguration)
            {
                this.IsExpanded = true;
                this.CreateChildren();
            }
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public NodeId Id { get; }

        /// <inheritdoc cref="IFixtureItemNode" />
        public IFixtureItemInformation FixtureItemInformation
        {
            get => this._fixtureItemInformation;
            private set
            {
                if (this._fixtureItemInformation != value)
                {
                    this._fixtureItemInformation = value;
                    this.OnPropertyChanged(nameof(this.FixtureItemInformation));
                }
            }
        }

        /// <inheritdoc />
        public IFixtureItemNodeValueController NodeValueController => (IFixtureItemNodeValueController)this.Value;

        /// <inheritdoc />
        public Maybe<IFixtureItemNode> Parent { get; }

        /// <inheritdoc />
        public IUiEventHub UiEventHub { get; }

        /// <inheritdoc />
        /// <summary>
        /// Gets or Sets a value indicating whether the tree view item is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get => this._isExpanded;
            set
            {
                if (this._isExpanded != value)
                {
                    this._isExpanded = value;
                    this.OnPropertyChanged(nameof(this.IsExpanded));
                }
            }
        }

        /// <inheritdoc />
        public bool IsDisposed { get; private set; }

        /// <inheritdoc />
        public ICommand ExpandChildMemberDefinition { get; }

        /// <inheritdoc />
        public IStatusPanelViewModel StatusPanelViewModel { get; }

        /// <inheritdoc />
        public IFixtureItemNodeValueViewModel Value { get; }

        /// <inheritdoc />
        public ObservableCollection<IFixtureItemNodeViewModel> Children { get; } =
            new();

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        IEnumerable<IFixtureItemNode> IFixtureItemNode.Children => this.Children.Cast<IFixtureItemNode>();

        #endregion

        #region members

        /// <inheritdoc />
        public async Task CommitMemberConfig(IMemberConfiguration memberConfiguration)
        {
            if (this.IsDisposed)
            {
                return;
            }

            if (!this.FixtureItemInformation.MemberConfiguration.Equals(memberConfiguration))
            {
                await this._sender.UpdateMemberConfigAsync(
                    this,
                    memberConfiguration,
                    this.FixtureItemInformation,
                    this.Parent);
            }
        }

        /// <inheritdoc />
        public void UpdateFixtureItemId(FixtureItemId fixtureItemId)
        {
            this.FixtureItemInformation = this.FixtureItemInformation.With(fixtureItemId);
        }

        /// <inheritdoc />
        public async void RefreshFixtureInformation(IFixtureItemInformation fixtureItemInformation)
        {
            try
            {
                var oldInformation = this.FixtureItemInformation;

                this.FixtureItemInformation = fixtureItemInformation;

                await this.NodeValueController.SetValueAsync(
                    this.FixtureItemInformation.DisplayValue,
                    this.FixtureItemInformation.IsDefault);

                // When the configuration has change and this is not a Ctor update its children.
                // When this is a constructor the configuration changes every time a child updates is value.
                // This leads to the neglect of some error message display.
                if (fixtureItemInformation.MemberConfiguration is not CtorMemberConfiguration &&
                    !oldInformation.MemberConfiguration.Equals(fixtureItemInformation.MemberConfiguration) &&
                    !this.FixtureItemInformation.FixtureDescription.IsBaseType)
                {
                    this.StatusPanelViewModel.Icons.Clear();
                    this.Children.ForEach(model => model.Dispose());
                    this.Children.Clear();
                    await this.CreateChildrenAsync();
                }

                this.OnPropertyChanged(nameof(this.FixtureItemInformation));
            }
            catch (Exception e)
            {
                this.Log(e);
            }
        }

        /// <inheritdoc />
        public async void DisplayOnMemberChangedFailed(FixtureItemMemberChangedFailedEvent e)
        {
            try
            {
                this.FixtureItemInformation = this.FixtureItemInformation
                    .With(e.MemberConfiguration)
                    .WithNotExpandable();

                await this.NodeValueController.SetValueAsync(
                    this.FixtureItemInformation.DisplayValue,
                    this.FixtureItemInformation.IsDefault);

                this._memberChangedFailedIcon.IfSome(model => this.StatusPanelViewModel.Remove(model));

                this._memberChangedFailedIcon = Maybe.Some(
                    this._fixtureItemNodeStatusFactory.CreateErrorStatusIconViewModel(e.Reason));

                this.StatusPanelViewModel.Add(this._memberChangedFailedIcon.GetValueUnsafe());
            }
            catch (Exception exp)
            {
                this.Log(exp);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.IsDisposed = true;
            this._receiver.FixtureInformationChanged -= this.RefreshFixtureInformation;
            this.NodeValueController.ValueChanged -= this.ValueControllerOnValueChanged;
            this.NodeValueController.Dispose();
            this._receiver.Dispose();
            this.Children.ForEach(model => model.Dispose());
        }

        [SuppressMessage(
            "Major Bug",
            "S3168:\"async\" methods should not return \"void\"",
            Justification = "Called by an event.")]
        private async void ValueControllerOnValueChanged(IViToken token)
        {
            if (this.IsDisposed)
            {
                return;
            }

            try
            {
                await Task.Run(() =>
                    this._sender.ChangeValue(
                        this,
                        token,
                        this.FixtureItemInformation,
                        this.Parent));
            }
            catch (Exception e)
            {
                this.Logger?.Log(e);
            }
        }

        private void CreateChildren()
        {
            this.CreateChildrenAsync().Forget();
        }

        /// <summary>
        /// Creates the child member definition view models.
        /// </summary>
        private async Task CreateChildrenAsync()
        {
            if (this.Children.Any())
            {
                return;
            }

            await this._createChildrenQuery(this, this.FixtureItemInformation)
                .DoAsync(
                    models =>
                    {
                        Application.Current.Dispatcher.Invoke(
                            () =>
                            {
                                this.Children.AddRange(models);

                                if (!this.Children.Any())
                                {
                                    this.StatusPanelViewModel.Add(
                                        this.FixtureItemInformation.FixtureDescription.IsArray
                                            ? this._fixtureItemNodeStatusFactory
                                                .CreateArrayNotConfigurableStatusIconViewModel(
                                                    this.StatusPanelViewModel)
                                            : this._fixtureItemNodeStatusFactory
                                                .CreateNoConfigurableMemberStatusIconViewModel(
                                                    this.StatusPanelViewModel));

                                    this.FixtureItemInformation = this.FixtureItemInformation.WithNotExpandable();
                                }
                                else
                                {
                                    this.RemoveNotConfigurableStatusIcon();
                                }
                            });
                    },
                    failure =>
                    {
                        Application.Current.Dispatcher.Invoke(
                            () =>
                            {
                                this.StatusPanelViewModel.Add(
                                    this._fixtureItemNodeStatusFactory.CreateErrorStatusIconViewModel(failure.Message));

                                this.IsExpanded = false;
                            });

                        this.FixtureItemInformation = this.FixtureItemInformation.WithNotExpandable();
                        this.Log(failure.Message, LogLevel.Error);
                    });
        }

        private void RemoveNotConfigurableStatusIcon()
        {
            var icon = this.StatusPanelViewModel.Icons.FirstOrDefault(
                model => model is NoConfigurableMemberStatusIconViewModel);

            if (icon != null)
            {
                this.StatusPanelViewModel.Remove(icon);
            }
        }
        #endregion
    }
}
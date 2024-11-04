using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Command.Commands;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Ui.Interfaces.Controller;
using Twizzar.Design.Ui.Interfaces.Factories;
using Twizzar.Design.Ui.Interfaces.Messaging.VsEvents;
using Twizzar.Design.Ui.Interfaces.Queries;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.ViewModels;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.FixtureInformations;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Ui.ViewModels.FixtureItem
{
    /// <summary>
    /// The MainViewModel of the fixture design.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class FixtureItemViewModel : ViewModelBase, IFixtureItemViewModel
    {
        #region fields

        private readonly IFixtureItemNodeViewModelQuery _fixtureItemNodeViewModelQuery;
        private readonly IUiEventHub _eventHub;
        private readonly IVsEventCache _vsEventCache;
        private readonly IFixtureItemNodeStatusFactory _fixtureItemNodeStatusFactory;
        private readonly ICommandBus _commandBus;

        private CancellationTokenSource _cancellationTokenSource = new();

        private Maybe<FixtureItemInstance> _fixtureItemInstance;

        private bool _isLoading;
        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemViewModel"/> class.
        /// </summary>
        /// <param name="fixtureItemNodeViewModelQuery">The fixture definition Repository.</param>
        /// <param name="eventHub">The ui event hub.</param>
        /// <param name="vsEventCache">The vs event cache for checking if the <see cref="ProjectReferencesLoadedEvent"/> has already occurred.</param>
        /// <param name="fixtureItemNodeStatusFactory"></param>
        /// <param name="commandBus">The command bus.</param>
        public FixtureItemViewModel(
            IFixtureItemNodeViewModelQuery fixtureItemNodeViewModelQuery,
            IUiEventHub eventHub,
            IVsEventCache vsEventCache,
            IFixtureItemNodeStatusFactory fixtureItemNodeStatusFactory,
            ICommandBus commandBus)
        {
            this.EnsureMany()
                .Parameter(fixtureItemNodeViewModelQuery, nameof(fixtureItemNodeViewModelQuery))
                .Parameter(eventHub, nameof(eventHub))
                .Parameter(vsEventCache, nameof(vsEventCache))
                .Parameter(fixtureItemNodeStatusFactory, nameof(fixtureItemNodeStatusFactory))
                .Parameter(commandBus, nameof(commandBus))
                .ThrowWhenNull();

            this.Id = new NodeId();
            this._fixtureItemNodeViewModelQuery = fixtureItemNodeViewModelQuery;
            this._eventHub = eventHub;
            this._vsEventCache = vsEventCache;
            this._fixtureItemNodeStatusFactory = fixtureItemNodeStatusFactory;
            this._commandBus = commandBus;
            this.IsLoading = true;

            eventHub.Subscribe<ProjectReferencesLoadedEvent>(this, this.HandleProjectReferencesLoaded);
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public NodeId Id { get; }

        /// <inheritdoc />
        public IFixtureItemInformation FixtureItemInformation { get; private set; }

        /// <inheritdoc />
        public IFixtureItemNodeValueController NodeValueController => null;

        /// <inheritdoc />
        public ObservableCollection<IFixtureItemNodeViewModel> Children { get; } =
            new ObservableCollection<IFixtureItemNodeViewModel>();

        /// <inheritdoc />
        public Maybe<IFixtureItemNode> Parent => Maybe.None();

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public bool IsLoading
        {
            get => this._isLoading;
            private set
            {
                if (this._isLoading != value)
                {
                    this._isLoading = value;
                    this.OnPropertyChanged(nameof(this.IsLoading));
                }
            }
        }

        /// <inheritdoc />
        IEnumerable<IFixtureItemNode> IFixtureItemNode.Children => this.Children.Cast<IFixtureItemNode>();

        #endregion

        #region members

        /// <inheritdoc />
        public async Task CommitMemberConfig(IMemberConfiguration memberConfiguration)
        {
            if (!this.FixtureItemInformation.MemberConfiguration.Equals(memberConfiguration) &&
                memberConfiguration is LinkMemberConfiguration linkMemberConfiguration &&
                linkMemberConfiguration.ConfigurationLink.Name.IsSome)
            {
                var typeName = $"{linkMemberConfiguration.ConfigurationLink.TypeFullName.GetTypeName().Replace("`", string.Empty)}";
                var configName = typeName + Guid.NewGuid().ToString("N").Substring(8, 4);
                await this.UpdateFixtureItemNameAsync(configName);
            }

            this.FixtureItemInformation = this.FixtureItemInformation.With(memberConfiguration);
        }

        /// <inheritdoc />
        public void UpdateFixtureItemId(FixtureItemId fixtureItemId)
        {
            this.FixtureItemInformation = this.FixtureItemInformation.With(fixtureItemId);
        }

        /// <inheritdoc />
        public void RefreshFixtureInformation(IFixtureItemInformation fixtureItemInformation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void DisplayOnMemberChangedFailed(FixtureItemMemberChangedFailedEvent e)
        {
            // this should never happened because the root is not a member.
            this._fixtureItemInstance.IfSome(instance => instance.DisplayOnMemberChangedFailed(e));
        }

        /// <inheritdoc />
        public async Task InitializeAsync(
            IAdornmentInformation adornmentInformation,
            AdornmentId adornmentId,
            IDocumentWriter documentWriter,
            IStatusPanelViewModel statusPanelViewModel,
            ICompilationTypeQuery compilationTypeQuery,
            CancellationToken cancellationToken)
        {
            this.EnsureMany()
                .Parameter(adornmentInformation, nameof(adornmentInformation))
                .Parameter(adornmentId, nameof(adornmentId))
                .Parameter(statusPanelViewModel, nameof(statusPanelViewModel))
                .ThrowWhenNull();

            if (!this._cancellationTokenSource.IsCancellationRequested)
            {
                this._cancellationTokenSource.Cancel();
            }

            this.FixtureItemInformation = RootFixtureItemInformation.Create(adornmentInformation.FixtureItemId);

            var instance = new FixtureItemInstance(
                this,
                adornmentInformation,
                adornmentId,
                documentWriter,
                statusPanelViewModel,
                this._commandBus,
                compilationTypeQuery);

            this._fixtureItemInstance = instance;

            if (this._vsEventCache.AllReferencesAreLoaded(adornmentInformation.ProjectName))
            {
                this.IsLoading = false;
            }

            this._cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            await instance.LoadMembersAsync(this._cancellationTokenSource.Token);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(IAdornmentInformation adornmentInformation)
        {
            if (this._fixtureItemInstance.AsMaybeValue() is SomeValue<FixtureItemInstance> someInstance &&
                someInstance.Value.AdornmentInformation != adornmentInformation)
            {
                this._cancellationTokenSource.Cancel();
                this._cancellationTokenSource = new CancellationTokenSource();
                var instance = someInstance.Value.With(adornmentInformation);
                this._fixtureItemInstance = instance;

                if (someInstance.Value.FixtureItemId != adornmentInformation.FixtureItemId)
                {
                    this.FixtureItemInformation =
                        RootFixtureItemInformation.Create(adornmentInformation.FixtureItemId);

                    await instance.LoadMembersAsync(this._cancellationTokenSource.Token);
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this._cancellationTokenSource.Cancel();
            this._cancellationTokenSource.Dispose();
            this._fixtureItemInstance = Maybe.None();
            this.Children.ForEach(model => model.Dispose());
            this._eventHub.Subscribe<ProjectReferencesLoadedEvent>(this, this.HandleProjectReferencesLoaded);
            this.OnPropertyChanged(nameof(this.Children));
        }

        private async Task UpdateFixtureItemNameAsync(string newName)
        {
            await this._fixtureItemInstance.IfSomeAsync(instance => instance.UpdateFixtureItemNameAsync(newName));
        }

        [SuppressMessage(
            "Major Bug",
            "S3168:\"async\" methods should not return \"void\"",
            Justification = "Called by event hub.")]
        private async void HandleProjectReferencesLoaded(ProjectReferencesLoadedEvent e)
        {
            this.IsLoading = false;

            await this._fixtureItemInstance.IfSomeAsync(
                async instance =>
                {
                    if (instance.AdornmentInformation.ProjectName == e.Project.Name)
                    {
                        await instance.LoadMembersAsync(this._cancellationTokenSource.Token);
                    }
                });
        }

        #endregion

        #region Nested type: FixtureItemInstance

        private sealed class FixtureItemInstance
        {
            #region fields

            private readonly FixtureItemViewModel _model;
            private readonly AdornmentId _adornmentId;
            private readonly IDocumentWriter _documentWriter;
            private readonly IStatusPanelViewModel _statusPanelViewModel;
            private readonly ICommandBus _commandBus;
            private readonly ICompilationTypeQuery _compilationTypeQuery;

            #endregion

            #region ctors

            public FixtureItemInstance(
                FixtureItemViewModel model,
                IAdornmentInformation adornmentInformation,
                AdornmentId adornmentId,
                IDocumentWriter documentWriter,
                IStatusPanelViewModel statusPanelViewModel,
                ICommandBus commandBus,
                ICompilationTypeQuery compilationTypeQuery)
            {
                this._model = model;
                this.AdornmentInformation = adornmentInformation;
                this._adornmentId = adornmentId;
                this._documentWriter = documentWriter;
                this._statusPanelViewModel = statusPanelViewModel;
                this._commandBus = commandBus;
                this._compilationTypeQuery = compilationTypeQuery;
                this.FixtureItemId = adornmentInformation.FixtureItemId;

                this.ClearStatusIcons();
            }

            #endregion

            #region properties

            public IAdornmentInformation AdornmentInformation { get; }

            public FixtureItemId FixtureItemId { get; }

            #endregion

            #region members

            public FixtureItemInstance With(IAdornmentInformation adornmentInformation) =>
                new(
                    this._model,
                    adornmentInformation,
                    this._adornmentId,
                    this._documentWriter,
                    this._statusPanelViewModel,
                    this._commandBus,
                    this._compilationTypeQuery);

            public async Task LoadMembersAsync(CancellationToken cancellationToken)
            {
                if (this._model.IsLoading)
                {
                    return;
                }

                var nodes =
                    await this._model._fixtureItemNodeViewModelQuery.GetFixtureItemNodeViewModels(
                        this.FixtureItemId,
                        Maybe.None(),
                        this._model,
                        this._compilationTypeQuery);

                nodes.Do(
                    models => Application.Current.Dispatcher.Invoke(
                        () => this.UpdateChildren(models, cancellationToken)),
                    f =>
                    {
                        this._statusPanelViewModel.Add(
                            this._model._fixtureItemNodeStatusFactory.CreateErrorStatusIconViewModel(f.Message));

                        this._model._eventHub.Publish(new LockAdornmentExpanderEvent(this._adornmentId));
                        this.Log(f.Message, LogLevel.Error);
                    });
            }

            public void DisplayOnMemberChangedFailed(FixtureItemMemberChangedFailedEvent e)
            {
                this._statusPanelViewModel.Add(
                    this._model._fixtureItemNodeStatusFactory.CreateErrorStatusIconViewModel(e.Reason));
            }

            public async Task UpdateFixtureItemNameAsync(string configName)
            {
                configName = $"{configName}Builder";

                await this._commandBus.SendAsync(
                    new CreateCustomBuilderCommand(
                        this._documentWriter,
                        configName,
                        this.AdornmentInformation));
            }

            private void UpdateChildren(
                IEnumerable<IFixtureItemNodeViewModel> models,
                CancellationToken cancellationToken)
            {
                var i = 0;

                foreach (var fixtureItemNodeViewModel in models)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    if (i < this._model.Children.Count)
                    {
                        this._model.Children[i].Dispose();
                        this._model.Children[i] = fixtureItemNodeViewModel;
                    }
                    else
                    {
                        this._model.Children.Add(fixtureItemNodeViewModel);
                    }

                    i++;
                }

                for (var j = this._model.Children.Count - 1; j >= i && !cancellationToken.IsCancellationRequested; j--)
                {
                    this._model.Children[j].Dispose();
                    this._model.Children.RemoveAt(j);
                }

                if (!this._model.Children.Any())
                {
                    this._statusPanelViewModel.Add(
                        this._model.FixtureItemInformation.TypeFullName.Cast().IsArray()
                            ? this._model._fixtureItemNodeStatusFactory.CreateArrayNotConfigurableStatusIconViewModel(this._statusPanelViewModel)
                            : this._model._fixtureItemNodeStatusFactory.CreateNoConfigurableMemberStatusIconViewModel(this._statusPanelViewModel));

                    this._model._eventHub.Publish(new LockAdornmentExpanderEvent(this._adornmentId));
                }
                else
                {
                    if (this._model.Children[0] is IFixtureItemNode node)
                    {
                        node.NodeValueController.Focus();
                    }

                    this.ClearStatusIcons();
                }
            }

            private void ClearStatusIcons()
            {
                if (this._statusPanelViewModel.Icons.Any())
                {
                    this._statusPanelViewModel.Icons.Clear();
                    this._model._eventHub.Publish(new ReleaseAdornmentExpanderEvent(this._adornmentId));
                }
            }

            #endregion
        }

        #endregion
    }
}
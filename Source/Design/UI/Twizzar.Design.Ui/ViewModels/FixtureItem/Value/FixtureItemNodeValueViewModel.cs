using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.VisualStudio.Shell;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.Ui.Interfaces.Controller;
using Twizzar.Design.Ui.Interfaces.Enums;
using Twizzar.Design.Ui.Interfaces.Messaging.VsEvents;
using Twizzar.Design.Ui.Interfaces.Parser;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Validator;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.ViewModels;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.Parser.SyntaxTree;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.NLog;
using Twizzar.SharedKernel.NLog.Logging;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Ui.ViewModels.FixtureItem.Value
{
    /// <inheritdoc cref="IFixtureItemNodeValueViewModel" />
    public sealed class FixtureItemNodeValueViewModel : ViewModelBase,
        IFixtureItemNodeValueViewModel,
        IFixtureItemNodeValueController,
        IHasEnsureHelper
    {
        #region fields

        private readonly IParser _parser;
        private readonly IValidator _validator;
        private readonly IValidTokenToItemValueSegmentsConverter _converter;
        private readonly IVsCommandQuery _vsCommandQuery;

        private readonly IUiEventHub _eventHub;

        private string _fullText;
        private IViToken _lastCommittedValue;
        private bool _hasFocus;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemNodeValueViewModel"/> class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parser">The parser used.</param>
        /// <param name="validator">The validator used.</param>
        /// <param name="converter">Converter for converting validated tokens to segments.</param>
        /// <param name="isReadOnly">If the value is read only.</param>
        /// <param name="uiEventHub">The ui event hub.</param>
        /// <param name="vsCommandQuery"></param>
        /// <param name="scopeServiceProviderFactory"></param>
        public FixtureItemNodeValueViewModel(
            NodeId id,
            IParser parser,
            IValidator validator,
            IValidTokenToItemValueSegmentsConverter converter,
            bool isReadOnly,
            IUiEventHub uiEventHub,
            IVsCommandQuery vsCommandQuery,
            IScopeServiceProviderFactory scopeServiceProviderFactory)
        {
            this.EnsureMany()
                .Parameter(id, nameof(id))
                .Parameter(parser, nameof(parser))
                .Parameter(validator, nameof(validator))
                .Parameter(converter, nameof(converter))
                .Parameter(uiEventHub, nameof(uiEventHub))
                .Parameter(vsCommandQuery, nameof(vsCommandQuery))
                .Parameter(scopeServiceProviderFactory, nameof(scopeServiceProviderFactory))
                .ThrowWhenNull();

            this.Id = id;
            this._parser = parser;
            this._validator = validator;
            this._converter = converter;
            this._vsCommandQuery = vsCommandQuery;
            this.IsReadOnly = isReadOnly;
            this._eventHub = uiEventHub;
            this.ServiceProvider = scopeServiceProviderFactory.CreateNew();

            this.Initialize();

            this.OnValidInputsChanged(this._validator.ValidInput);
            this._validator.OnInitialized += this.ValidatorOnInitialized;
            this._validator.OnValidInputsChanged += this.OnValidInputsChanged;
            this.ValidatorOnInitialized();
            this.Commit = new RelayCommand(this.CommitValue);
            this.ExpandCollapseCommand = new RelayCommand(this.ExpandOrCollapse);

            uiEventHub.Subscribe<AdornmentTypesInitializedEvent>(this, this.Handle);
        }

        #endregion

        #region events

        /// <inheritdoc />
        public event Action<IViToken> ValueChanged;

        #endregion

        #region properties

        /// <summary>
        /// Gets the node id.
        /// </summary>
        public NodeId Id { get; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public string FullText
        {
            get => this._fullText;
            set
            {
                if (!this.IsReadOnly && this._fullText != value)
                {
                    this._fullText = value;
                    this.Update();
                }
            }
        }

        /// <inheritdoc />
        public string DefaultValue => this._validator.DefaultValue;

        /// <inheritdoc />
        public string Tooltip => this._validator.Tooltip;

        /// <inheritdoc />
        public string AdornerText => this._validator.AdornerText;

        /// <inheritdoc />
        public bool HasFocus
        {
            get => this._hasFocus;
            set
            {
                if (this._hasFocus != value)
                {
                    this._hasFocus = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<ItemValueSegment> ItemValueSegments { get; private set; } =
            Enumerable.Empty<ItemValueSegment>();

        /// <inheritdoc />
        public IEnumerable<AutoCompleteEntry> AutoCompleteEntries { get; private set; } =
            Enumerable.Empty<AutoCompleteEntry>();

        /// <inheritdoc />
        public bool IsReadOnly { get; }

        /// <inheritdoc />
        public ICommand Commit { get; }

        /// <inheritdoc />
        public ICommand ExpandCollapseCommand { get; }

        /// <inheritdoc />
        public IFixtureItemNodeViewModel FixtureNodeVM { get; set; }

        /// <inheritdoc />
        public string ExpandAndCollapseShortcut { get; private set; }

        /// <summary>
        /// Gets a value indicating whether commit is dirty or not.
        /// </summary>
        public bool IsCommitDirty { get; private set; }

        /// <inheritdoc />
        public IScopedServiceProvider ServiceProvider { get; }

        #endregion

        #region members

        /// <inheritdoc />
        public async Task SetValueAsync(string value, bool isDefault)
        {
            // set default value as empty string.
            if (isDefault && !this.IsReadOnly)
            {
                value = string.Empty;
            }

            if (this._fullText != value)
            {
                this._fullText = value;
                var token = await this.UpdateAndPrepareAsync();
                token.IfSome(viToken => this._lastCommittedValue = viToken);
            }
        }

        /// <inheritdoc />
        public void Focus()
        {
            if (this.FixtureNodeVM.IsDisposed)
            {
                this.MoveFocusUp();
            }
            else
            {
                this._eventHub.Publish(new FixtureItemNodeFocusedEvent(this.Id));
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this._validator.OnInitialized -= this.ValidatorOnInitialized;
            this._validator.OnValidInputsChanged -= this.OnValidInputsChanged;
            this._eventHub.Unsubscribe<AdornmentTypesInitializedEvent>(this, this.Handle);
            this.ServiceProvider.Dispose();

            if (this.HasFocus)
            {
                this.MoveFocusUp();
            }
        }

        private void Handle(AdornmentTypesInitializedEvent obj)
        {
            if (!this.IsReadOnly)
            {
                try
                {
                    this._validator.InitializeAsync().Forget();
                }
                catch (Exception ex)
                {
                    this.Log(ex);
                }
            }
        }

        [SuppressMessage(
            "Major Bug",
            "S3168:\"async\" methods should not return \"void\"",
            Justification = "Ui async void ok.")]
        private async void Initialize()
        {
            try
            {
                this.ExpandAndCollapseShortcut =
                    await this._vsCommandQuery.GetShortCutOfOpenCloseCommandAsync();
                this._validator.InitializeAsync().Forget();
            }
            catch (Exception e)
            {
                this.Log(e);
            }
        }

        [SuppressMessage(
            "Major Bug",
            "S3168:\"async\" methods should not return \"void\"",
            Justification = "Ok for this UI method.")]
        private async void CommitValue()
        {
            try
            {
                if (this.IsReadOnly)
                {
                    return;
                }

                var validatedToken = await this.UpdateAndPrepareAsync();

                validatedToken.IfSome(
                    token =>
                    {
                        if (!this._lastCommittedValue.Equals(token))
                        {
                            this._lastCommittedValue = token;

                            // commit is dirty when token is empty token
                            // during onMemberChanged in memberViewModel.Listener,
                            // configuration will be reset in ui.
                            this.IsCommitDirty = token is ViEmptyToken;
                            this.ValueChanged?.Invoke(token);
                        }
                    });
            }
            catch (Exception e)
            {
                this.Logger?.Log(e);
            }
        }

        [SuppressMessage(
            "Major Bug",
            "S3168:\"async\" methods should not return \"void\"",
            Justification = "Ok for this UI method.")]
        private async void ValidatorOnInitialized()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            await this.UpdateAndPrepareAsync();
        }

        private async void OnValidInputsChanged(IEnumerable<string> validInputs)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            this.AutoCompleteEntries = validInputs.Select(
                s =>
                    new AutoCompleteEntry(s, AutoCompleteFormat.Keyword));

            this.OnPropertyChanged(nameof(this.AutoCompleteEntries));
        }

        /// <summary>
        /// Gets called every change.
        /// </summary>
        [SuppressMessage(
            "Major Bug",
            "S3168:\"async\" methods should not return \"void\"",
            Justification = "Ok for this UI method.")]
        private async void Update()
        {
            if (this.FullText == null)
            {
                return;
            }

            try
            {
                var tokens = await this.ParseAndValidateAsync();
                this.Update(tokens);
            }
            catch (Exception e)
            {
                this.Logger?.Log(e);
            }
        }

        private void Update(IViToken token)
        {
            try
            {
                this.ItemValueSegments = this._converter.ToItemValueSegments(token);

                this.OnPropertyChanged(nameof(this.FullText));
                this.OnPropertyChanged(nameof(this.ItemValueSegments));
                this.OnPropertyChanged(nameof(this.Tooltip));
                this.OnPropertyChanged(nameof(this.AdornerText));
            }
            catch (Exception e)
            {
                this.Logger?.Log(e);
            }
        }

        /// <summary>
        /// Get called on <see cref="SetValueAsync"/>, <see cref="CommitValue"/> and <see cref="ValidatorOnInitialized"/>.
        /// </summary>
        private async Task<Maybe<IViToken>> UpdateAndPrepareAsync()
        {
            try
            {
                if (this.FullText != null)
                {
                    var token = this._validator.Prettify(await this.ParseAndValidateAsync());
                    this.Update(token);
                    return Maybe.Some(token);
                }
            }
            catch (Exception e)
            {
                this.Logger?.Log(e);
            }

            return Maybe.None<IViToken>();
        }

        private Task<IViToken> ParseAndValidateAsync()
        {
            var token = this._parser.Parse(this._fullText);
            var validatedTokens = this._validator.ValidateAsync(token);
            return validatedTokens;
        }

        private void ExpandOrCollapse()
        {
            this.FixtureNodeVM.IsExpanded = !this.FixtureNodeVM.IsExpanded;
            this.FixtureNodeVM.ExpandChildMemberDefinition.Execute(null);
        }

        private void MoveFocusUp()
        {
            if (this.FixtureNodeVM is IFixtureItemNode node &&
                node.Parent.AsMaybeValue() is SomeValue<IFixtureItemNode> parent)
            {
                parent.Value?.NodeValueController?.Focus();
            }
        }

        #endregion
    }
}
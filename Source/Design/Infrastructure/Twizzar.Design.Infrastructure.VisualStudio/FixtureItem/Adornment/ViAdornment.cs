using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Common.Messaging.Events;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.Infrastructure.VisualStudio.Enums;
using Twizzar.Design.Infrastructure.VisualStudio.Extensions;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.Design.Ui.Interfaces.Factories;
using Twizzar.Design.Ui.Interfaces.Messaging.VsEvents;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status;
using Twizzar.Design.Ui.Interfaces.VisualStudio;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.SharedKernel.CoreInterfaces.Util;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable VSTHRD110 // Observe result of async calls

namespace Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment
{
    /// <inheritdoc cref="IViAdornment" />
    public sealed class ViAdornment : Entity<ViAdornment, AdornmentId>, IViAdornment
    {
        #region fields

        private readonly IWpfTextView _view;
        private readonly IUiEventHub _eventHub;
        private readonly ISnapshotHistory _snapshotHistory;
        private readonly IDocumentAdornmentController _documentAdornmentController;

        private bool _isExpanded = false;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViAdornment"/> class.
        /// </summary>
        /// <param name="elementsFactory">The element factory.</param>
        /// <param name="adornmentInformation">The adornment information.</param>
        /// <param name="textView">The text view.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="snapshotHistory">The snapshot history.</param>
        /// <param name="nodeStatusFactory"></param>
        /// <param name="documentAdornmentController">The adornment controller.</param>
        /// <param name="baseTypeService"></param>
        /// <param name="fixtureItemNodeStatusFactory"></param>
        [SuppressMessage(
            "Major Code Smell",
            "S107:Methods should not have too many parameters",
            Justification = "Resolved over IoC")]
        public ViAdornment(
            IUiElementsFactory elementsFactory,
            IAdornmentInformation adornmentInformation,
            ISnapshotHistory snapshotHistory,
            IWpfTextView textView,
            IUiEventHub eventHub,
            IFixtureItemNodeStatusFactory nodeStatusFactory,
            IDocumentAdornmentController documentAdornmentController,
            IBaseTypeService baseTypeService,
            IFixtureItemNodeStatusFactory fixtureItemNodeStatusFactory)
            : base(AdornmentId.CreateNew(adornmentInformation.ProjectName))
        {
            this.EnsureMany()
                .Parameter(elementsFactory, nameof(elementsFactory))
                .Parameter(adornmentInformation, nameof(adornmentInformation))
                .Parameter(snapshotHistory, nameof(snapshotHistory))
                .Parameter(eventHub, nameof(eventHub))
                .Parameter(nodeStatusFactory, nameof(nodeStatusFactory))
                .Parameter(textView, nameof(textView))
                .Parameter(textView, nameof(textView))
                .Parameter(documentAdornmentController, nameof(documentAdornmentController))
                .Parameter(baseTypeService, nameof(baseTypeService))
                .Parameter(fixtureItemNodeStatusFactory, nameof(fixtureItemNodeStatusFactory))
                .ThrowWhenNull();

            this.Id = this.EntityId;
            this.AdornmentInformation = adornmentInformation;
            this._view = textView;

            this._eventHub = eventHub;
            this._documentAdornmentController = documentAdornmentController;
            this._snapshotHistory = snapshotHistory;
            this.StatusPanelViewModel = nodeStatusFactory.CreateStatusPanelViewModel();

            var typeFullName = adornmentInformation.FixtureItemId.TypeFullName;
            this.AdornmentExpander = elementsFactory.CreateAdornmentExpander(this.Id, this.StatusPanelViewModel);

            if (baseTypeService.IsBaseType(typeFullName) || baseTypeService.IsNullableBaseType(typeFullName))
            {
                this.StatusPanelViewModel.Add(
                    fixtureItemNodeStatusFactory.CreateBaseTypeIsAlwaysUniqueStatusIconViewModel(
                        this.StatusPanelViewModel));

                this.AdornmentExpander.HideExpanderButton();
            }

            if (textView.Caret != null)
            {
                textView.Caret.PositionChanged += this.CaretOnPositionChanged;
            }

            eventHub.Subscribe<AdornmentExpandedOrCollapsedEvent>(this, this.AdornmentExpandedOrCollapsed);
            eventHub.Subscribe<PeekCollapsedEvent>(this, this.PeekViewCollapsed);
            eventHub.Subscribe<FixtureItemPanelLeaveRequestEvent>(this, this.LeavePanel);
            eventHub.Subscribe<VsOpenOrCloseShortcutPressedEvent>(this, this.OpenOrClosePressed);
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public AdornmentId Id { get; }

        /// <inheritdoc />
        public IAdornmentInformation AdornmentInformation { get; private set; }

        /// <inheritdoc />
        public IAdornmentExpander AdornmentExpander { get; }

        /// <inheritdoc />
        public IStatusPanelViewModel StatusPanelViewModel { get; }

        #endregion

        #region members

        /// <inheritdoc />
        public void Update(IAdornmentInformation adornmentInformation)
        {
            this.AdornmentInformation = adornmentInformation;

            if (this._isExpanded)
            {
                this._documentAdornmentController.UpdateInformationAsync(adornmentInformation);
            }
        }

        /// <inheritdoc />
        public void UpdateIsExpanded(bool isExpanded)
        {
            this._isExpanded = isExpanded;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this._eventHub.Unsubscribe<AdornmentExpandedOrCollapsedEvent>(this, this.AdornmentExpandedOrCollapsed);
            this._eventHub.Unsubscribe<VsOpenOrCloseShortcutPressedEvent>(this, this.OpenOrClosePressed);
            this._eventHub.Unsubscribe<PeekCollapsedEvent>(this, this.PeekViewCollapsed);
            this.AdornmentExpander.Dispose();

            if (this._view.Caret != null)
            {
                this._view.Caret.PositionChanged -= this.CaretOnPositionChanged;
            }

            if (this._isExpanded)
            {
                this._documentAdornmentController.CloseAdornmentAsync(this);
            }
        }

        /// <inheritdoc />
        protected override bool Equals(AdornmentId a, AdornmentId b) => a == b;

        private void CaretOnPositionChanged(object sender, CaretPositionChangedEventArgs args)
        {
            if (!this._isExpanded)
            {
                return;
            }

            var oldLine = args.OldPosition.BufferPosition.GetLineNumber();
            var newLine = args.NewPosition.BufferPosition.GetLineNumber();
            var peekViewLine = this.GetPeekViewLine();

            // if the caret was on the peek view line and moved down.
            if (oldLine == peekViewLine && newLine == oldLine + 1)
            {
                this._documentAdornmentController.FocusFixturePanel(ViEnterFocusPosition.First);
            }

            // if the caret was directly under the peek view line and moved up.
            else if (oldLine == peekViewLine + 1 && newLine == oldLine - 1)
            {
                this._documentAdornmentController.FocusFixturePanel(ViEnterFocusPosition.Last);
            }
        }

        private int GetPeekViewLine() =>
            this.GetLineNumber(
                this.AdornmentInformation.ObjectCreationSpan.End());

        private void LeavePanel(FixtureItemPanelLeaveRequestEvent e)
        {
            if (!this._isExpanded)
            {
                return;
            }

            // get the peek view line and the next line.
            var lines = this._view.TextViewLines
                .SkipWhile(viewLine => !viewLine.ContainsBufferPosition(this.GetInvocationSnapshotSpan().End))
                .Take(
                    2) // Take enumerates source and yields elements until count elements have been yielded or source contains no more elements.
                .ToList();

            if (lines.Count == 0)
            {
                this.Log(
                    "Trying to leave the Fixture Item Panel but failed to get the line where the adornment is placed.",
                    LogLevel.Warn);
            }

            switch (e.Direction)
            {
                case FixtureItemPanelLeaveRequestEvent.LeaveDirection.Up when lines.Count > 0:
                    this._view.Caret.MoveTo(lines[0]);
                    break;
                case FixtureItemPanelLeaveRequestEvent.LeaveDirection.Down when lines.Count > 1:
                    this._view.Caret.MoveTo(lines[1]);
                    break;
            }

            this._view.VisualElement.Focus();
        }

        private int GetLineNumber(int position) => this._view.TextSnapshot.GetLineNumberFromPosition(position);

        [ExcludeFromCodeCoverage]
        private SnapshotSpan GetInvocationSnapshotSpan()
        {
            var oldVersion = this.AdornmentInformation.ObjectCreationSpan.Version;

            var oldSnapshot = this._snapshotHistory.Get(oldVersion)
                .SomeOrProvided(() =>
                {
                    this.Log("Cannot find version in version history", LogLevel.Error);
                    return this._view.TextSnapshot;
                });

            var span = new SnapshotSpan(oldSnapshot, this.AdornmentInformation.ObjectCreationSpan.ToSpan());

            return this._view.TextSnapshot.Version.VersionNumber == oldVersion.VersionNumber
                ? span
                : span.TranslateTo(this._view.TextSnapshot, SpanTrackingMode.EdgeExclusive);
        }

        private async void PeekViewCollapsed(PeekCollapsedEvent e)
        {
            try
            {
                // When the event is not addressed to this adornment or the isExpanded state is false
                if (e.AdornmentId != this.Id || !this._isExpanded)
                {
                    return;
                }

                this._isExpanded = false;
                await this._documentAdornmentController.CloseAdornmentAsync(this);
            }
            catch (Exception ex)
            {
                this.Log(ex);
            }
        }

        [ExcludeFromCodeCoverage]
        private void AdornmentExpandedOrCollapsed(AdornmentExpandedOrCollapsedEvent e)
        {
            using var o = ViMonitor.StartOperation(nameof(this.AdornmentExpandedOrCollapsed));

            // When the event is not addressed to this adornment or the isExpanded state is already set correctly then do nothing.
            if (e.AdornmentId != this.Id || this._isExpanded == e.IsExpanded)
            {
                return;
            }

            this._isExpanded = e.IsExpanded;

            if (e.IsExpanded)
            {
                var expandEvent = new EventTelemetry("adornmentExpanded")
                {
                    Properties =
                    {
                        ["AdornmentId"] = this.Id.ToString(),
                        ["FixtureItemIdHash"] = this.AdornmentInformation.FixtureItemId.GetHashCode().ToString(),
                        ["ProjectNameHash"] = this.AdornmentInformation.ProjectName.GetHashCode().ToString(),
                    },
                };

                ViMonitor.TrackEvent(expandEvent);
                this._documentAdornmentController.OpenAdornmentAsync(this, this.GetInvocationSnapshotSpan());
            }
            else
            {
                var collapsedEvent = new EventTelemetry("adornmentCollapsed")
                {
                    Properties =
                    {
                        ["AdornmentId"] = this.Id.ToString(),
                        ["FixtureItemIdHash"] = this.AdornmentInformation.FixtureItemId.GetHashCode().ToString(),
                        ["ProjectNameHash"] = this.AdornmentInformation.ProjectName.GetHashCode().ToString(),
                    },
                };

                ViMonitor.TrackEvent(collapsedEvent);
                ViMonitor.Flush();

                this._documentAdornmentController.CloseAdornmentAsync(this);
            }
        }

        private void OpenOrClosePressed(VsOpenOrCloseShortcutPressedEvent obj)
        {
            if (!this._view.HasAggregateFocus)
            {
                return;
            }

            var invocationSpan = this.AdornmentInformation.ObjectCreationSpan;

            var (caretLineCount, caretLineSpan, invocationStartLineCount, invocationEndLineCount) =
                this.GetCurrentLinePositions();

            var distance = invocationEndLineCount - invocationStartLineCount;

            // return if not correct adornment
            if (Math.Abs(caretLineCount - invocationStartLineCount) > distance)
            {
                return;
            }

            // when single line adornment, match line number
            if (!this.AdornmentInformation.MultipleAdornmentsOnLine &&
                caretLineCount >= invocationStartLineCount &&
                caretLineCount <= invocationEndLineCount)
            {
                this.ToggleExpanderState();
                return;
            }

            // If multiple adornments on line, match exactly on invocation span
            if (caretLineSpan.IntersectsWith(invocationSpan.ToSpan()))
            {
                this.ToggleExpanderState();
            }
        }

        private void ToggleExpanderState()
        {
            if (this._isExpanded)
            {
                this._documentAdornmentController.CloseAdornmentAsync(this);
                this._isExpanded = false;
            }
            else
            {
                this._documentAdornmentController.OpenAdornmentAsync(this, this.GetInvocationSnapshotSpan());
                this._isExpanded = true;
            }

            this.AdornmentExpander.ToggleExpander();
        }

        private (int CaretLineCount, SnapshotSpan CaretLineSpan, int InvocationStartLineCount, int
            InvocationEndLineCount) GetCurrentLinePositions()
        {
            var caretPosition = this._view.Caret.Position.BufferPosition;

            var caretLineSpan = this._view
                .GetTextViewLineContainingBufferPosition(caretPosition)
                .GetTextElementSpan(caretPosition);

            var caretLineCount = caretLineSpan.Start.GetLineNumber();
            var invocationStartLineCount = this.GetLineNumber(this.AdornmentInformation.ObjectCreationSpan.Start);
            var invocationEndLineCount = this.GetPeekViewLine();

            return (caretLineCount, caretLineSpan, invocationStartLineCount, invocationEndLineCount);
        }

        #endregion
    }
}
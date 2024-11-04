using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.Enums;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.Design.Ui.Interfaces.VisualStudio;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment
{
    /// <inheritdoc cref="IDocumentAdornmentController" />
    public sealed class DocumentAdornmentController : IDocumentAdornmentController, IHasLogger, IHasEnsureHelper
    {
        #region fields

        private readonly IPeekBroker _peekBroker;
        private readonly IDocumentWriter _documentWriter;
        private readonly IWpfTextView _view;
        private readonly IFixtureItemPeekResultContent _fixtureItemPeekResultContent;
        private readonly IAdornmentSessionFactory _adornmentSessionFactory;
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
        private bool _isDisposed;

        private Maybe<IAdornmentSession> _adornmentSession;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAdornmentController"/> class.
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="view"></param>
        /// <param name="peekBroker"></param>
        /// <param name="documentWriter"></param>
        /// <param name="fixtureItemPeekResultContent"></param>
        /// <param name="adornmentSessionFactory"></param>
        public DocumentAdornmentController(
            string projectName,
            IWpfTextView view,
            IPeekBroker peekBroker,
            IDocumentWriter documentWriter,
            IFixtureItemPeekResultContent fixtureItemPeekResultContent,
            IAdornmentSessionFactory adornmentSessionFactory)
        {
            this.EnsureParameter(projectName, nameof(projectName))
                .IsNotNullAndNotEmpty().ThrowOnFailure();

            this.EnsureMany()
                .Parameter(view, nameof(view))
                .Parameter(peekBroker, nameof(peekBroker))
                .Parameter(documentWriter, nameof(documentWriter))
                .Parameter(fixtureItemPeekResultContent, nameof(fixtureItemPeekResultContent))
                .Parameter(adornmentSessionFactory, nameof(adornmentSessionFactory))
                .ThrowWhenNull();

            this._peekBroker = peekBroker;
            this._documentWriter = documentWriter;
            this._view = view;
            this._fixtureItemPeekResultContent = fixtureItemPeekResultContent;
            this._adornmentSessionFactory = adornmentSessionFactory;

            view.Properties.AddProperty(typeof(IFixtureItemPeekResultContent), this._fixtureItemPeekResultContent);
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public void Dispose()
        {
            this._isDisposed = true;
            this._view.Properties.RemoveProperty(typeof(IFixtureItemPeekResultContent));
            this._adornmentSession.IfSome(session => session.Dispose());
            this._fixtureItemPeekResultContent.Dispose();
            this._semaphoreSlim.Dispose();
        }

        /// <inheritdoc />
        public async Task UpdateInformationAsync(IAdornmentInformation adornmentInformation)
        {
            await this._fixtureItemPeekResultContent.UpdateAsync(adornmentInformation);
        }

        /// <inheritdoc />
        public void FocusFixturePanel(ViEnterFocusPosition direction)
        {
            Keyboard.Focus(this._fixtureItemPeekResultContent.FixtureUserControl);
            this._fixtureItemPeekResultContent.ScrollViewer.Focus();
            this._fixtureItemPeekResultContent.MoveFocus(direction);
        }

        /// <inheritdoc />
        public async Task OpenAdornmentAsync(IViAdornment viAdornment, SnapshotSpan snapshotSpan)
        {
            if (this._isDisposed)
            {
                return;
            }

            try
            {
                await this._semaphoreSlim.WaitAsync(TimeSpan.FromSeconds(1));
                await this._adornmentSession.IfSomeAsync(session => session.CloseAsync());

                this._adornmentSession = await Maybe.SomeAsync(
                    this._adornmentSessionFactory.CreateAndStartAsync(
                        viAdornment,
                        this._view,
                        snapshotSpan,
                        this._fixtureItemPeekResultContent,
                        this._documentWriter,
                        this._peekBroker));
            }
            catch (Exception e)
            {
                this.Log(e);
            }
            finally
            {
                this._semaphoreSlim.Release();
            }
        }

        /// <inheritdoc />
        public async Task CloseAdornmentAsync(IViAdornment viAdornment)
        {
            if (this._isDisposed)
            {
                return;
            }

            try
            {
                await this._semaphoreSlim.WaitAsync(TimeSpan.FromSeconds(1));
                await this._adornmentSession.IfSomeAsync(session => session.CloseAsync());
                this._adornmentSession = Maybe.None();
            }
            catch (Exception e)
            {
                this.Log(e);
            }
            finally
            {
                this._semaphoreSlim.Release();
            }
        }

        #endregion
    }
}
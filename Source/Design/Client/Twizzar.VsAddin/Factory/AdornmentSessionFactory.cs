using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.Design.Ui.Interfaces.VisualStudio;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.VsAddin.Factory
{
    /// <inheritdoc cref="IAdornmentSessionFactory" />
    [ExcludeFromCodeCoverage]
    public class AdornmentSessionFactory : IAdornmentSessionFactory, IHasEnsureHelper, IHasLogger
    {
        #region fields

        private readonly Factory _factory;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdornmentSessionFactory"/> class.
        /// </summary>
        /// <param name="factory"></param>
        public AdornmentSessionFactory(Factory factory)
        {
            this._factory = factory;
        }

        #endregion

        #region delegates

        /// <summary>
        /// Autofac factory.
        /// </summary>
        /// <param name="viAdornment"></param>
        /// <returns>A new instance of <see cref="IAdornmentSession"/>.</returns>
        public delegate IAdornmentSession Factory(IViAdornment viAdornment);

        #endregion

        #region properties

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public async Task<IAdornmentSession> CreateAndStartAsync(
            IViAdornment viAdornment,
            ITextView textView,
            SnapshotSpan snapshotSpan,
            IFixtureItemPeekResultContent fixtureItemPeekResultContent,
            IDocumentWriter documentWriter,
            IPeekBroker peekBroker)
        {
            this.EnsureMany()
                .Parameter(viAdornment, nameof(viAdornment))
                .Parameter(textView, nameof(textView))
                .Parameter(fixtureItemPeekResultContent, nameof(fixtureItemPeekResultContent))
                .Parameter(documentWriter, nameof(documentWriter))
                .Parameter(peekBroker, nameof(peekBroker))
                .ThrowWhenNull();

            var adornmentSession = this._factory(viAdornment);

            await adornmentSession.StartAsync(
                textView,
                snapshotSpan,
                fixtureItemPeekResultContent,
                documentWriter,
                peekBroker);

            return adornmentSession;
        }

        #endregion
    }
}
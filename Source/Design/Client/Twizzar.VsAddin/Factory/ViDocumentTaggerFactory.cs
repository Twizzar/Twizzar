using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.VsAddin.Factory
{
    /// <inheritdoc cref="IViDocumentTaggerFactory" />
    [ExcludeFromCodeCoverage]
    public class ViDocumentTaggerFactory : IViDocumentTaggerFactory
    {
        #region fields

        private readonly Factory _factory;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViDocumentTaggerFactory"/> class.
        /// </summary>
        /// <param name="factory"></param>
        public ViDocumentTaggerFactory(Factory factory)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(factory, nameof(factory))
                .ThrowWhenNull();

            this._factory = factory;
        }

        #endregion

        #region delegates

        /// <summary>
        /// Factory for autofac.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="peekBroker"></param>
        /// <param name="documentFilePath"></param>
        /// <param name="projectName"></param>
        /// <returns>A new instance of <see cref="IViDocumentTagger"/>.</returns>
        public delegate IViDocumentTagger Factory(
            IWpfTextView view,
            IPeekBroker peekBroker,
            string documentFilePath,
            string projectName);

        #endregion

        #region members

        /// <inheritdoc />
        public IViDocumentTagger Create(
            IWpfTextView view,
            IPeekBroker peekBroker,
            string documentFilePath,
            string projectName) =>
            this._factory(
                view,
                peekBroker,
                documentFilePath,
                projectName);

        #endregion
    }
}
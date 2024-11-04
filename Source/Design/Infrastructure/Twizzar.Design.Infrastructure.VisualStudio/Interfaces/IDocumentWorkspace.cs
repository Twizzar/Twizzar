using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;

namespace Twizzar.Design.Infrastructure.VisualStudio.Interfaces
{
    /// <summary>
    /// Workspace for working with a document.
    /// </summary>
    public interface IDocumentWorkspace : IHasEnsureHelper, IHasLogger
    {
        /// <summary>
        /// Gets the document adornment controller.
        /// </summary>
        IDocumentAdornmentController DocumentAdornmentController { get; }

        /// <summary>
        /// Gets the document reader.
        /// </summary>
        IDocumentReader DocumentReader { get; }

        /// <summary>
        /// Gets the document writer.
        /// </summary>
        IDocumentWriter DocumentWriter { get; }

        /// <summary>
        /// Gets the adornment creator.
        /// </summary>
        IViAdornmentCache ViAdornmentCache { get; }

        /// <summary>
        /// Gets the snapshot history.
        /// </summary>
        ISnapshotHistory SnapshotHistory { get; }
    }
}
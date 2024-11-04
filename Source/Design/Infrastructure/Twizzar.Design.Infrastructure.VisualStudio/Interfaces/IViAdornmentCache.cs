using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Editor;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Ui.Interfaces.VisualStudio;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.Infrastructure.VisualStudio.Interfaces
{
    /// <summary>
    /// Caches the created viAdornments.
    /// </summary>
    public interface IViAdornmentCache : IService
    {
        #region members

        /// <summary>
        /// Create a the <see cref="IViAdornment"/>s.
        /// </summary>
        /// <param name="adornmentInformation">Sequence of adornment information.</param>
        /// <param name="textView">The text view.</param>
        /// <param name="documentAdornmentController"></param>
        /// <returns>A sequence of new created or cached IViAdornments.</returns>
        public IEnumerable<IViAdornment> GetOrCreate(
            IAdornmentInformation[] adornmentInformation,
            IWpfTextView textView,
            IDocumentAdornmentController documentAdornmentController);

        #endregion
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;

using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.Common.FixtureItem.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn.DocumentReader;
using Twizzar.Design.Shared.Infrastructure.Extension;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.Util;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <summary>
    /// Class to read <see cref="IAdornmentInformation"/> from a document in a project,
    /// using the roslyn code analysis api.
    /// </summary>
    public class RoslynDocumentReader : IDocumentReader, IService
    {
        #region fields

        private readonly string _projectName;
        private readonly string _filePath;
        private readonly IItemBuilderFinderFactory _itemBuilderFinderFactory;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynDocumentReader"/> class.
        /// </summary>
        /// <param name="filePath">The file path of the document to be analyzed.</param>
        /// <param name="projectName"></param>
        /// <param name="itemBuilderFinderFactory"></param>
        public RoslynDocumentReader(
            IItemBuilderFinderFactory itemBuilderFinderFactory,
            string filePath,
            string projectName)
        {
            this.EnsureMany()
                .Parameter(itemBuilderFinderFactory, nameof(itemBuilderFinderFactory))
                .Parameter(filePath, nameof(filePath))
                .Parameter(projectName, nameof(projectName))
                .ThrowWhenNull();

            this._filePath = filePath;
            this._projectName = projectName;
            this._itemBuilderFinderFactory = itemBuilderFinderFactory;
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
        public IEnumerable<IAdornmentInformation> GetAdornmentInformation(
            IRoslynContext context,
            CancellationToken cancellationToken = default)
        {
            using var methodOperation = ViMonitor.StartOperation(nameof(this.GetAdornmentInformation));

            cancellationToken.ThrowIfCancellationRequested();

            var finder = this._itemBuilderFinderFactory.Create(context.SemanticModel);

            foreach (var group in finder.FindBuilderInformation(context.RootNode)
                         .GroupBy(
                             information => information.ObjectCreationExpression.GetLocation()
                                 .GetLineSpan()
                                 .StartLinePosition.Line))
            {
                var g = group.ToArray();

                foreach (var (objectCreationExpression, fixtureItemType, isCustomBuilder) in g)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (fixtureItemType is IErrorTypeSymbol)
                    {
                        continue;
                    }

                    var span = objectCreationExpression.Span.ConvertToViSpan();
                    var typeSpan = objectCreationExpression.Type.Span.ConvertToViSpan();
                    var fixtureItemTypeFullName = fixtureItemType.GetTypeFullName();

                    var fixtureItemId = isCustomBuilder
                        ? FixtureItemId.CreateNamed(fixtureItemTypeFullName.FullName, fixtureItemTypeFullName)
                        : FixtureItemId.CreateNameless(fixtureItemTypeFullName);

                    fixtureItemId = fixtureItemId.WithRootItemPath(fixtureItemId.TypeFullName.FullName);

                    yield return new AdornmentInformation(
                        span,
                        typeSpan,
                        g.Length > 1,
                        fixtureItemId,
                        this._projectName,
                        this._filePath);
                }
            }
        }

        #endregion
    }
}
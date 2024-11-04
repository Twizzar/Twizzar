using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces.Roslyn;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.Util;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <inheritdoc cref="IUserConfigurationQuery" />
    public class RoslynConfigReader : IUserConfigurationQuery
    {
        #region fields

        private readonly IRoslynContextQuery _roslynContextQuery;
        private readonly IDocumentFileNameQuery _documentFileNameQuery;
        private readonly IBuildInvocationSpanQuery _buildInvocationSpanQuery;
        private readonly IRoslynConfigFinder _roslynConfigFinder;
        private readonly IRoslynConfigurationItemReader _roslynConfigurationItemReader;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynConfigReader"/> class.
        /// </summary>
        /// <param name="roslynContextQuery"></param>
        /// <param name="documentFileNameQuery"></param>
        /// <param name="buildInvocationSpanQuery"></param>
        /// <param name="roslynConfigFinder"></param>
        /// <param name="roslynConfigurationItemReader"></param>
        public RoslynConfigReader(
            IRoslynContextQuery roslynContextQuery,
            IDocumentFileNameQuery documentFileNameQuery,
            IBuildInvocationSpanQuery buildInvocationSpanQuery,
            IRoslynConfigFinder roslynConfigFinder,
            IRoslynConfigurationItemReader roslynConfigurationItemReader)
        {
            this.EnsureMany()
                .Parameter(roslynContextQuery, nameof(roslynContextQuery))
                .Parameter(documentFileNameQuery, nameof(documentFileNameQuery))
                .Parameter(buildInvocationSpanQuery, nameof(buildInvocationSpanQuery))
                .Parameter(roslynConfigurationItemReader, nameof(roslynConfigurationItemReader))
                .Parameter(roslynConfigFinder, nameof(roslynConfigFinder))
                .ThrowWhenNull();

            this._roslynContextQuery = roslynContextQuery;
            this._documentFileNameQuery = documentFileNameQuery;
            this._buildInvocationSpanQuery = buildInvocationSpanQuery;
            this._roslynConfigFinder = roslynConfigFinder;
            this._roslynConfigurationItemReader = roslynConfigurationItemReader;
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
        public Task<IEnumerable<IConfigurationItem>> GetAllAsync(Maybe<string> rootItemPath, CancellationToken cancellationToken)
        {
            using (ViMonitor.StartOperation($"{nameof(RoslynConfigReader)}{nameof(this.GetAllAsync)}"))
            {
                return this.ReadConfigAsync(rootItemPath, cancellationToken)
                    .MapAsync(items => items.Values)
                    .SomeOrProvidedAsync(Enumerable.Empty<IConfigurationItem>);
            }
        }

        private Task<Maybe<IImmutableDictionary<FixtureItemId, IConfigurationItem>>> ReadConfigAsync(
            Maybe<string> rootItemPath,
            CancellationToken cancellationToken) =>
                rootItemPath.MapAsync(s => this.ReadConfigSafeAsync(s, cancellationToken));

        private async Task<IImmutableDictionary<FixtureItemId, IConfigurationItem>> ReadConfigSafeAsync(
            string rootItemPath,
            CancellationToken cancellationToken)
        {
            var contextResult = await this._documentFileNameQuery.GetDocumentFileName(rootItemPath)
                .BindAsync(s => this._roslynContextQuery.GetContextAsync(s, cancellationToken));

            var spanResult = await this._buildInvocationSpanQuery.GetSpanAsync(rootItemPath);

            var parseTaskResult =
                from context in contextResult
                from span in spanResult
                select this._roslynConfigFinder.FindConfigClass(span, context)
                    .MatchAsync(
                        configInformation => this._roslynConfigurationItemReader.ReadConfigurationItemsAsync(configInformation, cancellationToken),
                        () => Task.FromResult<IImmutableDictionary<FixtureItemId, IConfigurationItem>>(
                            ImmutableDictionary.Create<FixtureItemId, IConfigurationItem>()));

            return await parseTaskResult
                .Match(f =>
                {
                    this.Log(f.Message);
                    throw new InternalException(f.Message);
                });
        }

        #endregion
    }
}
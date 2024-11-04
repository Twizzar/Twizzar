using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.CoreInterfaces.Query.Services;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Core.Query.Services
{
    /// <inheritdoc cref="IDocumentFileNameQuery" />
    public class DocumentFileNameQuery : FixtureItemConfigurationStartedQuery, IDocumentFileNameQuery
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentFileNameQuery"/> class.
        /// </summary>
        /// <param name="eventStore"></param>
        public DocumentFileNameQuery(IEventStore eventStore)
            : base(eventStore)
        {
        }

        #endregion

        #region members

        /// <inheritdoc />
        public Task<IResult<string, Failure>> GetDocumentFileName(Maybe<string> rootItemPath) =>
            this.GetFixtureItemConfigurationStartedEventAsync(rootItemPath)
                .MapSuccessAsync(e => e.DocumentFilePath);

        #endregion
    }
}
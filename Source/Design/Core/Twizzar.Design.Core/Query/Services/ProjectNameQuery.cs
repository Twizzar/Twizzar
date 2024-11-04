using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.CoreInterfaces.Query.Services;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Core.Query.Services
{
    /// <inheritdoc cref="IProjectNameQuery" />
    public class ProjectNameQuery : FixtureItemConfigurationStartedQuery, IProjectNameQuery
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectNameQuery"/> class.
        /// </summary>
        /// <param name="eventStore"></param>
        public ProjectNameQuery(IEventStore eventStore)
            : base(eventStore)
        {
        }

        #region members

        /// <inheritdoc />
        public Task<IResult<string, Failure>> GetProjectNameAsync(Maybe<string> rootItemPath) =>
            this.GetFixtureItemConfigurationStartedEventAsync(rootItemPath)
                .MapSuccessAsync(e => e.ProjectName);

        #endregion
    }
}
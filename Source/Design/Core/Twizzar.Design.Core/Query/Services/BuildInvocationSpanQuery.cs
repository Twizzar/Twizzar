using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.CoreInterfaces.Query.Services;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Core.Query.Services
{
    /// <inheritdoc cref="IBuildInvocationSpanQuery" />
    public class BuildInvocationSpanQuery : FixtureItemConfigurationStartedQuery, IBuildInvocationSpanQuery
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildInvocationSpanQuery"/> class.
        /// </summary>
        /// <param name="eventStore"></param>
        public BuildInvocationSpanQuery(IEventStore eventStore)
            : base(eventStore)
        {
        }

        #endregion

        #region members

        /// <inheritdoc />
        public Task<IResult<IViSpan, Failure>> GetSpanAsync(Maybe<string> rootItemPath) =>
            this.GetFixtureItemConfigurationStartedEventAsync(rootItemPath)
                .MapSuccessAsync(e => e.InvocationSpan);

        #endregion
    }
}
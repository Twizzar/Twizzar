using Twizzar.Design.CoreInterfaces.Adornment;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <summary>
    /// Service for fingins the config class.
    /// </summary>
    public interface IRoslynConfigFinder
    {
        /// <summary>
        /// Find the config class.
        /// </summary>
        /// <param name="span">The text span of the invocation <c>Build.New</c>.</param>
        /// <param name="context"></param>
        /// <returns>The <see cref="IBuilderInformation"/>.</returns>
        Maybe<IBuilderInformation> FindConfigClass(IViSpan span, IRoslynContext context);
    }
}
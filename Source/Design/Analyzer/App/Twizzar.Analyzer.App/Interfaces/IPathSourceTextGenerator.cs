using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Twizzar.Analyzer2022.App.SourceTextGenerators;
using Twizzar.Design.Shared.Infrastructure.PathTree;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Analyzer.Core.Interfaces
{
    /// <summary>
    /// Service which generates source code for a class.
    /// </summary>
    public interface IPathSourceTextGenerator
    {
        #region members

        /// <summary>
        /// Generate source code for a specific config class.
        /// </summary>
        /// <param name="className">The name of the class to generate.</param>
        /// <param name="nameSpace">The namespace to use for the generated class.</param>
        /// <param name="fixtureItemSymbol">The fixtureItem symbols this is the to be configured instance.</param>
        /// <param name="root">The root node of the path tree.</param>
        /// <param name="generateFuturePaths">When true also future paths for auto generation will be generated.</param>
        /// <param name="compilation"></param>
        /// <param name="sourceSymbol"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The generated source text.</returns>
        IResult<SourceText, PathGenerationFailure> GenerateSourceText(
            string className,
            string nameSpace,
            ITypeSymbol fixtureItemSymbol,
            IPathNode root,
            bool generateFuturePaths,
            Compilation compilation,
            ISymbol sourceSymbol,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
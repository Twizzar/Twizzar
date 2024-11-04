using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Twizzar.Analyzer.Core.SourceTextGenerators;
using Twizzar.Design.Shared.Infrastructure.PathTree;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Analyzer.Core.Interfaces
{
    /// <summary>
    /// Service for generating path members.
    /// </summary>
    public interface IPathSourceTextMemberGenerator
    {
        #region members

        /// <summary>
        /// Generate member for one level and recursively generates all sub level.
        /// </summary>
        /// <param name="typeDescription">The type description, this can be the fixture item type or a sub type of a member.</param>
        /// <param name="parent">The parent <see cref="PathNode"/>.</param>
        /// <param name="fixtureItemTypeName">The fixture item type name.</param>
        /// <param name="usingStatements">A set of all usings, theses will be later added to the source code.</param>
        /// <param name="reservedMembers">Set with members which are reserved by the config class. Method members should end with ().</param>
        /// <param name="generateFuturePaths">When true also future paths for auto generation will be generated.</param>
        /// <param name="compilation"></param>
        /// <param name="sourceType"></param>
        /// <param name="membersForVerification"></param>
        /// <param name="declaredType"></param>
        /// <param name="isRoot">True when this is the first level and the members need to be static.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A string which contains all generated members.</returns>
        string GenerateMembers(
            ITypeDescription typeDescription,
            Maybe<IPathNode> parent,
            string fixtureItemTypeName,
            HashSet<string> usingStatements,
            HashSet<string> reservedMembers,
            bool generateFuturePaths,
            Compilation compilation,
            ISymbol sourceType,
            in List<MemberVerificationInfo> membersForVerification,
            string declaredType,
            bool isRoot = true,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.Query.Services
{
    /// <summary>
    /// Interface to find all derived types and interfaces for the given type full name.
    /// </summary>
    public interface IAssignableTypesQuery : IQuery
    {
        #region members

        /// <summary>
        /// Gets the assignable types (Derived types + self) asynchronous.
        /// </summary>
        /// <param name="typeDescription">The type description.</param>
        /// <returns>A sequence of type full name.</returns>
        Task<IEnumerable<IBaseDescription>> GetAssignableTypesAsync(IBaseDescription typeDescription);

        /// <summary>
        /// Checks whether given typeDescription is assignable to baseDescription.
        /// </summary>
        /// <param name="baseDescription"></param>
        /// <param name="typeFullName"></param>
        /// <param name="rootItemPath"></param>
        /// <returns>None if not assignable else the resolved type matching the typeFullName.</returns>
        Task<Maybe<IBaseDescription>> IsAssignableTo(IBaseDescription baseDescription, ITypeFullName typeFullName, Maybe<string> rootItemPath);

        /// <summary>
        /// Initialize the query.
        /// </summary>
        /// <param name="compilationTypeQuery"></param>
        /// <returns></returns>
        Task InitializeAsync(ICompilationTypeQuery compilationTypeQuery);

        #endregion
    }
}
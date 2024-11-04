using System.Threading.Tasks;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.CoreInterfaces.Common.FixtureItem.Description.Services
{
    /// <summary>
    /// Query for getting a <see cref="ITypeDescription"/>.
    /// </summary>
    public interface ITypeDescriptionQuery : IQuery
    {
        /// <summary>
        /// Gets a <see cref="ITypeDescription"/> for a certain type.
        /// </summary>
        /// <param name="typeFullName">The type full name.</param>
        /// <param name="rootItemPath">The root fixture item path.</param>
        /// <returns>Success when the type description was created; else Failure.</returns>
        Task<IResult<ITypeDescription, Failure>> GetTypeDescriptionAsync(ITypeFullName typeFullName, Maybe<string> rootItemPath);
    }
}

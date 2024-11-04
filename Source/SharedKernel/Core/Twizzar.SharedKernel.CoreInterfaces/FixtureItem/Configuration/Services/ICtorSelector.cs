using Twizzar.SharedKernel.CoreInterfaces.Failures;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services
{
    /// <summary>
    /// Determines ctor from given type description and selectorDescription.
    /// </summary>
    public interface ICtorSelector : IService
    {
        /// <summary>
        /// Gets the MethodDescription for the selected ctor.
        /// </summary>
        /// <param name="typeDescription">The type description for which the ctor will be determined.</param>
        /// <param name="ctorSelectionBehavior">The ctor selection behavior.</param>
        /// <returns>The determined ctor as <see cref="IMethodDescription"/>.</returns>
        Result<IMethodDescription, InvalidTypeDescriptionFailure> GetCtorDescription(
            ITypeDescription typeDescription,
            CtorSelectionBehavior ctorSelectionBehavior);

        /// <summary>
        /// Find the matching constructor <see cref="IMethodDescription"/> to the <see cref="CtorMemberConfiguration"/>.
        /// </summary>
        /// <param name="config">The configuration item.</param>
        /// <param name="typeDescription">The type description.</param>
        /// <returns>Some <see cref="IMethodDescription"/> when configuration kind is <see cref="FixtureKind.Class"/> else None.</returns>
        Maybe<IMethodDescription> FindCtor(
            IConfigurationItem config,
            ITypeDescription typeDescription);
    }
}

using Twizzar.SharedKernel.CoreInterfaces.Failures;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services
{
    /// <summary>
    /// Service which knows the system default configurations.
    /// </summary>
    public interface ISystemDefaultService : IQuery
    {
        /// <summary>
        /// Gets a default configuration item. Returns a different <see cref="IConfigurationItem"/> based on the
        /// <see cref="FixtureKind"/>.
        /// </summary>
        /// <param name="typeDescription">The type description node.</param>
        /// <param name="rootFixturePath">The root fixture path.</param>
        /// <returns>A new instance of <see cref="IConfigurationItem"/>.</returns>
        Result<IConfigurationItem, InvalidTypeDescriptionFailure> GetDefaultConfigurationItem(
            ITypeDescription typeDescription,
            Maybe<string> rootFixturePath);

        /// <summary>
        /// Gets a default <see cref="IMemberConfiguration"/> for a base type.
        /// </summary>
        /// <param name="baseDescription"></param>
        /// <returns>A new instance of <see cref="IMemberConfiguration"/>.</returns>
        IMemberConfiguration GetBaseTypeMemberConfigurationItem(IBaseDescription baseDescription);

        /// <summary>
        /// Gets a default <see cref="IMemberConfiguration"/> for a type member.
        /// </summary>
        /// <param name="memberDescription">The member description.</param>
        /// <param name="rootFixturePath">The root fixture path.</param>
        /// <returns>A new instance of <see cref="IMemberConfiguration"/>.</returns>
        IMemberConfiguration GetDefaultMemberConfigurationItem(
            IMemberDescription memberDescription,
            Maybe<string> rootFixturePath);

        /// <summary>
        /// Get a default <see cref="IMemberConfiguration"/> for a constructor parameter.
        /// </summary>
        /// <param name="description">The parameter description.</param>
        /// <param name="rootFixturePath">The root fixture path.</param>
        /// <returns>A new instance of <see cref="IMemberConfiguration"/>.</returns>
        IMemberConfiguration GetDefaultConstructorParameterMemberConfigurationItem(
            IParameterDescription description,
            Maybe<string> rootFixturePath);
    }
}

using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.Failures;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.Services
{
    /// <summary>
    /// Factory for creating a <see cref="IFixtureItemDefinitionNode"/>.
    /// </summary>
    public interface IFixtureItemDefinitionNodeCreationService : IService
    {
        /// <summary>
        /// Creates a new instance of the <see cref="IBaseTypeNode"/> class.
        /// </summary>
        /// <param name="typeDescription">The type description node which describes this interface.</param>
        /// <param name="fixtureId">The fixture id of this definition.</param>
        /// <param name="configuration">A fully set Configuration item of the fixture id.</param>
        /// <returns>A new <see cref="IBaseTypeNode"/> or on failure a <see cref="InvalidConfigurationFailure"/>.</returns>
        IResult<IBaseTypeNode, InvalidConfigurationFailure> CreateBaseType(
            IRuntimeTypeDescription typeDescription,
            FixtureItemId fixtureId,
            IConfigurationItem configuration);

        /// <summary>
        /// Initializes a new instance of the <see cref="IClassNode"/> class.
        /// </summary>
        /// <param name="typeDescription">The type description node which describes this interface.</param>
        /// <param name="fixtureItemId">The fixture id of this definition.</param>
        /// <param name="configuration">A fully set Configuration item of the fixture id.</param>
        /// <returns>Returns <see cref="IClassNode"/> or on failure <see cref="InvalidConfigurationFailure"/>.</returns>
        IResult<IClassNode, InvalidConfigurationFailure> CreateClassNode(
            IRuntimeTypeDescription typeDescription,
            FixtureItemId fixtureItemId,
            IConfigurationItem configuration);

        /// <summary>
        /// Create a new instance of the <see cref="IMockNode"/> class.
        /// </summary>
        /// <param name="typeDescription">The type description node which describes this interface.</param>
        /// <param name="fixtureItemId">The fixture id of this definition.</param>
        /// <param name="configuration">A fully set Configuration item of the fixture id.</param>
        /// <returns>Returns a <see cref="IMockNode"/> or on failure a <see cref="InvalidConfigurationFailure"/>.</returns>
        IResult<IMockNode, InvalidConfigurationFailure> CreateInterfaceNode(
            IRuntimeTypeDescription typeDescription,
            FixtureItemId fixtureItemId,
            IConfigurationItem configuration);
    }
}

using System.Threading.Tasks;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.Services;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.Failures;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Runtime.Core.FixtureItem.Definition.Services
{
    /// <inheritdoc />
    public class FixtureItemDefinitionQuery : IFixtureItemDefinitionQuery
    {
        #region fields

        private readonly IConfigurationItemQuery _configurationItemQuery;
        private readonly ITypeDescriptionQuery _typeTypeDescriptionQuery;
        private readonly IFixtureItemDefinitionNodeCreationService _fixtureItemDefinitionCreationService;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemDefinitionQuery"/> class.
        /// </summary>
        /// <param name="configurationItemQuery">Query for a <see cref="IConfigurationItem"/>.</param>
        /// <param name="typeTypeDescriptionQuery">Query for a <see cref="ITypeDescription"/>.</param>
        /// <param name="fixtureItemDefinitionNodeCreationService">Factory for <see cref="IFixtureItemDefinitionNode"/>.</param>
        public FixtureItemDefinitionQuery(
            IConfigurationItemQuery configurationItemQuery,
            ITypeDescriptionQuery typeTypeDescriptionQuery,
            IFixtureItemDefinitionNodeCreationService fixtureItemDefinitionNodeCreationService)
        {
            this._configurationItemQuery =
                this.EnsureCtorParameterIsNotNull(configurationItemQuery, nameof(configurationItemQuery));

            this._typeTypeDescriptionQuery =
                this.EnsureCtorParameterIsNotNull(typeTypeDescriptionQuery, nameof(typeTypeDescriptionQuery));

            this._fixtureItemDefinitionCreationService = this.EnsureCtorParameterIsNotNull(
                fixtureItemDefinitionNodeCreationService,
                nameof(fixtureItemDefinitionNodeCreationService));
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public async Task<IFixtureItemDefinitionNode> GetDefinitionNode(FixtureItemId fixtureItemId)
        {
            this.EnsureParameter(fixtureItemId, nameof(fixtureItemId)).ThrowWhenNull();

            var descriptionNode = this._typeTypeDescriptionQuery.GetTypeDescription(fixtureItemId.TypeFullName);

            var configurationItem =
                await this._configurationItemQuery.GetConfigurationItem(fixtureItemId, descriptionNode);

            var node =
                this.CreateNode(fixtureItemId, descriptionNode, configurationItem);

            return node.Case switch
            {
                IFixtureItemDefinitionNode n => n,
                InvalidConfigurationFailure f =>
                    throw new InvalidConfigurationException(f.Message, f.ConfigurationItem),
                _ =>
                    throw new PatternErrorBuilder(nameof(node))
                        .IsNotOneOf(
                            nameof(IFixtureItemDefinitionNode),
                            nameof(InvalidConfigurationFailure)),
            };
        }

        private IResult<IFixtureItemDefinitionNode, InvalidConfigurationFailure> CreateNode(
            FixtureItemId id,
            IRuntimeTypeDescription description,
            IConfigurationItem configurationItem) =>
            description.DefaultFixtureKind switch
            {
                FixtureKind.Class =>
                    this._fixtureItemDefinitionCreationService.CreateClassNode(description, id, configurationItem),
                FixtureKind.BaseType =>
                    this._fixtureItemDefinitionCreationService.CreateBaseType(description, id, configurationItem),
                FixtureKind.Mock =>
                    this._fixtureItemDefinitionCreationService.CreateInterfaceNode(description, id, configurationItem),
                _ => throw new InvalidConfigurationException("Configuration type is not known", configurationItem),
            };

        #endregion
    }
}
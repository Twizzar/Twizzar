using System.Globalization;
using System.Threading.Tasks;

using Twizzar.Design.CoreInterfaces.Command.FixtureItem.Definition;
using Twizzar.Design.CoreInterfaces.Common.FixtureItem.Description.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.Resources;
using Twizzar.SharedKernel.NLog.Interfaces;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.ResultMonad;

using static ViCommon.Functional.Monads.ResultMonad.Result;

namespace Twizzar.Design.Core.Command.FixtureItem.Definition
{
    /// <inheritdoc />
    public class FixtureItemDefinitionQuery : IFixtureItemDefinitionQuery
    {
        private readonly ITypeDescriptionQuery _typeDescriptionQuery;
        private readonly IFixtureDefinitionNodeFactory _fixtureDefinitionNodeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemDefinitionQuery"/> class.
        /// </summary>
        /// <param name="typeDescriptionQuery">.</param>
        /// <param name="fixtureDefinitionNodeFactory">..</param>
        public FixtureItemDefinitionQuery(
            ITypeDescriptionQuery typeDescriptionQuery,
            IFixtureDefinitionNodeFactory fixtureDefinitionNodeFactory)
        {
            this._typeDescriptionQuery =
                this.EnsureCtorParameterIsNotNull(typeDescriptionQuery, nameof(typeDescriptionQuery));
            this._fixtureDefinitionNodeFactory =
                this.EnsureCtorParameterIsNotNull(fixtureDefinitionNodeFactory, nameof(fixtureDefinitionNodeFactory));
        }

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        /// <inheritdoc />
        public async Task<IResult<IFixtureItemDefinitionNode, Failure>> GetDefinitionNode(FixtureItemId id)
        {
            this.EnsureParameter(id, nameof(id)).ThrowWhenNull();

            var descriptionNode = await this._typeDescriptionQuery.GetTypeDescriptionAsync(id.TypeFullName, id.RootItemPath);
            return descriptionNode.Match(
                onSuccess: description =>
                    Success<IFixtureItemDefinitionNode, Failure>(this._fixtureDefinitionNodeFactory.Create(id, description)),
                onFailure: f =>
                    Failure<IFixtureItemDefinitionNode, Failure>(new Failure(
                        string.Format(CultureInfo.InvariantCulture, ErrorMessages.TypeDescrption_not_found + "\n" + f.Message, id.TypeFullName))));
        }
    }
}

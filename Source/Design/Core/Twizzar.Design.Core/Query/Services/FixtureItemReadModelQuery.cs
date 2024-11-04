using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Twizzar.Design.CoreInterfaces.Common.FixtureItem.Description.Services;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.CoreInterfaces.Query.Services.ReadModel;
using Twizzar.Design.CoreInterfaces.Resources;
using Twizzar.Design.Shared.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums;
using Twizzar.SharedKernel.CoreInterfaces.Resources;
using Twizzar.SharedKernel.NLog.Interfaces;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

using static ViCommon.Functional.Monads.ResultMonad.Result;

using PatternErrorBuilder = ViCommon.Functional.PatternErrorBuilder;

namespace Twizzar.Design.Core.Query.Services
{
    /// <inheritdoc />
    public class FixtureItemReadModelQuery : IFixtureItemReadModelQuery
    {
        #region fields

        private readonly ITypeDescriptionQuery _typeDescriptionQuery;
        private readonly IConfigurationItemQuery _configItemQuery;
        private readonly ICtorSelector _ctorSelector;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemReadModelQuery"/> class.
        /// </summary>
        /// <param name="typeDescriptionQuery">The type description query.</param>
        /// <param name="configItemQuery">The configuration item query.</param>
        /// <param name="ctorSelector">The ctor selector service.</param>
        public FixtureItemReadModelQuery(
            ITypeDescriptionQuery typeDescriptionQuery,
            IConfigurationItemQuery configItemQuery,
            ICtorSelector ctorSelector)
        {
            this.EnsureMany()
                .Parameter(typeDescriptionQuery, nameof(typeDescriptionQuery))
                .Parameter(configItemQuery, nameof(configItemQuery))
                .Parameter(ctorSelector, nameof(ctorSelector))
                .ThrowWhenNull();

            this._typeDescriptionQuery = typeDescriptionQuery;
            this._configItemQuery = configItemQuery;
            this._ctorSelector = ctorSelector;
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
        public async Task<IResult<IFixtureItemModel, Failure>> GetFixtureItem(FixtureItemId id)
        {
            this.EnsureParameter(id, nameof(id)).ThrowWhenNull();

            var descriptionResult =
                await this._typeDescriptionQuery.GetTypeDescriptionAsync(id.TypeFullName, id.RootItemPath);

            switch (descriptionResult.AsResultValue())
            {
                case FailureValue<Failure> failureValue:
                    return Failure<IFixtureItemModel, Failure>(
                        new Failure(failureValue.Value.Message));

                case SuccessValue<ITypeDescription> successValue:
                    var description = successValue.Value;
                    var config = await this._configItemQuery.GetConfigurationItem(id, description);

                    if (description.AccessModifier.IsPrivate || description.AccessModifier.IsProtected)
                    {
                        return Failure(new Failure(
                                string.Format(ErrorMessages._0__is_not_supported__Only_public_and_internal_types_are_supported, description.AccessModifier)))
                            .WithSuccess<IFixtureItemModel>();
                    }
                    if (description.IsBaseType && id.RootItemPath.SomeOrProvided(string.Empty) == id.TypeFullName.FullName)
                    {
                        return Failure(new Failure(MessagesDesign.Basetype_is_always_unique))
                            .WithSuccess<IFixtureItemModel>();
                    }

                    return description.IsBaseType switch
                    {
                        true =>
                            Success<IFixtureItemModel, Failure>(this.CreateBaseTypeModel(id, config, description)),
                        _ =>
                            Success<IFixtureItemModel, Failure>(this.CreateObjectModel(id, description, config)),
                    };
                default:
                    throw PatternErrorBuilder.PatternCase(nameof(descriptionResult.AsResultValue))
                        .IsNotOneOf(nameof(FailureValue<Failure>), nameof(SuccessValue<ITypeDescription>));
            }
        }

        private static ImmutableDictionary<string, FixtureItemMemberModel> CreateVariableMemberModels(
            IConfigurationItem config,
            IEnumerable<IMemberDescription> members)
        {
            var member = config.OnlyVariableMemberConfiguration;

            return members.Select(
                    definition =>
                        new FixtureItemMemberModel(member[definition.Name].WithName(definition.Name), definition))
                .ToImmutableDictionary(model => model.Description.Name, FunctionalCommon.Identity);
        }

        private IFixtureItemModel CreateBaseTypeModel(
            FixtureItemId id,
            IConfigurationItem config,
            ITypeDescription description)
        {
            this.EnsureParameter(config, nameof(config))
                .IsTrue(
                    item => item.MemberConfigurations.ContainsKey(ConfigurationConstants.BaseTypeMemberName),
                    pName => new InvalidConfigurationException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            MessagesDesign.FixtureItemReadModelQuery_CreateBaseTypeModel_Configuration_of_Kind__0__has_no_MemberConfiguration_with_the_key_name__1_,
                            "BaseType",
                            ConfigurationConstants.BaseTypeMemberName),
                        config))
                .ThrowOnFailure();

            return new BaseTypeFixtureItemModel(
                id,
                config.FixtureConfigurations,
                config.MemberConfigurations[ConfigurationConstants.BaseTypeMemberName],
                description);
        }

        private IFixtureItemModel CreateObjectModel(
            FixtureItemId id,
            ITypeDescription description,
            IConfigurationItem config) =>
            new ObjectFixtureItemModel(
                id,
                config.FixtureConfigurations,
                this.CreateConstructorModel(config, description),
                CreateVariableMemberModels(
                    config,
                    description.GetDeclaredProperties()
                        .OfType<IDesignPropertyDescription>()
                        .Where(p => description.IsInterface || p.CanWrite)),
                CreateVariableMemberModels(
                    config,
                    description.GetDeclaredFields()
                        .OfType<IDesignFieldDescription>()
                        .Where(f => !f.IsBackingField ||
                                    ((description.IsClass || description.IsStruct) &&
                                     !f.BackingFieldProperty.GetValueUnsafe().CanWrite))),
                CreateMethodMember(description, config),
                description.GetDeclaredConstructors(),
                description);

        /// <summary>
        /// Creates a <see cref="FixtureItemMemberModel"/> when:
        ///     The type description is an interface.
        ///     The Method is not a Constructor.
        ///     The Method does not return void.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="config"></param>
        /// <returns>An immutable dictionary.</returns>
        private static IImmutableDictionary<string, FixtureItemMemberModel> CreateMethodMember(
            ITypeDescription description,
            IConfigurationItem config) =>
                description.IsInterface
                    ? GetMethodMemberModel(config, description.GetDeclaredMethods())
                    : ImmutableDictionary.Create<string, FixtureItemMemberModel>();

        private static IImmutableDictionary<string, FixtureItemMemberModel> GetMethodMemberModel(
            IConfigurationItem config,
            IEnumerable<IMethodDescription> methodDescriptions) =>
        (
            from methodDescription in methodDescriptions
            where !methodDescription.IsConstructor
            where methodDescription.MethodKind == MethodKind.Ordinary
            where methodDescription.TypeFullName.FullName != typeof(void).FullName
            select new KeyValuePair<string, FixtureItemMemberModel>(
                methodDescription.UniqueName,
                new FixtureItemMemberModel(
                    config.OnlyVariableMemberConfiguration[methodDescription.UniqueName],
                    methodDescription)))
            .ToImmutableDictionary();

        private Maybe<FixtureItemConstructorModel> CreateConstructorModel(
            IConfigurationItem config,
            ITypeDescription typeDescription)
        {
            if (config.OnlyCtorParameterMemberConfigurations.Count <= 0)
            {
                return Maybe.None();
            }

            if (!config.MemberConfigurations.ContainsKey(ConfigurationConstants.CtorMemberName))
            {
                var exp = new InternalException(string.Format(
                    CultureInfo.InvariantCulture,
                    MessagesDesign.FixtureItemReadModelQuery_CreateConstructorModel_Configuration_members_does_not_have_a_key__0_,
                    ConfigurationConstants.CtorMemberName));

                throw this.LogAndReturn(exp);
            }

            if (!(config.MemberConfigurations[ConfigurationConstants.CtorMemberName] is CtorMemberConfiguration))
            {
                var exp = new InternalException(string.Format(
                    CultureInfo.InvariantCulture,
                    MessagesDesign.FixtureItemReadModelQuery_MemberConfigIsNotOfType,
                    ConfigurationConstants.CtorMemberName,
                    nameof(CtorMemberConfiguration)));

                throw this.LogAndReturn(exp);
            }

            var ctorMember =
                (CtorMemberConfiguration)config.MemberConfigurations[ConfigurationConstants.CtorMemberName];

            var methodDescription = this._ctorSelector.FindCtor(config, typeDescription);

            return methodDescription.Map(
                description =>
                    new FixtureItemConstructorModel(
                        description.DeclaredParameters
                            .Select(
                                parameterDescription =>
                                    new FixtureItemParameterModel(
                                        config.OnlyCtorParameterMemberConfigurations[parameterDescription.Name],
                                        parameterDescription))
                            .ToImmutableArray(),
                        description,
                        ctorMember));
        }

        #endregion
    }
}
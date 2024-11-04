using System.Collections.Immutable;
using System.Linq;

using Twizzar.SharedKernel.CoreInterfaces.Failures;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.FixtureConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.NLog.Interfaces;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.SharedKernel.Core.FixtureItem.Configuration.Services
{
    /// <summary>
    /// This service provide the system default configurations.
    /// </summary>
    public class SystemDefaultService : ISystemDefaultService
    {
        #region static fields and constants

        private static readonly IConfigurationSource Source = new FromSystemDefault();

        #endregion

        #region fields

        private readonly ICtorSelector _ctorSelector;
        private readonly IConfigurationItemFactory _configurationItemFactory;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemDefaultService"/> class.
        /// </summary>
        /// <param name="ctorSelector">The constructor selector.</param>
        /// <param name="configurationItemFactory">The configuration item factory.</param>
        public SystemDefaultService(
            ICtorSelector ctorSelector,
            IConfigurationItemFactory configurationItemFactory)
        {
            this._ctorSelector = this.EnsureCtorParameterIsNotNull(ctorSelector, nameof(ctorSelector));

            this._configurationItemFactory =
                this.EnsureCtorParameterIsNotNull(configurationItemFactory, nameof(configurationItemFactory));
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
        public Result<IConfigurationItem, InvalidTypeDescriptionFailure> GetDefaultConfigurationItem(
            ITypeDescription typeDescription,
            Maybe<string> rootFixturePath)
        {
            this.EnsureParameter(typeDescription, nameof(typeDescription)).ThrowWhenNull();

            var memberConfigurations = typeDescription.DefaultFixtureKind == FixtureKind.BaseType
                ? ImmutableDictionary<string, IMemberConfiguration>.Empty.Add(
                    ConfigurationConstants.BaseTypeMemberName,
                    this.GetBaseTypeMemberConfigurationItem(typeDescription))
                : this.GetMemberConfiguration(typeDescription, rootFixturePath);

            return Result.Success(
                this._configurationItemFactory.CreateConfigurationItem(
                    FixtureItemId.CreateNameless(typeDescription.TypeFullName).WithRootItemPath(rootFixturePath),
                    ImmutableDictionary<string, IFixtureConfiguration>.Empty,
                    memberConfigurations,
                    ImmutableDictionary.Create<string, IImmutableList<object>>()));
        }

        /// <inheritdoc />
        public IMemberConfiguration GetBaseTypeMemberConfigurationItem(IBaseDescription baseDescription)
        {
            var typeFullName = baseDescription.IsNullableBaseType
                ? baseDescription.TypeFullName.NullableGetUnderlyingType().GetValueUnsafe()
                : baseDescription.TypeFullName;

            var configName = baseDescription is IParameterDescription param
                ? param.Name
                : ConfigurationConstants.BaseTypeMemberName;

            if (!typeFullName.FullName.Equals(typeof(bool).FullName))
            {
                return new UniqueValueMemberConfiguration(configName, Source);
            }

            return new ValueMemberConfiguration(configName, true, Source);
        }

        /// <inheritdoc />
        public IMemberConfiguration GetDefaultMemberConfigurationItem(
            IMemberDescription memberDescription,
            Maybe<string> rootFixturePath) =>
            this.GetDefaultVariableMemberConfiguration(
                memberDescription.Name,
                memberDescription.TypeFullName);

        /// <inheritdoc/>
        public IMemberConfiguration GetDefaultConstructorParameterMemberConfigurationItem(
            IParameterDescription description,
            Maybe<string> rootFixturePath)
        {
            this.EnsureParameter(description, nameof(description)).ThrowWhenNull();

            return (description.IsBaseType || description.IsNullableBaseType) switch
            {
                true => this.GetBaseTypeMemberConfigurationItem(description),
                false => new LinkMemberConfiguration(
                    description.Name,
                    FixtureItemId.CreateNameless(description.TypeFullName)
                        .WithRootItemPath(rootFixturePath),
                    Source),
            };
        }

        private IImmutableDictionary<string, IMemberConfiguration> GetMemberConfiguration(
            ITypeDescription typeDescription,
            Maybe<string> rootFixturePath)
        {
            var properties =
                this.GenerateDefaultPropertyConfiguration(typeDescription.GetDeclaredProperties());

            var methods =
                this.GenerateDefaultMethodConfiguration(typeDescription.GetDeclaredMethods());

            var fields =
                this.GenerateDefaultFieldConfiguration(typeDescription.GetDeclaredFields());

            var memberConfigurations = properties
                .AddRange(methods)
                .AddRange(fields);

            var ctorMemberConfig =
                this.GenerateDefaultCtorConfiguration(typeDescription, rootFixturePath);

            if (ctorMemberConfig.AsResultValue() is SuccessValue<IMemberConfiguration> someCtorMemberConfig)
            {
                memberConfigurations = memberConfigurations.Add(
                    ConfigurationConstants.CtorMemberName,
                    someCtorMemberConfig.Value);
            }

            return memberConfigurations;
        }

        private IMemberConfiguration GetDefaultVariableMemberConfiguration(string name, ITypeFullName type)
        {
            this.EnsureMany<string>()
                .Parameter(name, nameof(name))
                .IsNotNullAndNotEmpty()
                .ThrowOnFailure();

            return new UndefinedMemberConfiguration(name, type, Source);
        }

        private ImmutableDictionary<string, IMemberConfiguration> GenerateDefaultPropertyConfiguration(
            ImmutableArray<IPropertyDescription> properties) =>
            properties
                .Select(
                    description => this.GetDefaultVariableMemberConfiguration(
                        description.Name,
                        description.TypeFullName))
                .ToImmutableDictionary(
                    configuration => configuration.Name,
                    configuration => configuration);

        /// <summary>
        /// Generate for each method a MethodConfiguration.
        /// </summary>
        /// <param name="methods"></param>
        /// <returns>A dictionary with the method name as key and a <see cref="MethodConfiguration"/> as value.</returns>
        private ImmutableDictionary<string, IMemberConfiguration> GenerateDefaultMethodConfiguration(
            ImmutableArray<IMethodDescription> methods) =>
            (
                from methodDescription in methods
                let name = methodDescription.UniqueName
                let type = methodDescription.TypeFullName
                where name != ConfigurationConstants.CtorMemberName
                select MethodConfiguration.Create(
                    methodDescription,
                    Source,
                    this.GetDefaultVariableMemberConfiguration(name, type)))
            .ToImmutableDictionary(
                configuration => configuration.Name,
                configuration => (IMemberConfiguration)configuration);

        private ImmutableDictionary<string, IMemberConfiguration> GenerateDefaultFieldConfiguration(
            ImmutableArray<IFieldDescription> fields) =>
            fields
                .Select(
                    description => this.GetDefaultVariableMemberConfiguration(
                        description.Name,
                        description.TypeFullName))
                .ToImmutableDictionary(
                    configuration => configuration.Name,
                    configuration => configuration);

        private Result<IMemberConfiguration, InvalidTypeDescriptionFailure>
            GenerateDefaultCtorConfiguration(
                ITypeDescription typeDescription,
                Maybe<string> rootFixturePath)
        {
            if (typeDescription.GetDeclaredConstructors().IsEmpty)
            {
                return new InvalidTypeDescriptionFailure(
                    typeDescription,
                    "DeclaredConstructors is empty");
            }

            var defaultCtor = typeDescription.IsArray || typeDescription.IsInheritedFromICollection
                ? this._ctorSelector.GetCtorDescription(typeDescription, CtorSelectionBehavior.Min)
                : this._ctorSelector.GetCtorDescription(typeDescription, CtorSelectionBehavior.Max);

            return
                (Result<IMemberConfiguration, InvalidTypeDescriptionFailure>)defaultCtor
                    .MapSuccess<IMemberConfiguration>(
                        ctor =>
                            (IMemberConfiguration)new CtorMemberConfiguration(
                                ctor.DeclaredParameters
                                    .Select(
                                        desc =>
                                            this.GetDefaultConstructorParameterMemberConfigurationItem(
                                                desc,
                                                rootFixturePath))
                                    .ToImmutableArray(),
                                ctor.DeclaredParameters
                                    .Select(p => p.TypeFullName)
                                    .ToImmutableArray(),
                                Source));
        }

        #endregion
    }
}
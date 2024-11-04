using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Twizzar.Runtime.Core.FixtureItem.Definition.ValueDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.MemberDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.Services;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.Runtime.CoreInterfaces.Resources;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.Failures;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;
using static ViCommon.Functional.Monads.ResultMonad.Result;

#pragma warning disable CA2208 // Instantiate argument exceptions correctly
#pragma warning disable S3928 // Parameter names used into ArgumentException constructors should match an existing one

namespace Twizzar.Runtime.Core.FixtureItem.Definition.Services
{
    /// <inheritdoc />
    public partial class FixtureItemDefinitionNodeCreationService : IFixtureItemDefinitionNodeCreationService
    {
        private readonly IFixtureDefinitionFactory _factory;
        private readonly ICtorSelector _ctorSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemDefinitionNodeCreationService"/> class.
        /// </summary>
        /// <param name="factory">The fixture definition factory.</param>
        /// <param name="ctorSelector">The ctor selector service.</param>
        public FixtureItemDefinitionNodeCreationService(IFixtureDefinitionFactory factory, ICtorSelector ctorSelector)
        {
            this.EnsureParameter(factory, nameof(factory)).ThrowWhenNull();
            this.EnsureParameter(ctorSelector, nameof(ctorSelector)).ThrowWhenNull();

            this._factory = factory;
            this._ctorSelector = ctorSelector;
        }

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region Implementation of IFixtureDefinitionNodeFactory

        /// <inheritdoc />
        public IResult<IBaseTypeNode, InvalidConfigurationFailure> CreateBaseType(
            IRuntimeTypeDescription typeDescription,
            FixtureItemId fixtureId,
            IConfigurationItem configuration)
        {
            this.EnsureMany()
                .Parameter(typeDescription, nameof(typeDescription))
                .Parameter(fixtureId, nameof(fixtureId))
                .Parameter(configuration, nameof(configuration))
                .ThrowWhenNull();

            this.EnsureParameter(typeDescription, nameof(typeDescription))
                .IsTrue(
                    description => description.IsBaseType || description.IsNullableBaseType,
                    "Cannot create a BaseTypeNode when the type is not BaseType")
                .ThrowOnFailure();

            if (configuration.MemberConfigurations.First().Value is NullValueMemberConfiguration && !typeDescription.Type.CanBeNull())
            {
                return Failure<IBaseTypeNode, InvalidConfigurationFailure>(
                    new InvalidConfigurationFailure(
                    configuration,
                    string.Format(ErrorMessagesRuntime.The_type__0__is_not_nullable_, typeDescription.TypeFullName.GetTypeName())));
            }

            return MapToValueDefinition(configuration)
                .MapSuccess(
                    definition => this._factory.CreateBaseTypeNode(
                        typeDescription,
                        fixtureId,
                        definition,
                        typeDescription.IsNullableBaseType));
        }

        /// <inheritdoc />
        public IResult<IClassNode, InvalidConfigurationFailure> CreateClassNode(
            IRuntimeTypeDescription typeDescription,
            FixtureItemId fixtureItemId,
            IConfigurationItem configuration)
        {
            this.EnsureMany()
                .Parameter(typeDescription, nameof(typeDescription))
                .Parameter(fixtureItemId, nameof(fixtureItemId))
                .Parameter(configuration, nameof(configuration))
                .ThrowWhenNull();

            var selectedConstructor = this._ctorSelector.FindCtor(configuration, typeDescription);

            return
                from properties in this.MapToPropertyDefinitions(
                    configuration,
                    typeDescription)
                from methods in this.MapToMethodDefinitions(
                    configuration,
                    typeDescription)
                from fields in this.MapToFieldDefinitions(
                    configuration,
                    typeDescription)
                from creatorType in GetCreatorType(typeDescription)
                from parameterDesc in
                    this.MapParameterDescriptions(configuration, selectedConstructor, typeDescription)
                select this._factory.CreateClassNode(
                    typeDescription,
                    fixtureItemId,
                    typeDescription.Type,
                    properties,
                    methods,
                    fields,
                    parameterDesc.ToImmutableArray(),
                    creatorType);
        }

        /// <inheritdoc />
        public IResult<IMockNode, InvalidConfigurationFailure> CreateInterfaceNode(
            IRuntimeTypeDescription typeDescription,
            FixtureItemId fixtureItemId,
            IConfigurationItem configuration)
        {
            this.EnsureMany()
                .Parameter(typeDescription, nameof(typeDescription))
                .Parameter(fixtureItemId, nameof(fixtureItemId))
                .Parameter(configuration, nameof(configuration))
                .ThrowWhenNull();

            return
                from properties in this.MapToPropertyDefinitions(
                    configuration,
                    typeDescription)
                from methods in this.MapToMethodDefinitions(
                        configuration,
                        typeDescription)
                from creatorType in GetCreatorType(typeDescription)
                select this._factory.CreateMockNode(
                    typeDescription,
                    fixtureItemId,
                    typeDescription.Type,
                    properties,
                    methods,
                    creatorType);
        }

        #endregion

        private static Result<CreatorType, InvalidConfigurationFailure> GetCreatorType(
            ITypeDescription typeDescription) =>
            typeDescription.DefaultFixtureKind switch
            {
                FixtureKind.Class => CreatorType.ConcreteType,
                FixtureKind.BaseType => CreatorType.BaseType,
                FixtureKind.Mock => CreatorType.Moq,
                _ => throw new ArgumentOutOfRangeException(nameof(typeDescription.DefaultFixtureKind)),
            };
    }

    /// <summary>
    /// Methods for value definition mapping.
    /// </summary>
    public partial class FixtureItemDefinitionNodeCreationService
    {
        private static Result<IValueDefinition, InvalidConfigurationFailure> MapToValueDefinition(
            IConfigurationItem configuration)
        {
            if (configuration.MemberConfigurations.Count != 1)
            {
                return new InvalidConfigurationFailure(
                    configuration,
                    "a BaseType should have 1 MemberConfiguration");
            }

            return configuration.MemberConfigurations.Values.First() switch
            {
                UniqueValueMemberConfiguration _ => new UniqueDefinition(),
                ValueMemberConfiguration x => new RawValueDefinition(x.Value),
                NullValueMemberConfiguration _ => new NullValueDefinition(),
                _ => new InvalidConfigurationFailure(
                    configuration,
                    $"Configuration is not of the type {nameof(UniqueValueMemberConfiguration)}, {nameof(ValueMemberConfiguration)} or {nameof(NullValueDefinition)}"),
            };
        }
    }

    /// <summary>
    /// Methods for property mapping.
    /// </summary>
    public partial class FixtureItemDefinitionNodeCreationService
    {
        private Result<ImmutableArray<IPropertyDefinition>, InvalidConfigurationFailure> MapToPropertyDefinitions(
            IConfigurationItem configuration,
            ITypeDescription typeDescription)
        {
            if (typeDescription.DefaultFixtureKind == FixtureKind.BaseType)
            {
                return new InvalidConfigurationFailure(
                    configuration,
                    $"The fixture type is not of the type {nameof(FixtureKind.Class)} or {nameof(FixtureKind.Mock)}");
            }
            else
            {
                return this.MapToPropertyDefinitions(
                    configuration.OnlyVariableMemberConfiguration,
                    typeDescription.GetDeclaredProperties().Select(description => (IRuntimePropertyDescription)description))
                    .ToImmutableArray();
            }
        }

        private IEnumerable<IPropertyDefinition> MapToPropertyDefinitions(
            IImmutableDictionary<string, IMemberConfiguration> configProperties,
            IEnumerable<IRuntimePropertyDescription> propertyDescriptions)
        {
            return propertyDescriptions
                .Select(propertyDescription =>
                    this._factory.CreatePropertyDefinition(
                        configProperties[propertyDescription.Name],
                        propertyDescription));
        }
    }

    /// <summary>
    /// Methods for method mapping.
    /// </summary>
    public partial class FixtureItemDefinitionNodeCreationService
    {
        private IResult<ImmutableArray<IMethodDefinition>, InvalidConfigurationFailure> MapToMethodDefinitions(
            IConfigurationItem configuration,
            ITypeDescription typeDescription)
        {
            if (typeDescription.DefaultFixtureKind == FixtureKind.BaseType)
            {
                return Failure<ImmutableArray<IMethodDefinition>, InvalidConfigurationFailure>(
                    new InvalidConfigurationFailure(
                        configuration,
                        $"The fixture type is not of the type {nameof(FixtureKind.Class)} or {nameof(FixtureKind.Mock)}"));
            }
            else
            {
                return this.MapToMethodDefinitions(
                        configuration,
                        configuration.OnlyVariableMemberConfiguration,
                        typeDescription
                            .GetDeclaredMethods()
                            .Select(description => (IRuntimeMethodDescription)description))
                    .MapSuccess(enumerable => enumerable.ToImmutableArray());
            }
        }

        private IResult<IEnumerable<IMethodDefinition>, InvalidConfigurationFailure> MapToMethodDefinitions(
            IConfigurationItem configuration,
            IImmutableDictionary<string, IMemberConfiguration> configProperties,
            IEnumerable<IRuntimeMethodDescription> methodDescriptions)
        {
            var result = new List<IMethodDefinition>();

            foreach (var methodDescription in methodDescriptions)
            {
                if (configProperties[methodDescription.UniqueName] is MethodConfiguration memberConfiguration)
                {
                    var callbacks = configuration.Callbacks.GetMaybe(methodDescription.UniqueName)
                        .SomeOrProvided(ImmutableList<object>.Empty);

                    result.Add(this._factory.CreateMethodDefinition(memberConfiguration, methodDescription, callbacks));
                }
                else
                {
                    return Failure<IEnumerable<IMethodDefinition>, InvalidConfigurationFailure>(
                        new InvalidConfigurationFailure(
                            configuration,
                            $"Configuration of a method need to be of type {nameof(MethodConfiguration)}"));
                }
            }

            return Success<IEnumerable<IMethodDefinition>, InvalidConfigurationFailure>(result);
        }
    }

    /// <summary>
    /// Methods for field mapping.
    /// </summary>
    public partial class FixtureItemDefinitionNodeCreationService
    {
        private Result<ImmutableArray<IFieldDefinition>, InvalidConfigurationFailure> MapToFieldDefinitions(
            IConfigurationItem configuration,
            ITypeDescription typeDescription)
        {
            if (typeDescription.DefaultFixtureKind is FixtureKind.BaseType or FixtureKind.Mock)
            {
                return new InvalidConfigurationFailure(
                    configuration,
                    $"The default Fixture Kind is not of the type {nameof(FixtureKind.Class)}");
            }
            else
            {
                return this.MapToFieldDefinitions(
                        configuration.OnlyVariableMemberConfiguration,
                        typeDescription.GetDeclaredFields().Select(description => (IRuntimeFieldDescription)description))
                    .ToImmutableArray();
            }
        }

        private IEnumerable<IFieldDefinition> MapToFieldDefinitions(
            IImmutableDictionary<string, IMemberConfiguration> configProperties,
            IEnumerable<IRuntimeFieldDescription> fieldDescriptions)
        {
            return fieldDescriptions
                .Select(fieldDescription =>
                    this._factory.CreateFieldDefinition(
                        configProperties[fieldDescription.Name],
                        fieldDescription));
        }
    }

    /// <summary>
    /// Methods for constructor parameter mapping.
    /// </summary>
    public partial class FixtureItemDefinitionNodeCreationService
    {
        private Result<IEnumerable<IParameterDefinition>, InvalidConfigurationFailure> MapParameterDescriptions(
            IConfigurationItem configuration,
            Maybe<IMethodDescription> selectedCtor,
            ITypeDescription typeDescription) =>
            typeDescription.DefaultFixtureKind switch
            {
                FixtureKind.Class =>
                    this.MapToParameterDefinition(configuration, selectedCtor),
                FixtureKind.Mock => Success(Enumerable.Empty<IParameterDefinition>()),
                _ => new InvalidConfigurationFailure(
                    configuration,
                    $"configuration is not of the kind {nameof(FixtureKind.Class)} or {nameof(FixtureKind.Mock)}"),
            };

        private Result<IEnumerable<IParameterDefinition>, InvalidConfigurationFailure> MapToParameterDefinition(
            IConfigurationItem config,
            Maybe<IMethodDescription> methodDescription) =>
            methodDescription.Match(
                some: description =>
                        this.MapToParameterDefinition(config, description).ExtractResult(),
                none:
                new InvalidConfigurationFailure(
                    config,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        ErrorMessagesRuntime.ClassNode_MapParameterDescriptions_ConcreteComponentShouldHaveSelectedCtor,
                        "ConcreteComponent")));

        private IEnumerable<Result<IParameterDefinition, InvalidConfigurationFailure>> MapToParameterDefinition(
                IConfigurationItem config,
                IMethodDescription methodDescription)
        {
            var ctorParams = config.OnlyCtorParameterMemberConfigurations;

            foreach (var parameter in methodDescription.DeclaredParameters)
            {
                if (!ctorParams.ContainsKey(parameter.Name))
                {
                    yield return new InvalidConfigurationFailure(
                        config,
                        $"Configuration {config} does not contain a constructor parameter with the name {parameter.Name}");
                }

                var memberConfig = ctorParams[parameter.Name];
                yield return Success(
                    this._factory.CreateParameterDefinition(
                        memberConfig,
                        (IRuntimeParameterDescription)parameter));
            }
        }
    }
}
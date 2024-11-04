using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Autofac;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.MemberDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.Services;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.Factories;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.CompositionRoot.Factory
{
    /// <summary>
    /// Factory for Fixture Definitions.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class FixtureDefinitionFactory : FactoryBase, IFixtureDefinitionFactory
    {
        private readonly BaseTypeFactory _baseTypeFactory;
        private readonly ClassFactory _classFactory;
        private readonly MockFactory _mockFactory;
        private readonly PropertyFactory _propertyFactory;
        private readonly FieldFactory _fieldFactory;
        private readonly ParameterFactory _parameterFactory;
        private readonly MethodFactory _methodFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureDefinitionFactory"/> class.
        /// </summary>
        /// <param name="componentContext">The autofac container.</param>
        /// <param name="baseTypeFactory">BaseType factory for autofac.</param>
        /// <param name="classFactory">Class factory for autofac.</param>
        /// <param name="mockFactory">Mock factory for autofac.</param>
        /// <param name="propertyFactory">Property factory for autofac.</param>
        /// <param name="fieldFactory">Field factory for autofac.</param>
        /// <param name="parameterFactory">Parameter factory for autofac.</param>
        /// <param name="methodFactory">Method factory for autofac.</param>
        public FixtureDefinitionFactory(
            IComponentContext componentContext,
            BaseTypeFactory baseTypeFactory,
            ClassFactory classFactory,
            MockFactory mockFactory,
            PropertyFactory propertyFactory,
            FieldFactory fieldFactory,
            ParameterFactory parameterFactory,
            MethodFactory methodFactory)
            : base(componentContext)
        {
            ViCommon.EnsureHelper.EnsureHelper.GetDefault.Many()
                .Parameter(baseTypeFactory, nameof(baseTypeFactory))
                .Parameter(classFactory, nameof(classFactory))
                .Parameter(mockFactory, nameof(mockFactory))
                .Parameter(propertyFactory, nameof(propertyFactory))
                .Parameter(fieldFactory, nameof(fieldFactory))
                .Parameter(parameterFactory, nameof(parameterFactory))
                .Parameter(methodFactory, nameof(methodFactory))
                .ThrowWhenNull();

            this._baseTypeFactory = baseTypeFactory;
            this._classFactory = classFactory;
            this._mockFactory = mockFactory;
            this._propertyFactory = propertyFactory;
            this._parameterFactory = parameterFactory;
            this._methodFactory = methodFactory;
            this._fieldFactory = fieldFactory;
        }

#pragma warning disable SA1600 // Elements should be documented
        public delegate IBaseTypeNode BaseTypeFactory(
            IRuntimeTypeDescription typeDescription,
            FixtureItemId fixtureId,
            IValueDefinition valueDefinition,
            bool isNullable);

        public delegate IClassNode ClassFactory(
            IRuntimeTypeDescription typeDescription,
            FixtureItemId fixtureItemId,
            Type type,
            ImmutableArray<IPropertyDefinition> properties,
            ImmutableArray<IMethodDefinition> methods,
            ImmutableArray<IFieldDefinition> fields,
            ImmutableArray<IParameterDefinition> constructorParameters,
            CreatorType creatorType);

        public delegate IMockNode MockFactory(
            IRuntimeTypeDescription typeDescription,
            FixtureItemId fixtureItemId,
            Type type,
            ImmutableArray<IPropertyDefinition> properties,
            ImmutableArray<IMethodDefinition> methods,
            CreatorType creatorType);

        public delegate IPropertyDefinition PropertyFactory(
            IMemberConfiguration configuration,
            IRuntimePropertyDescription propertyDescription);

        public delegate IMethodDefinition MethodFactory(
            MethodConfiguration methodConfiguration,
            IRuntimeMethodDescription methodDescription,
            IEnumerable<object> callbacks);

        public delegate IFieldDefinition FieldFactory(
            IMemberConfiguration configuration,
            IRuntimeFieldDescription fieldDescription);

        public delegate IParameterDefinition ParameterFactory(
            IMemberConfiguration memberConfiguration,
            IRuntimeParameterDescription parameterDescription);
#pragma warning restore SA1600 // Elements should be documented

        #region Implementation of IFixtureDefinitionFactory

        /// <inheritdoc />
        public IBaseTypeNode CreateBaseTypeNode(
            IRuntimeTypeDescription typeDescription,
            FixtureItemId id,
            IValueDefinition valueDefinition,
            bool isNullable) =>
                this._baseTypeFactory(typeDescription, id, valueDefinition, isNullable);

        /// <inheritdoc />
        public IClassNode CreateClassNode(
            IRuntimeTypeDescription typeDescription,
            FixtureItemId fixtureItemId,
            Type type,
            ImmutableArray<IPropertyDefinition> properties,
            ImmutableArray<IMethodDefinition> methods,
            ImmutableArray<IFieldDefinition> fields,
            ImmutableArray<IParameterDefinition> constructorParameters,
            CreatorType creatorType) =>
                this._classFactory(
                        typeDescription,
                        fixtureItemId,
                        type,
                        properties,
                        methods,
                        fields,
                        constructorParameters,
                        creatorType);

        /// <inheritdoc />
        public IMockNode CreateMockNode(
            IRuntimeTypeDescription typeDescription,
            FixtureItemId fixtureItemId,
            Type type,
            ImmutableArray<IPropertyDefinition> properties,
            ImmutableArray<IMethodDefinition> methods,
            CreatorType creatorType) =>
                this._mockFactory(
                        typeDescription,
                        fixtureItemId,
                        type,
                        properties,
                        methods,
                        creatorType);

        /// <inheritdoc />
        public IPropertyDefinition CreatePropertyDefinition(
            IMemberConfiguration configuration,
            IRuntimePropertyDescription propertyDescription) =>
                this._propertyFactory(
                        configuration,
                        propertyDescription);

        /// <inheritdoc />
        public IMethodDefinition CreateMethodDefinition(
            MethodConfiguration configuration,
            IRuntimeMethodDescription methodDescription,
            IEnumerable<object> callbacks) =>
                this._methodFactory(configuration, methodDescription, callbacks);

        /// <inheritdoc />
        public IFieldDefinition CreateFieldDefinition(
            IMemberConfiguration configuration,
            IRuntimeFieldDescription fieldDescription) =>
                this._fieldFactory(configuration, fieldDescription);

        /// <inheritdoc />
        public IParameterDefinition CreateParameterDefinition(
            IMemberConfiguration memberConfiguration,
            IRuntimeParameterDescription parameterDescription) =>
                this._parameterFactory(
                        memberConfiguration,
                        parameterDescription);

        #endregion
    }
}

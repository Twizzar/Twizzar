using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Twizzar.Runtime.Core.FixtureItem.Definition;
using Twizzar.Runtime.Core.FixtureItem.Definition.MemberDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.MemberDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.Services;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;

namespace Twizzar.Runtime.Core.Tests.Builder
{
    public class FixtureDefinitionFactoryBuilder
    {
        #region members

        public IFixtureDefinitionFactory Build() => new FixtureDefinitionFactoryMock();

        #endregion

        #region Nested type: FixtureDefinitionFactoryMock

        public class FixtureDefinitionFactoryMock : IFixtureDefinitionFactory
        {
            #region properties

            public ILogger Logger { get; set; }
            public IEnsureHelper EnsureHelper { get; set; }

            #endregion

            #region members

            public IBaseTypeNode CreateBaseTypeNode(
                IRuntimeTypeDescription typeDescription,
                FixtureItemId id,
                IValueDefinition valueDefinition,
                bool isNullable) =>
                new BaseTypeNode(typeDescription, id, valueDefinition, isNullable);

            public IClassNode CreateClassNode(
                IRuntimeTypeDescription typeDescription,
                FixtureItemId id,
                Type type,
                ImmutableArray<IPropertyDefinition> properties,
                ImmutableArray<IMethodDefinition> methods,
                ImmutableArray<IFieldDefinition> fields,
                ImmutableArray<IParameterDefinition> constructorParameters,
                CreatorType creatorType) =>
                new ClassNode(typeDescription,
                    id,
                    type,
                    properties,
                    methods,
                    fields,
                    constructorParameters,
                    creatorType);

            public IFieldDefinition CreateFieldDefinition(
                IMemberConfiguration configuration,
                IRuntimeFieldDescription fieldDescription) =>
                new FieldDefinition(configuration, fieldDescription);

            public IMethodDefinition CreateMethodDefinition(
                MethodConfiguration configuration,
                IRuntimeMethodDescription methodDescription,
                IEnumerable<object> callbacks) =>
                new MethodDefinition(configuration, methodDescription, callbacks);

            public IMockNode CreateMockNode(
                IRuntimeTypeDescription typeDescription,
                FixtureItemId fixtureItemId,
                Type type,
                ImmutableArray<IPropertyDefinition> properties,
                ImmutableArray<IMethodDefinition> methods,
                CreatorType creatorType) =>
                new MockNode(typeDescription, fixtureItemId, type, properties, methods, creatorType);

            public IParameterDefinition CreateParameterDefinition(
                IMemberConfiguration memberConfiguration,
                IRuntimeParameterDescription parameterDescription) =>
                new ParameterDefinition(memberConfiguration, parameterDescription);

            public IPropertyDefinition CreatePropertyDefinition(
                IMemberConfiguration configuration,
                IRuntimePropertyDescription propertyDescription) =>
                new PropertyDefinition(configuration, propertyDescription);

            #endregion
        }

        #endregion
    }
}
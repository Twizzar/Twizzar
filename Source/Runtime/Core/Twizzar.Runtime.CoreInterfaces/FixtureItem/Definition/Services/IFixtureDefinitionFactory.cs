using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.MemberDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;

#pragma warning disable S107 // Methods should not have too many parameters

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.Services
{
    /// <summary>
    /// Factory for Fixture Definitions.
    /// </summary>
    public interface IFixtureDefinitionFactory : IFactory
    {
        /// <summary>
        /// Create a <see cref="IBaseTypeNode"/>.
        /// </summary>
        /// <param name="typeDescription">The type description.</param>
        /// <param name="id">The fixture item id.</param>
        /// <param name="valueDefinition">The value definition.</param>
        /// <param name="isNullable">Is the baste type nullable.</param>
        /// <returns>A base typ node.</returns>
        public IBaseTypeNode CreateBaseTypeNode(
            IRuntimeTypeDescription typeDescription,
            FixtureItemId id,
            IValueDefinition valueDefinition,
            bool isNullable);

        /// <summary>
        /// Create a <see cref="IClassNode"/>.
        /// </summary>
        /// <param name="typeDescription">The type description.</param>
        /// <param name="fixtureItemId">The fixture item id.</param>
        /// <param name="type">The type of the class.</param>
        /// <param name="properties">The properties.</param>
        /// <param name="methods">The methods.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="constructorParameters">The constructor parameters.</param>
        /// <param name="creatorType">Creator type Mock or Concrete Class.</param>
        /// <returns>A class node.</returns>
        public IClassNode CreateClassNode(
            IRuntimeTypeDescription typeDescription,
            FixtureItemId fixtureItemId,
            Type type,
            ImmutableArray<IPropertyDefinition> properties,
            ImmutableArray<IMethodDefinition> methods,
            ImmutableArray<IFieldDefinition> fields,
            ImmutableArray<IParameterDefinition> constructorParameters,
            CreatorType creatorType);

        /// <summary>
        /// Create a <see cref="IMockNode"/>.
        /// </summary>
        /// <param name="typeDescription">The type description.</param>
        /// <param name="fixtureItemId">The fixture item id.</param>
        /// <param name="type">The type of the class or interface.</param>
        /// <param name="properties">The properties.</param>
        /// <param name="methods">The methods.</param>
        /// <param name="creatorType">Creator type Mock or Concrete Class.</param>
        /// <returns>A mock node.</returns>
        public IMockNode CreateMockNode(
            IRuntimeTypeDescription typeDescription,
            FixtureItemId fixtureItemId,
            Type type,
            ImmutableArray<IPropertyDefinition> properties,
            ImmutableArray<IMethodDefinition> methods,
            CreatorType creatorType);

        /// <summary>
        /// Create a <see cref="IPropertyDefinition"/>.
        /// </summary>
        /// <param name="configuration">The member configuration.</param>
        /// <param name="propertyDescription">The property description.</param>
        /// <returns>A property definition.</returns>
        public IPropertyDefinition CreatePropertyDefinition(
            IMemberConfiguration configuration,
            IRuntimePropertyDescription propertyDescription);

        /// <summary>
        /// Create a <see cref="IMethodDefinition"/>.
        /// </summary>
        /// <param name="configuration">The member configuration.</param>
        /// <param name="methodDescription">The method description.</param>
        /// <param name="callbacks"></param>
        /// <returns>A method definition.</returns>
        public IMethodDefinition CreateMethodDefinition(
            MethodConfiguration configuration,
            IRuntimeMethodDescription methodDescription,
            IEnumerable<object> callbacks);

        /// <summary>
        /// Create a <see cref="IFieldDefinition"/>.
        /// </summary>
        /// <param name="configuration">The member configuration.</param>
        /// <param name="fieldDescription">The property description.</param>
        /// <returns>A field definition.</returns>
        public IFieldDefinition CreateFieldDefinition(
            IMemberConfiguration configuration,
            IRuntimeFieldDescription fieldDescription);

        /// <summary>
        /// Create a <see cref="IParameterDefinition"/>.
        /// </summary>
        /// <param name="memberConfiguration">The member configuration.</param>
        /// <param name="parameterDescription">The parameter description.</param>
        /// <returns>A parameter definition.</returns>
        public IParameterDefinition CreateParameterDefinition(
            IMemberConfiguration memberConfiguration,
            IRuntimeParameterDescription parameterDescription);
    }
}

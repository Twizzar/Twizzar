using System;
using System.Collections.Immutable;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.MemberDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

// ReSharper disable ArrangeModifiersOrder

namespace Twizzar.Runtime.Core.FixtureItem.Definition
{
    /// <summary>
    /// Class definition node contains all information to create an instance of a class.
    /// </summary>
    public sealed class ClassNode : MockNode, IClassNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassNode"/> class.
        /// </summary>
        /// <param name="typeDescription">The type description node which describes this interface.</param>
        /// <param name="fixtureItemId">The fixture id of this definition.</param>
        /// <param name="type">The type of the class.</param>
        /// <param name="properties">Property configurations.</param>
        /// <param name="methods">Methods configuratuins.</param>
        /// <param name="fields">Field configurations.</param>
        /// <param name="constructorParameters">Constructor configurations.</param>
        /// <param name="creatorType">Creator type Mock or Concrete Class.</param>
        public ClassNode(
            IRuntimeTypeDescription typeDescription,
            FixtureItemId fixtureItemId,
            Type type,
            ImmutableArray<IPropertyDefinition> properties,
            ImmutableArray<IMethodDefinition> methods,
            ImmutableArray<IFieldDefinition> fields,
            ImmutableArray<IParameterDefinition> constructorParameters,
            CreatorType creatorType)
        : base(typeDescription, fixtureItemId, type, properties, methods, creatorType)
        {
            this.EnsureMany()
                .Parameter(constructorParameters, nameof(constructorParameters))
                .Parameter(fields, nameof(fields))
                .ThrowWhenNull();

            this.ConstructorParameters = constructorParameters;
            this.Fields = fields;
        }

        #region Properties

        /// <summary>
        /// Gets the parameters of the constructor.
        /// </summary>
        public ImmutableArray<IParameterDefinition> ConstructorParameters { get; }

        /// <inheritdoc />
        public ImmutableArray<IFieldDefinition> Fields { get; }

        #endregion
    }
}
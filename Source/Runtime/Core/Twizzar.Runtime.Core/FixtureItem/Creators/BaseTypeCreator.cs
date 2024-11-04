using System;
using System.Linq;
using Twizzar.Runtime.CoreInterfaces.Exceptions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Services;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Runtime.Core.FixtureItem.Creators
{
    /// <summary>
    /// Implements interface <see cref="IBaseTypeCreator"/>.
    /// </summary>
    public class BaseTypeCreator : IBaseTypeCreator
    {
        #region fields

        private readonly IBaseTypeUniqueCreator _baseTypeUniqueCreator;
        private readonly IInstanceCacheRegistrant _instanceCacheRegistrant;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTypeCreator"/> class.
        /// </summary>
        /// <param name="baseTypeUniqueCreator">The base type unique generator.</param>
        /// <param name="instanceCacheRegistrant">The instanceCacheRegistrant service.</param>
        public BaseTypeCreator(
            IBaseTypeUniqueCreator baseTypeUniqueCreator,
            IInstanceCacheRegistrant instanceCacheRegistrant)
        {
            this._baseTypeUniqueCreator = baseTypeUniqueCreator
                ?? throw new ArgumentNullException(nameof(baseTypeUniqueCreator));

            this._instanceCacheRegistrant = instanceCacheRegistrant
                ?? throw new ArgumentNullException(nameof(instanceCacheRegistrant));
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
        public object CreateInstance(IFixtureItemDefinitionNode definition)
        {
            this.EnsureParameter(definition, nameof(definition)).ThrowWhenNull();

            if (definition is IBaseTypeNode baseTypeNode)
            {
                return this.CreateInstance(baseTypeNode);
            }
            else
            {
                throw new ResolveTypeException(
                    $"{nameof(BaseTypeCreator)} can only create instance form {nameof(IBaseTypeNode)}");
            }
        }

        /// <inheritdoc />
        public object CreateInstance(IValueDefinition valueDefinition, IBaseDescription description) =>
            valueDefinition switch
            {
                IRawValueDefinition rawValueDefinition =>
                    rawValueDefinition.Value,

                IUniqueDefinition _ =>
                    this._baseTypeUniqueCreator.GetNextValue(
                        UnpackNullableType(description.GetReturnTypeDescription() as IRuntimeTypeDescription)),

                INullValueDefinition _ =>
                    new NullValue(),

                _ => throw new PatternErrorBuilder(nameof(valueDefinition))
                    .IsNotOneOf(
                        nameof(IRawValueDefinition),
                        nameof(IUniqueDefinition),
                        nameof(INullValueDefinition)),
            };

        private static Type UnpackNullableType(IRuntimeTypeDescription typeDescription) =>
            typeDescription.IsNullableBaseType
                ? typeDescription.GenericRuntimeTypeArguments.First().Value
                : typeDescription.Type;

        private object CreateInstance(IBaseTypeNode baseTypeNode)
        {
            var type = baseTypeNode.TypeDescription.Type;

            var value = this.CreateInstance(
                baseTypeNode.ValueDefinition,
                baseTypeNode.TypeDescription);

            if (!baseTypeNode.IsNullable)
            {
                return value;
            }

            // create nullable base type.
            var nullableValue = Activator.CreateInstance(type, value);

            baseTypeNode.FixtureItemId.Name.IfSome(
                name =>
                    this._instanceCacheRegistrant.Register(name, value));

            return nullableValue;
        }

        #endregion
    }
}
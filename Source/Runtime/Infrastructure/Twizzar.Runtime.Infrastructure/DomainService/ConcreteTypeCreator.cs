using System;
using System.Linq;
using System.Reflection;
using Autofac.Builder;
using Autofac.Core;
using Twizzar.Runtime.CoreInterfaces.Exceptions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Services;
using Twizzar.Runtime.Infrastructure.AutofacServices.Creator;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Runtime.Infrastructure.DomainService
{
    /// <summary>
    /// Implements interface <see cref="IConcreteTypeCreator"/>.
    /// </summary>
    public class ConcreteTypeCreator : AutofacCreator, IConcreteTypeCreator
    {
        private const BindingFlags BindingFlags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcreteTypeCreator"/> class.
        /// </summary>
        /// <param name="instanceCacheRegistrant">The instanceCacheRegistrant service.</param>
        /// <param name="baseTypeCreator">The baseTypeCreator service.</param>
        public ConcreteTypeCreator(
            IInstanceCacheRegistrant instanceCacheRegistrant,
            IBaseTypeCreator baseTypeCreator)
            : base(instanceCacheRegistrant, baseTypeCreator)
        {
        }

        #endregion

        #region members

        /// <inheritdoc />
        public override object CreateInstance(IFixtureItemDefinitionNode definition)
        {
            this.EnsureParameter(definition, nameof(definition)).ThrowWhenNull();

            if (definition is not IClassNode classNode)
            {
                throw new ResolveTypeException($"{nameof(ConcreteTypeCreator)} cannot create instance from another type than ClassNode at {definition?.FixtureItemId?.ToPathString()}.");
            }

            if (this.AutofacContext.IsNone || this.AutofacParameters.IsNone)
            {
                throw new ResolveTypeException(
                    $"the {nameof(this.Update)} method needs to be called before calling {nameof(this.CreateInstance)}");
            }

            var activator = this.GetActivator(classNode);

            var instance = activator.ActivateInstance(
                this.AutofacContext.GetValueUnsafe(),
                this.AutofacParameters.GetValueUnsafe());

            instance = this.SetPropertiesWithReflection(classNode, instance);
            instance = this.SetFieldsWithReflection(classNode, instance);
            this.RegisterInstance(instance, definition.FixtureItemId);
            return instance;
        }

        private IInstanceActivator GetActivator(IClassNode classNode) =>
            RegistrationBuilder.ForDelegate(
                (context, parameters) =>
                    this.CallCtor(classNode)).ActivatorData.Activator;

        private object CallCtor(IClassNode classNode)
        {
            // if default static constructor we cannot find it with reflection.
            if (classNode.TypeDescription.IsStruct && classNode.ConstructorParameters.Length == 0)
            {
                return Activator.CreateInstance(classNode.TypeDescription.Type);
            }

            if (classNode.TypeDescription.IsArray && classNode.ConstructorParameters.Length == 0)
            {
                // use array element type with length (first dimension as 1s)
                // so int[,,][] can be activated with .CreateInstance(typeof(int[]), int[]{1,1,1}
                const int arrayDimLength = 0;

                return Array.CreateInstance(
                    classNode.TypeDescription.Type.GetElementType(),
                    Enumerable.Repeat(arrayDimLength, classNode.TypeDescription.ArrayDimension.Last()).ToArray());
            }

            var paramTypes = classNode.ConstructorParameters
                .Select(definition =>
                    definition.ParameterDescription.Type)
                .ToArray();

            var ctorParams = classNode.ConstructorParameters
                .Select(ctorParam =>
                    this.ResolveType(
                        AlterValueDef(ctorParam.Name, ctorParam.ValueDefinition, classNode.FixtureItemId),
                        ctorParam.ParameterDescription,
                        classNode.FixtureItemId))
                .ToArray();

            var ctorInfo = classNode.TypeDescription.Type.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                null,
                paramTypes,
                null);

            if (ctorInfo == null)
            {
                throw this.LogAndReturn(
                        new ResolveTypeException($"Cannot find constructor for type {classNode.TypeDescription.TypeFullName} with parameter types: " +
                                                 string.Join(";", paramTypes.Select(type => type.FullName)) +
                                                 $" at {classNode.FixtureItemId.ToPathString()}"));
            }

            return ctorInfo.Invoke(ctorParams);
        }

        /// <summary>
        /// Enrich the value definition with a id name when it is a LinkDefinition.
        /// To ensure the member is saved in the <see cref="IInstanceCacheRegistrant"/>.
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="valueDefinition"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private static IValueDefinition AlterValueDef(string memberName, IValueDefinition valueDefinition, FixtureItemId parentId)
        {
            return valueDefinition switch
            {
                ILinkDefinition { Link.Name: { IsNone: true } } link =>
                    link.WithLink(link.Link.WithName(parentId.Name.Map(s => $"{s}.{memberName}"))),
                _ => valueDefinition,
            };
        }

        private object SetFieldsWithReflection(IClassNode classNode, object instance)
        {
            foreach (var fieldDefinition in classNode.Fields
                .Where(fieldDefinition => fieldDefinition.ValueDefinition is not IUndefinedDefinition))
            {
                var fieldInfo = classNode.TypeDescription.Type
                    .GetField(
                        fieldDefinition.Name, BindingFlags);

                if (fieldInfo != null)
                {
                    fieldInfo.SetValue(
                        instance,
                        this.ResolveType(
                            fieldDefinition.ValueDefinition,
                            fieldDefinition.FieldDescription,
                            classNode.FixtureItemId));
                }
                else
                {
                    this.Logger?.Log(
                        LogLevel.Error,
                        $"Cannot find the field {fieldDefinition.Name} in the class {classNode.TypeDescription.TypeFullName.FullName} at {classNode.FixtureItemId.ToPathString()}");
                }
            }

            return instance;
        }

        private object SetPropertiesWithReflection(IClassNode classNode, object instance)
        {
            foreach (var propDefinition in classNode.Properties
                .Where(propDefinition => propDefinition.ValueDefinition is not IUndefinedDefinition))
            {
                if (propDefinition.ValueDefinition is IDelegateValueDefinition)
                {
                    throw new ResolveTypeDescriptionException(
                        $"Delegates can only be used on interfaces. Type {classNode.FixtureItemId.TypeFullName.FullName} is not an interface.");
                }

                var propInfo = classNode.TypeDescription.Type
                    .GetProperty(propDefinition.Name, BindingFlags);

                if (propInfo != null && propInfo.CanWrite)
                {
                    propInfo.SetValue(
                        instance,
                        this.ResolveType(
                            propDefinition.ValueDefinition,
                            propDefinition.PropertyDescription,
                            classNode.FixtureItemId));
                }
                else
                {
                    this.Logger?.Log(
                        LogLevel.Error,
                        $"Cannot find the property setter of the property {propDefinition.Name} in the class {classNode.TypeDescription.TypeFullName.FullName} either the property does not exists or has no setter at {classNode.FixtureItemId.ToPathString()}.{propDefinition.Name}.");
                }
            }

            return instance;
        }

        #endregion
    }
}
using System.Linq;
using System.Reflection;

using Moq;
using Twizzar.Runtime.CoreInterfaces.Exceptions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Services;
using Twizzar.Runtime.CoreInterfaces.Helpers;
using Twizzar.Runtime.Infrastructure.AutofacServices.Creator;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.Infrastructure.DomainService
{
    /// <summary>
    /// Implements interface <see cref="IMoqCreator"/>.
    /// </summary>
    public class MoqCreator : AutofacCreator, IMoqCreator
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="MoqCreator"/> class.
        /// </summary>
        /// <param name="instanceCacheRegistrant">instanceCacheRegistrant service.</param>
        /// <param name="baseTypeCreator">baseTypeCreator service.</param>
        public MoqCreator(
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

            if (definition is IMockNode interfaceNode)
            {
                var mock = this.CreateMock(interfaceNode).Object;
                this.RegisterInstance(mock, definition.FixtureItemId);
                return mock;
            }

            throw new ResolveTypeException(
                $"{nameof(MoqCreator)} can only create instance form {nameof(IMockNode)} at {definition?.FixtureItemId?.ToPathString()}");
        }

        private Mock CreateMock(IMockNode mockNode) =>
            ReflectionGenericMethodBuilder.Create(this.CreateMock<object>)
                .WithInvocationObject(this)
                .WithGenericTypes(mockNode.TypeDescription.Type)
                .WithParameters(mockNode)
                .Invoke<Mock>();

        private Mock<T> CreateMock<T>(IMockNode mockNode)
            where T : class
        {
            var stubBuilder = new StubBuilder<T>();

            this.SetupProperties(mockNode, stubBuilder);
            this.SetupMethods(mockNode, stubBuilder);

            return stubBuilder.Build();
        }

        private void SetupProperties<T>(IMockNode mockNode, StubBuilder<T> stubBuilder)
            where T : class
        {
            foreach (var property in mockNode.Properties.Where(property => property.ValueDefinition is not IUndefinedDefinition))
            {
                if (StubBuilder<T>.FindPropertyInfo(property.PropertyDescription).AsMaybeValue() is not SomeValue<PropertyInfo> somePropertyInfo)
                {
                    throw this.LogAndReturn(new ResolveTypeException(
                        $"Cannot find the property info of the property {property.Name} at path {mockNode.FixtureItemId.ToPathString()}"));
                }

                if (property.ValueDefinition is IDelegateValueDefinition delegateValueDefinition)
                {
                    stubBuilder.SetupPropertyValue(somePropertyInfo.Value, delegateValueDefinition.ValueDelegate);
                }
                else
                {
                    var value = this.ResolveType(
                        property.ValueDefinition,
                        property.PropertyDescription,
                        mockNode.FixtureItemId);

                    stubBuilder.SetupPropertyValue(somePropertyInfo.Value, value);
                }
            }
        }

        private void SetupMethods<T>(IMockNode mockNode, StubBuilder<T> stubBuilder)
            where T : class
        {
            foreach (var method in mockNode.Methods)
            {
                if (StubBuilder<T>.FindMethodInfo(method.MethodDescription).AsMaybeValue() is not SomeValue<MethodInfo>
                    someMethodInfo)
                {
                    throw this.LogAndReturn(new ResolveTypeException(
                        $"Cannot find the method info of the method {method.MethodDescription.Name} with the parameters {method.MethodDescription.Parameters} at path {mockNode.FixtureItemId.ToPathString()}"));
                }

                var methodBuilder = stubBuilder.Method(someMethodInfo);

                if (method.ValueDefinition is not IUndefinedDefinition)
                {
                    if (method.ValueDefinition is IDelegateValueDefinition delegateValueDefinition)
                    {
                        methodBuilder.AddMethodValue(delegateValueDefinition.ValueDelegate);
                    }
                    else
                    {
                        var value = this.ResolveType(
                            method.ValueDefinition,
                            method.MethodDescription,
                            mockNode.FixtureItemId);

                        methodBuilder.AddMethodValue(value);
                    }
                }

                method.Callbacks.ForEach(callback => methodBuilder.AddMethodCallback(callback));
                methodBuilder.Setup();
            }
        }

        #endregion
    }
}
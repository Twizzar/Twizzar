using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Moq;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.TestCommon.TypeDescription.Builders
{
    public class MethodDescriptionBuilder
    {
        private IEnumerable<IParameterDescription> _declaredParameter;
        private string _name;
        private ITypeFullName _typeName;
        private AccessModifier _accessModifier = AccessModifier.CreatePublic();
        private ITypeDescription _returnTypeDescription = Mock.Of<ITypeDescription>();
        private bool _isInterface = false;
        private bool _isClass = true;
        private bool _isStruct = false;
        private bool _isStatic = false;
        private ImmutableDictionary<int, GenericParameterType> _genericParameters =
            ImmutableDictionary<int, GenericParameterType>.Empty;
        public MethodDescriptionBuilder()
        {
            this._declaredParameter = Enumerable.Empty<ParameterDescription>();
            this._name = RandomString();
            this._typeName = RandomTypeFullName();
        }

        public MethodDescriptionBuilder WithDeclaredParameter(params IParameterDescription[] parameters)
        {
            this._declaredParameter = parameters;
            return this;
        }

        public MethodDescriptionBuilder WithDeclaredParameter(IEnumerable<IParameterDescription> parameters)
        {
            this._declaredParameter = parameters;
            return this;
        }

        public MethodDescriptionBuilder WithName(string name)
        {
            this._name = name;
            return this;
        }

        public MethodDescriptionBuilder WithType(string type)
        {
            this._typeName = Mock.Of<ITypeFullName>(name => name.FullName == type);
            return this;
        }

        public MethodDescriptionBuilder WithIsStatic(bool isStatic)
        {
            this._isStatic = isStatic;
            return this;
        }

        public MethodDescriptionBuilder WithType(ITypeFullName typeFullName)
        {
            this._typeName = typeFullName;
            return this;
        }

        public MethodDescriptionBuilder AsConstructor()
        {
            this._name = ConfigurationConstants.CtorMemberName;
            return this;
        }

        public MethodDescriptionBuilder WithAccessModifier(AccessModifier accessModifier)
        {
            this._accessModifier = accessModifier;
            return this;
        }

        public MethodDescriptionBuilder WithReturnType(ITypeDescription typeDescription)
        {
            this._returnTypeDescription = typeDescription;
            return this;
        }

        public MethodDescriptionBuilder AsInterface()
        {
            this._isInterface = true;
            this._isClass = false;
            this._isStruct = false;
            return this;
        }

        public MethodDescriptionBuilder WithGenericParameters(params GenericParameterType[] genericParameters)
        {
            this._genericParameters = genericParameters
                .ToImmutableDictionary(type => type.Position);
            return this;
        }

        public IMethodDescription Build() => 
            this.Build<IMethodDescription>();

        public T Build<T>(Action<Mock<T>> additionalSetups = null) where T: class, IMethodDescription
        {
            var mock = new Mock<T>();
            mock.Setup(description => description.Name).Returns(this._name);
            mock.Setup(description => description.TypeFullName).Returns(this._typeName);
            mock.Setup(description => description.DeclaredParameters).Returns(this._declaredParameter.ToImmutableArray);
            mock.Setup(description => description.Parameters).Returns(
                string.Join(",", this._declaredParameter.Select(description => description.Name)));
            mock.Setup(description => description.AccessModifier).Returns(this._accessModifier);
            mock.Setup(description => description.GetReturnTypeDescription()).Returns(this._returnTypeDescription);
            mock.Setup(description => description.IsClass).Returns(this._isClass);
            mock.Setup(description => description.IsInterface).Returns(this._isInterface);
            mock.Setup(description => description.IsStruct).Returns(this._isStruct);
            mock.Setup(description => description.FriendlyParameterFullTypes).Returns(string.Empty);
            mock.Setup(description => description.GenericTypeArguments).Returns(this._genericParameters);
            mock.Setup(description => description.IsStatic).Returns(this._isStatic);

            additionalSetups?.Invoke(mock);
            return mock.Object;
        }
    }
}

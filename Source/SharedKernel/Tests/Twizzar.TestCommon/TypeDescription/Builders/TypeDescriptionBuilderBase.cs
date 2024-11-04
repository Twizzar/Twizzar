using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Moq;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;

namespace Twizzar.TestCommon.TypeDescription.Builders
{
    public abstract class TypeDescriptionBuilderBase<TDescriptionType>
        where TDescriptionType : class
    {
        protected TypeDescriptionBuilderBase()
        {
            this.DeclaredConstructors = Enumerable.Empty<IMethodDescription>();
            this.DeclaredProperties = Enumerable.Empty<IPropertyDescription>();
            this.DeclaredFields = Enumerable.Empty<IFieldDescription>();
            this.DeclaredMethods = Enumerable.Empty<IMethodDescription>();
        }

        protected string FullName { get; set; } = "testFullName";

        protected ITypeFullName Typename { get; set; } = Mock.Of<ITypeFullName>(name => name.FullName == "TestClass");

        protected bool IsClass { get; set; } = true;

        protected bool IsInterface { get; set; } = false;

        protected bool IsBaseType { get; set; } = false;

        protected FixtureKind FixtureKind { get; set; } = FixtureKind.BaseType;

        protected IEnumerable<IMethodDescription> DeclaredConstructors { get; set; }

        protected IEnumerable<IPropertyDescription> DeclaredProperties { get; set; }

        protected IEnumerable<IMethodDescription> DeclaredMethods { get; set; }

        protected IEnumerable<IFieldDescription> DeclaredFields { get; set; }

        protected bool IsNullableBaseType { get; set; }

        protected AccessModifier AccessModifier { get; set; } = AccessModifier.CreatePublic();

        protected ITypeDescription ReturnTypeDescription { get; set; }

        protected ImmutableDictionary<int, GenericParameterType> GenericArguments { get; set; } =
            ImmutableDictionary<int, GenericParameterType>.Empty;

        public virtual TypeDescriptionBuilderBase<TDescriptionType> WithTypeFullName(ITypeFullName typeName)
        {
            this.Typename = typeName;
            return this;
        }

        public virtual TypeDescriptionBuilderBase<TDescriptionType> WithIsNullableBaseType(bool isNullableBaseType)
        {
            this.IsNullableBaseType = isNullableBaseType;
            return this;
        }

        public virtual TypeDescriptionBuilderBase<TDescriptionType> AsBaseType()
        {
            this.IsBaseType = true;
            this.IsClass = false;
            this.IsInterface = false;
            return this.WithFixtureKind(FixtureKind.BaseType);
        }

        public virtual TypeDescriptionBuilderBase<TDescriptionType> AsClass()
        {
            this.IsClass = true;
            this.IsInterface = false;
            return this.WithFixtureKind(FixtureKind.Class);
        }

        public virtual TypeDescriptionBuilderBase<TDescriptionType> AsInterface()
        {
            this.IsClass = false;
            this.IsInterface = true;
            return this.WithFixtureKind(FixtureKind.Mock);
        }

        public virtual TypeDescriptionBuilderBase<TDescriptionType> WithIsClass(bool isClass)
        {
            this.IsClass = isClass;
            return this;
        }

        public virtual TypeDescriptionBuilderBase<TDescriptionType> WithIsInterface(bool isInterface)
        {
            this.IsInterface = isInterface;
            return this;
        }

        public virtual TypeDescriptionBuilderBase<TDescriptionType> WithDeclaredConstructors(
            params IMethodDescription[] ctors)
        {
            this.DeclaredConstructors = ctors;
            return this;
        }

        public virtual TypeDescriptionBuilderBase<TDescriptionType> WithReturnTypeDescription(
            ITypeDescription description)
        {
            this.ReturnTypeDescription = description;
            return this;
        }

        public virtual TypeDescriptionBuilderBase<TDescriptionType> WithDeclaredConstructorsParams(
            params IParameterDescription[] parameterDescriptions)
        {
            var methodDesc = new Mock<IMethodDescription>();
            methodDesc.Setup(description => description.DeclaredParameters)
                .Returns(parameterDescriptions.ToImmutableArray);
            methodDesc.Setup(description => description.AccessModifier)
                .Returns(AccessModifier.CreatePublic);

            this.DeclaredConstructors = new[]
            {
                methodDesc.Object,
            };
            return this;
        }

        public virtual TypeDescriptionBuilderBase<TDescriptionType> WithDeclaredConstructorsParams(IEnumerable<string> names)
        {
            IEnumerable<IParameterDescription> ParamDescriptions()
            {
                foreach (var name in names)
                {
                    var m = new Mock<IParameterDescription>();
                    m.Setup(description => description.Name)
                        .Returns(name);

                    m.Setup(description => description.AccessModifier)
                        .Returns(AccessModifier.CreatePublic);
                    yield return m.Object;
                }
            }

            return this.WithDeclaredConstructorsParams(ParamDescriptions().ToArray());
        }

        public virtual TypeDescriptionBuilderBase<TDescriptionType> WithDeclaredProperties(params IPropertyDescription[] props)
        {
            this.DeclaredProperties = props;
            return this;
        }

        public virtual TypeDescriptionBuilderBase<TDescriptionType> WithDeclaredFields(params IFieldDescription[] fields)
        {
            this.DeclaredFields = fields;
            return this;
        }

        public virtual TypeDescriptionBuilderBase<TDescriptionType> WithDeclaredMethods(params IMethodDescription[] methods)
        {
            this.DeclaredMethods = methods;
            return this;
        }

        public virtual TypeDescriptionBuilderBase<TDescriptionType> WithFixtureKind(FixtureKind kind)
        {
            this.FixtureKind = kind;
            return this;
        }

        public virtual TypeDescriptionBuilderBase<TDescriptionType> WithAccessModifier(AccessModifier accessModifier)
        {
            this.AccessModifier = accessModifier;
            return this;
        }

        public virtual TypeDescriptionBuilderBase<TDescriptionType> WithGenericArgument(params GenericParameterType[] typeParameters)
        {
            var builder = ImmutableDictionary.CreateBuilder<int, GenericParameterType>();

            for (var i = 0; i < typeParameters.Length; i++)
            {
                builder.Add(i, typeParameters[i]);
            }

            this.GenericArguments = builder.ToImmutable();
            return this;
        }

        /// <summary>
        /// Builds up the type description.
        /// </summary>
        /// <returns>Element <see cref="TDescriptionType"/> instance.</returns>
        public TDescriptionType Build() => this.Build(m => m);

        public abstract TDescriptionType Build(Func<Mock<TDescriptionType>, Mock<TDescriptionType>> func);
    }
}

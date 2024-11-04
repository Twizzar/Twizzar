using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Moq;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.TestCommon.TypeDescription.Builders;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.Infrastructure.Tests.Builder
{
    public class RuntimeTypeDescriptionBuilder : TypeDescriptionBuilderBase<IRuntimeTypeDescription>
    {
        private readonly Mock<Type> _typeMock;
        private Maybe<Type> _type;
        private readonly Mock<IRuntimeTypeDescription> _typeDescriptionMock;

        public RuntimeTypeDescriptionBuilder() : base()
        {
            this._typeDescriptionMock = new Mock<IRuntimeTypeDescription>();
            this._typeMock = new Mock<Type>();
        }

        public TypeDescriptionBuilderBase<IRuntimeTypeDescription> WithType(Type type)
        {
            this._type = type;
            return this;
        }

        public override TypeDescriptionBuilderBase<IRuntimeTypeDescription> WithTypeFullName(ITypeFullName typeName)
        {
            this._typeMock.Setup(type => type.FullName)
                .Returns(typeName.FullName);

            return base.WithTypeFullName(typeName);
        }

        public override TypeDescriptionBuilderBase<IRuntimeTypeDescription> AsClass()
        {
            this._typeDescriptionMock
                .Setup(description => description.DefaultFixtureKind)
                .Returns(SharedKernel.CoreInterfaces.FixtureItem.Description.Enums.FixtureKind.Class);

            return base.AsClass();
        }

        public override TypeDescriptionBuilderBase<IRuntimeTypeDescription> AsInterface()
        {
            this._typeDescriptionMock
                .Setup(description => description.DefaultFixtureKind)
                .Returns(SharedKernel.CoreInterfaces.FixtureItem.Description.Enums.FixtureKind.Mock);

            return base.AsInterface();
        }

        public override TypeDescriptionBuilderBase<IRuntimeTypeDescription> WithDeclaredConstructorsParams(IEnumerable<string> names)
        {
            IEnumerable<IParameterDescription> ParamDescriptions()
            {
                foreach (var name in names)
                {
                    var m = new Mock<IRuntimeParameterDescription>();
                    m.Setup(description => description.Name)
                        .Returns(name);
                    m.Setup(description => description.Type)
                        .Returns(new Mock<Type>().Object);
                    m.Setup(description => description.AccessModifier)
                        .Returns(SharedKernel.CoreInterfaces.FixtureItem.Description.AccessModifier.CreatePublic);
                    yield return m.Object;
                }
            }

            var methodDesc = new Mock<IMethodDescription>();
            methodDesc.Setup(description => description.DeclaredParameters)
                .Returns(ParamDescriptions().ToImmutableArray);
            methodDesc.Setup(description => description.AccessModifier)
                .Returns(SharedKernel.CoreInterfaces.FixtureItem.Description.AccessModifier.CreatePublic);

            this.DeclaredConstructors = new[]
            {
                methodDesc.Object,
            };
            return this;
        }

        /// <inheritdoc />
        public override IRuntimeTypeDescription Build(Func<Mock<IRuntimeTypeDescription>, Mock<IRuntimeTypeDescription>> func)
        {
            this._typeDescriptionMock
                .Setup(description => description.TypeFullName)
                .Returns(Mock.Of<ITypeFullName>(name => name.FullName == this.FullName));

            this._typeDescriptionMock
                .Setup(description => description.GetDeclaredConstructors())
                .Returns(this.DeclaredConstructors.ToImmutableArray());

            this._typeDescriptionMock
                .Setup(description => description.GetDeclaredProperties())
                .Returns(this.DeclaredProperties.ToImmutableArray());

            this._typeDescriptionMock
                .Setup(description => description.GetDeclaredFields())
                .Returns(this.DeclaredFields.ToImmutableArray());

            this._typeDescriptionMock
                .Setup(description => description.IsClass)
                .Returns(this.IsClass);

            this._typeDescriptionMock
                .Setup(description => description.IsInterface)
                .Returns(this.IsInterface);

            if (this._type.AsMaybeValue() is SomeValue<Type> someType)
            {
                this._typeDescriptionMock
                    .Setup(description => description.Type)
                    .Returns(someType.Value);
            }
            else
            {
                this._typeDescriptionMock
                    .Setup(description => description.Type)
                    .Returns(this._typeMock.Object);
            }

            this._typeDescriptionMock
                .Setup(description => description.DefaultFixtureKind)
                .Returns(this.FixtureKind);

            this._typeDescriptionMock
                .Setup(description => description.IsNullableBaseType)
                .Returns(this.IsNullableBaseType);

            this._typeDescriptionMock
                .Setup(description => description.IsBaseType)
                .Returns(this.IsBaseType);

            this._typeDescriptionMock
                .Setup(description => description.AccessModifier)
                .Returns(this.AccessModifier);

            return func(this._typeDescriptionMock).Object;
        }
    }
}
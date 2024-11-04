using System;
using System.Collections.Immutable;
using Moq;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.TestCommon.TypeDescription.Builders
{
    /// <summary>
    /// Builder for type description behavior.
    /// </summary>
    public class TypeDescriptionBuilder : TypeDescriptionBuilderBase<ITypeDescription>
    {
        public override ITypeDescription Build(Func<Mock<ITypeDescription>, Mock<ITypeDescription>> func)
        {
            var mock = new Mock<ITypeDescription>();

            mock
                .Setup(description => description.TypeFullName)
                .Returns(this.Typename);

            mock
                .Setup(description => description.GetDeclaredConstructors())
                .Returns(this.DeclaredConstructors.ToImmutableArray());

            mock
                .Setup(description => description.GetDeclaredProperties())
                .Returns(this.DeclaredProperties.ToImmutableArray());

            mock
                .Setup(description => description.GetDeclaredMethods())
                .Returns(this.DeclaredMethods.ToImmutableArray());

            mock
                .Setup(description => description.GetDeclaredFields())
                .Returns(this.DeclaredFields.ToImmutableArray());

            mock
                .Setup(description => description.IsClass)
                .Returns(this.IsClass);

            mock
                .Setup(description => description.IsInterface)
                .Returns(this.IsInterface);

            mock
                .Setup(description => description.DefaultFixtureKind)
                .Returns(this.FixtureKind);

            mock
                .Setup(description => description.IsNullableBaseType)
                .Returns(this.IsNullableBaseType);

            mock
                .Setup(description => description.IsBaseType)
                .Returns(this.IsBaseType);

            mock
                .Setup(description => description.AccessModifier)
                .Returns(this.AccessModifier);

            mock
                .Setup(description => description.GetReturnTypeDescription())
                .Returns(this.ReturnTypeDescription);

            mock.Setup(description => description.GetFriendlyReturnTypeFullName())
                .Returns(this.ReturnTypeDescription?.TypeFullName?.GetFriendlyCSharpTypeFullName());

            mock.Setup(description => description.GenericTypeArguments)
                .Returns(this.GenericArguments);

            mock = func(mock);

            return mock.Object;
        }
    }
}

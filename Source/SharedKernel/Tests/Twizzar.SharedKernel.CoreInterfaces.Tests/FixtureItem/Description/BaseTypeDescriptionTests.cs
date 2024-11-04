using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.Tests.FixtureItem.Description;

[TestFixture]
public partial class BaseTypeDescriptionTests
{
    #region members

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<BaseTypeDescription>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void GetEnumValues_returns_names_of_all_fields()
    {
        // arrange
        var declaredFields = new ItemBuilder<IFieldDescription>().BuildMany(3);

        var returnTypeDescription = new ItemBuilder<ITypeDescription>().Build();

        var returnTypeDescriptionMock = Mock.Get(returnTypeDescription);

        returnTypeDescriptionMock.Setup(
                m => m.GetDeclaredFields())
            .Returns(declaredFields.ToImmutableArray);

        // act
        var sut = new BaseTypeDescriptionMock(returnTypeDescription, true);

        // assert
        var enumValues = sut.GetEnumNames()
            .AsMaybeValue()
            .Should()
            .BeAssignableTo<SomeValue<string[]>>()
            .Subject.Value;

        enumValues.Should().HaveCount(3);
        enumValues.Should().BeEquivalentTo(declaredFields.Select(description => description.Name));
    }

    #endregion

    #region Nested type: BaseTypeDescriptionMock

    private class BaseTypeDescriptionMock : BaseTypeDescription
    {
        #region fields

        private readonly ITypeDescription _typeDescription;

        #endregion

        #region ctors

        /// <inheritdoc />
        public BaseTypeDescriptionMock(ITypeDescription typeDescription, bool isEnum)
            : base(Mock.Of<IBaseTypeService>())
        {
            this._typeDescription = typeDescription;
            this.IsEnum = isEnum;
        }

        #endregion

        #region members

        /// <inheritdoc />
        public override ITypeDescription GetReturnTypeDescription() => this._typeDescription;

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents() => throw new NotImplementedException();

        #endregion
    }

    #endregion
}
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Twizzar.Design.Ui.Helpers;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Parser.SyntaxTree.Keywords;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.TestCommon.TypeDescription.Builders;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Test.Validator;

public abstract class BaseTypeValidatorTests : BaseValidatorTests
{
    #region members

    [TestMethod]
    public virtual void DefaultValue_is_unique_for_parameters_and_type_descriptions()
    {
        // arrange
        var sut = this.CreateSut(new ParameterDescriptionBuilder().WithBaseType(true).Build());
        var sut2 = this.CreateSut(new TypeDescriptionBuilder().AsBaseType().WithIsNullableBaseType(true).Build());

        // assert
        sut.DefaultValue.Should().Be(KeyWords.Unique);
        sut2.DefaultValue.Should().Be(KeyWords.Unique);
    }

    [TestMethod]
    public virtual void DefaultValue_is_undefined_for_other_than_parameters_and_types()
    {
        // arrange
        var sut = this.CreateSut(new PropertyDescriptionBuilder().WithBaseType(true).Build());

        // assert
        sut.DefaultValue.Should().Be(KeyWords.Undefined);
    }

    [TestMethod]
    public void ValidInput_contains_null_when_nullable_baseType()
    {
        // arrange
        var sut = this.CreateSut(new TypeDescriptionBuilder().WithIsNullableBaseType(true).Build());

        // assert
        sut.ValidInput.Should().Contain(KeyWords.Null);
    }

    [TestMethod]
    public async Task Validator_with_IMemberDescription_undefinedToken_is_valid()
    {
        // arrange
        var token = new ViUndefinedToken(0, 10, RandomString());
        var sut = this.CreateSut(new Mock<IMemberDescription>().Object);

        // act
        var result = await sut.ValidateAsync( Success(token));

        // assert
        result.Should().Be(token);
    }

    [TestMethod]
    public async Task Validator_with_no_IMemberDescription_undefinedToken_is_invalid()
    {
        // arrange
        var token = new ViUndefinedToken(0, 10, RandomString());
        var sut = this.CreateSut(new Mock<ITypeDescription>().Object);

        // act
        var result =  await sut.ValidateAsync(Success(token));

        // assert
        result.Should().BeAssignableTo<IViInvalidToken>();
    }

    #endregion
}
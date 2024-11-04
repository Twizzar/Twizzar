using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Parser;
using Twizzar.TestCommon;

namespace Twizzar.Design.Test.Parser;

[TestClass]
public class EnumParserTests
{
    [TestMethod]
    [DataRow("SomeTest")]
    [DataRow("allinlower")]
    [DataRow("ALLINCAPS")]
    [DataRow("_with_underlines")]
    [DataRow("NumerNotAtStart8")]
    public void Valid_input_results_in_Success(string input)
    {
        // arrange
        var enumParser = Build.New<EnumParser>();

        // act
        var result = enumParser.Parse(input);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.GetSuccessUnsafe().ContainingText.Should().Be(input);
    }

    [TestMethod]
    [DataRow("With Space")]
    [DataRow("#Hashtag")]
    [DataRow("$")]
    [DataRow("\"")]
    [DataRow("7NumberAtStart")]
    [DataRow("/")]
    public void Invalid_input_results_in_failure(string input)
    {
        // arrange
        var enumParser = Build.New<EnumParser>();

        // act
        var result = enumParser.Parse(input);

        // assert
        result.IsSuccess.Should().BeFalse();
    }
}
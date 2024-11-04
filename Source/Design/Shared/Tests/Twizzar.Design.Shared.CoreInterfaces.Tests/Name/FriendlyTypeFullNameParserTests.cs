using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.CoreInterfaces.Tests.Name
{
    [TestFixture]
    public class FriendlyTypeFullNameParserTests
    {
        [Test]
        public void METHOD()
        {
            var result = TypeFullNameParser.FriendlyTypeParser.Parse("System.Int32    ");

            var parseSuccess = result.AsResultValue().Should().BeAssignableTo<SuccessValue<ParseSuccess<ITypeFullNameToken>>>().Subject;
            var typeFullNameToken = parseSuccess.Value.Value;

            typeFullNameToken.ContainingAssembly.IsNone.Should().BeTrue();
            typeFullNameToken.GenericPostfix.IsNone.Should().BeTrue();
            typeFullNameToken.Namespace.Should().Be("System.");
            typeFullNameToken.Typename.Should().Be("Int32");
            typeFullNameToken.ContainingText.Should().Be("System.Int32");
        }

        [Test]
        public void METHODGenerics()
        {
            var result = TypeFullNameParser.FriendlyTypeParser.Parse("Tuple<Tuple<Int>, String>   ");

            var parseSuccess = result.AsResultValue().Should().BeAssignableTo<SuccessValue<ParseSuccess<ITypeFullNameToken>>>().Subject;
            var typeFullNameToken = parseSuccess.Value.Value;

            typeFullNameToken.ContainingAssembly.IsNone.Should().BeTrue();
            typeFullNameToken.GenericPostfix.IsSome.Should().BeTrue();
            typeFullNameToken.Namespace.Should().Be("");
            typeFullNameToken.Typename.Should().Be("Tuple");
            typeFullNameToken.GenericTypeArguments.Should().HaveCount(2);
            typeFullNameToken.GenericTypeArguments[0].Typename.Should().Be("Tuple");
            typeFullNameToken.GenericTypeArguments[0].GenericTypeArguments.Should().HaveCount(1);
            typeFullNameToken.GenericTypeArguments[1].Typename.Should().Be("String");
            typeFullNameToken.ContainingText.Should().Be("Tuple<Tuple<Int>, String>");
        }

    }
}
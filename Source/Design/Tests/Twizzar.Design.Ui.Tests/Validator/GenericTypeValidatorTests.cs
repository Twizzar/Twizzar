using NUnit.Framework;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Link;
using Twizzar.Design.Ui.Parser.SyntaxTree;
using Twizzar.Design.Ui.Parser.SyntaxTree.Link;
using Twizzar.Design.Ui.Validator;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Ui.Tests.Validator;

public partial class GenericTypeValidatorTests
{
    [Test]
    public void Ctor_throws_when_arguments_is_null()
    {
        Verify.Ctor<GenericTypeValidator>()
            .ShouldThrowArgumentNullException();
    }

    private IResult<IViToken, ParseFailure> CreateLinkTokenResult(string typeName)
    {
        var fullNameToken = new TypeFullNameTokenBuilder()
            .WithTypeName(typeName)
            .Build();
        var typeToken = ViTypeToken.Create(fullNameToken);
        var linkToken = ViLinkToken.Create(Maybe.Some<IViTypeToken>(typeToken), Maybe.None());
        return linkToken.ToSuccess<IViToken, ParseFailure>();
    }
}
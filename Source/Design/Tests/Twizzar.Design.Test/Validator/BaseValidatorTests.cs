using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Validator;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using ViCommon.Functional.Monads.ResultMonad;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Test.Validator;

[TestClass]
public abstract class BaseValidatorTests
{
    #region properties

    public abstract IValidator CreateSut(IBaseDescription typeDescription);

    #endregion

    #region members

    public static IResult<IViToken, ParseFailure> Success(IViToken token) =>
        Result.Success<IViToken, ParseFailure>(token);

    public static IResult<IViToken, ParseFailure> Failure(ParseFailure parseFailure) =>
        Result.Failure<IViToken, ParseFailure>(parseFailure);

    public static IViToken CreateRandomToken(Func<int, int, string, IViToken> creator) =>
        creator(RandomInt(0, 5), RandomInt(0, 5), RandomString());

    #endregion
}
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.CoreInterfaces.TestCreation.BaseConfig;

/// <summary>
/// Parser for parsing a config which has tags with leading content.
/// </summary>
public interface IBaseConfigParser
{
    /// <summary>
    /// Parse a text.
    /// </summary>
    /// <param name="text"></param>
    /// <returns>Config syntax on success; else a ParseFailure.</returns>
    IResult<ConfigSyntax, ParseFailure> ParseBaseConfig(string text);
}
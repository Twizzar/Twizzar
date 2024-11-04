using System.Collections.Generic;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.CoreInterfaces.TestCreation.Mapping;

/// <summary>
/// Matcher for matching patterns in the mapping to the given mapping result.
/// </summary>
public interface IWildcardPatternMatcher
{
    /// <summary>
    /// Match an input to a mapping entries.
    /// This will try to match from start to end all the entries till one matches and returns the match.
    /// If none is matching this will return a <see cref="Failure"/>.
    /// </summary>
    /// <param name="input">The input to match.</param>
    /// <param name="mapping">The mapping entries, order is important.</param>
    /// <returns>Success with the first match if one is matching; else failure.</returns>
    IResult<string, Failure> Match(string input, IEnumerable<MappingEntry> mapping);
}
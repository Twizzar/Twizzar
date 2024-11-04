using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Twizzar.Design.CoreInterfaces.TestCreation.Mapping;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Mapping;

/// <inheritdoc cref="IWildcardPatternMatcher" />
public class WildcardPatternMatcher : IWildcardPatternMatcher
{
    #region members

    /// <inheritdoc />
    public IResult<string, Failure> Match(string input, IEnumerable<MappingEntry> mapping)
    {
        foreach (var (from, to) in mapping)
        {
            if (from.AsMaybeValue() is not SomeValue<string> someValue)
            {
                return to.ToSuccess<string, Failure>();
            }

            if (IsWildcardMatch(someValue, input, out var matchGroups))
            {
                var result = to;
                for (var i = 1; i < matchGroups.Length; i++)
                {
                    result = result.Replace($"${i}", matchGroups[i]);
                }

                return result.ToSuccess<string, Failure>();
            }
        }

        return new Failure($"No matching mapping was found for input {input} and the patterns {mapping.Select(entry => entry.Pattern).Somes().ToCommaSeparated()}.").ToResult<string, Failure>();
    }

    private static bool IsWildcardMatch(string wildcardPattern, string subject, out string[] matchedGroups)
    {
        wildcardPattern = wildcardPattern.Replace('\\', '/').Replace("//", "/");
        subject = subject.Replace('\\', '/').Replace("//", "/");
        matchedGroups = Array.Empty<string>();

        if (string.IsNullOrWhiteSpace(wildcardPattern))
        {
            return false;
        }

        var regexPattern = string.Concat("^", Regex.Escape(wildcardPattern).Replace("\\*", "(.*)"), "$");

        var wildcardCount = wildcardPattern.Count(x => x.Equals('*'));

        switch (wildcardCount)
        {
            case <= 0:
                return subject.Equals(wildcardPattern, StringComparison.CurrentCultureIgnoreCase);
            default:
                try
                {
                    var match = Regex.Match(subject, regexPattern);
                    matchedGroups = new string[match.Groups.Count];
                    for (var i = 0; i < match.Groups.Count; i++)
                    {
                        var matchGroup = match.Groups[i];
                        matchedGroups[i] = matchGroup.Value;
                    }

                    return match.Success;
                }
                catch
                {
                    return false;
                }
        }
    }

    #endregion
}
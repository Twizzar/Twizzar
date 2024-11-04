using System.Text.RegularExpressions;
using Moq;
using Twizzar.Design.CoreInterfaces.Adornment;

namespace Twizzar.Design.TestCommon
{
    public record RegexSpan(int Start, int Length, IViSpanVersion Version) : IViSpan
    {
        public static RegexSpan CreateWithRegex(string code, string pattern, int group = 0)
        {
            var match = Regex.Match(code, pattern);
            var matchGroup = match.Groups[group];
            return new RegexSpan(matchGroup.Index, matchGroup.Length, Mock.Of<IViSpanVersion>());
        }

        public static RegexSpan CreateWithStringMatch(string code, string pattern)
        {
            var start = code.IndexOf(pattern);
            var length = pattern.Length;
            return new RegexSpan(start, length, Mock.Of<IViSpanVersion>());
        }

        #region Implementation of IViSpan

        /// <inheritdoc />
        public IViSpan WithVersion(IViSpanVersion version) =>
            throw new System.NotImplementedException();

        #endregion
    }
}
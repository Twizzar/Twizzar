using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using Twizzar.TestCommon;

namespace Twizzar.Design.CoreInterfaces.Tests.Name
{
    [TestFixture]
    public class TypeFullNameParserTests
    {
        private static IEnumerable<object[]> TestTypes
        {
            get
            {
                yield return new object[] { typeof(List<int[]>) };
                yield return new object[] {typeof(int?)};
                yield return new object[] {typeof(int)};
                yield return new object[] {typeof(object)};
                yield return new object[] {typeof(TypeFullNameParserTests)};
                yield return new object[] {typeof(Tuple<int, Tuple<string, string>, IEnumerable<TypeFullNameParserTests>>)};
            }
        }

        [TestCaseSource(nameof(TestTypes))]
        public void ParsedToken_fullName_is_equal_to_type_fullName(Type type)
        {
            // act
            var result = TypeFullNameParser.MetaTypeParser.Parse(type.FullName);

            // assert
            var token = TestHelper.AssertAndUnwrapSuccess(result).Value;
            token.ToFullName().Should().Be(type.FullName);
            token.Namespace.Should().Be(type.Namespace + ".");

            var name = token.Typename + token.GenericPostfix.SomeOrProvided(string.Empty);
            name.Should().Be(type.Name);

            var tokenGenericTypeArguments = token.GenericTypeArguments.Select(nameToken => nameToken.ToFullName());
            var typeGenericTypeArguments = type.GenericTypeArguments.Select(t => t.FullName);

            tokenGenericTypeArguments
                .Should()
                .BeEquivalentTo(typeGenericTypeArguments);
        }
    }
}
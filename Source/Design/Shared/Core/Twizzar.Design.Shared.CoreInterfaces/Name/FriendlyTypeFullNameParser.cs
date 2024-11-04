using System.Collections.Generic;
using System.Linq;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using ViCommon.Functional.Monads.ResultMonad;

#pragma warning disable SA1102 // Query clause should follow previous clause

namespace Twizzar.Design.Shared.CoreInterfaces.Name
{
    /// <summary>
    /// Parser for parsing friendly c# generics.
    /// </summary>
    public static class FriendlyTypeFullNameParser
    {
        #region properties

        /// <summary>
        /// Gets a generic parser which returns the generic postfix (`1) and the generic types.
        /// </summary>
        public static ParserExtensions.Parser<(string GenericPostfix, IEnumerable<ITypeFullNameToken> GenericTypes)> GenericParser =>
            i =>

                // Parse the generic types this is everything between [ ] separated with commas
                from genericTypes in GenericTypesParser(i)

                select ParseSuccess<(string, IEnumerable<ITypeFullNameToken>)>.FromSpan(
                    i,
                    genericTypes.ParsedSpan.End,
                    ("`" + genericTypes.Value.Count(), genericTypes.Value));

        /// <summary>
        /// Gets a parser which parses the generic part of an type in the format &lt;TypeFullName, TypeFullName, , ...&gt;
        /// For example : &lt;System.Tuple`2&lt;System.Int32, System.Int32&gt;&gt;.
        /// </summary>
        private static ParserExtensions.Parser<IEnumerable<ITypeFullNameToken>> GenericTypesParser =>
            input =>

                // Parse the opening braced [
                from openingBraced in Consume.Char('<')(input)

                // Parse all the generic types
                from genericTypes in GenericParameterParser.OneOrMore()(openingBraced.OutputPoint)

                // Parse the closing braced ]
                from closingBraced in Consume.Char('>')(genericTypes.OutputPoint)

                select
                    ParseSuccess<IEnumerable<ITypeFullNameToken>>.FromSpan(
                        input,
                        closingBraced.ParsedSpan.End,
                        genericTypes.Value);

        /// <summary>
        /// Gets a parser which parses a generic type parameter in the format &lt;TypeFullName, ...&gt;.
        /// </summary>
        private static ParserExtensions.Parser<ITypeFullNameToken> GenericParameterParser =>
            input =>

                // parse recursively the type full name.
                from typeFullName in TypeFullNameParser.FriendlyTypeParser(input)

                // when another generic follows there will be a comma so we optional parsed the comma
                from tailingComma in Consume.Char(',').WithSurroundingWhiteSpaces().Optional()(
                    typeFullName.OutputPoint)
                select
                    ParseSuccess<ITypeFullNameToken>.FromSpan(
                        input,
                        tailingComma.ParsedSpan.End,
                        typeFullName.Value);

        #endregion
    }
}
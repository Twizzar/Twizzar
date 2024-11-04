using System.Collections.Generic;
using System.Linq;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using ViCommon.Functional.Monads.ResultMonad;

#pragma warning disable SA1102 // Query clause should follow previous clause

namespace Twizzar.Design.Shared.CoreInterfaces.Name
{
    /// <summary>
    /// Parser for parsing meta c# generics.
    /// </summary>
    public static class MetaTypeFullNameParser
    {
        #region properties

        /// <summary>
        /// Gets a generic parser which returns the generic postfix (`1) and the generic types.
        /// </summary>
        public static ParserExtensions.Parser<(string GenericPostfix, IEnumerable<ITypeFullNameToken> GenericTypes)> GenericParser =>
            i =>

                // Parse the prefix of a generic like ´1 when it exists
                from genericPostFix in GenericPostfixParser.Optional()(i)

                // Parse the generic types this is everything between [ ] separated with commas
                from genericTypes in GenericTypesParser.Optional()(genericPostFix.OutputPoint)

                select ParseSuccess<(string, IEnumerable<ITypeFullNameToken>)>.FromSpan(
                        i,
                        genericTypes.ParsedSpan.End,
                        (genericPostFix.Value.SomeOrProvided(string.Empty), genericTypes.Value.SomeOrProvided(Enumerable.Empty<ITypeFullNameToken>())));

        private static ParserExtensions.Parser<string> GenericPostfixParser =>
            input =>
                from head in Consume.Char('`')(input)
                from body in Consume.Digit.Many()(head.OutputPoint)
                select ParseSuccess<string>.FromSpan(
                    input,
                    body.ParsedSpan.End,
                    head.Value + body.Value.AsString());

        /// <summary>
        /// Gets a parser which parses the generic part of an type in the format [[TypeFullName, containingAssembly], [TypeFullName, containingAssembly], ...]
        /// For example : [System.Tuple`2[
        ///                    [System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],
        ///                    [System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]
        ///               ]
        /// The line breaker or only there for readability.
        /// </summary>
        private static ParserExtensions.Parser<IEnumerable<ITypeFullNameToken>> GenericTypesParser =>
            input =>

                // Parse the opening braced [
                from openingBraced in Consume.Char('[')(input)

                // Parse all the generic types formatted like this [TypeFullName1, containingAssembly1, TypeFullName2, containingAssembly2]
                // TypeFullName1 can also have Generic types
                // This could look like this [System.Tuple´1[System.Int32, assembly], assembly, System.Int32, assembly]
                // where assembly can look like this: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
                from genericTypes in GenericParameterParser.OneOrMore()(openingBraced.OutputPoint)

                // Parse the closing braced ]
                from closingBraced in Consume.Char(']')(genericTypes.OutputPoint)
                select
                    ParseSuccess<IEnumerable<ITypeFullNameToken>>.FromSpan(
                        input,
                        closingBraced.ParsedSpan.End,
                        genericTypes.Value);

        /// <summary>
        /// Gets a parser which parses a generic type parameter in the format [TypeFullName, containingAssembly].
        /// For example: [[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]].
        /// </summary>
        private static ParserExtensions.Parser<ITypeFullNameToken> GenericParameterParser =>
            input =>

                // parse the opening braced [
                from openingBraced in Consume.Char('[')(input)

                // parse recursively the type full name.
                from typeFullName in TypeFullNameParser.MetaTypeParser(openingBraced.OutputPoint)

                // parse the containing assembly something like this: ,mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
                from containingAssembly in Consume.CharExcept(']').OneOrMore()(typeFullName.OutputPoint)

                // parse a closing braced ]
                from closingBraced in Consume.Char(']')(containingAssembly.OutputPoint)

                // when another generic follows there will be a comma so we optional parsed the comma
                from tailingComma in Consume.Char(',').WithSurroundingWhiteSpaces().Optional()(
                    closingBraced.OutputPoint)
                select
                    ParseSuccess<ITypeFullNameToken>.FromSpan(
                        input,
                        tailingComma.ParsedSpan.End,
                        typeFullName.Value.AddContainingAssembly(
                            containingAssembly.Value
                                .AsString()));

        #endregion
    }
}
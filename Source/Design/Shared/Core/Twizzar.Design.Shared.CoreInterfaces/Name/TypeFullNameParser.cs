using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

#pragma warning disable SA1102 // Query clause should follow previous clause

namespace Twizzar.Design.Shared.CoreInterfaces.Name
{
    /// <summary>
    /// Static class for getting parses which parses text to <see cref="ITypeFullNameToken"/>.
    /// </summary>
    public static class TypeFullNameParser
    {
        #region properties

        /// <summary>
        /// Gets a parser which parses a meta type full name to a <see cref="ITypeFullNameToken"/>
        /// <example>
        /// For example:
        ///  <c>System.Nullable`1[[System.Int32, mscorlib, Version=4.0.e0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]</c>
        /// </example>
        /// When the type is not generic the genericTypeFullNames sequence is empty.
        /// </summary>
        public static ParserExtensions.Parser<ITypeFullNameToken> MetaTypeParser =>
            FullTypeNameAndGenericTypeParser(MetaTypeFullNameParser.GenericParser);

        /// <summary>
        /// Gets a parser which parses a type full name to a <see cref="ITypeFullNameToken"/>.
        /// <example>
        /// For example:
        ///     <c>System.Nullable&lt;System.Int32&gt;</c>
        /// </example>
        /// </summary>
        public static ParserExtensions.Parser<ITypeFullNameToken> FriendlyTypeParser =>
            FullTypeNameAndGenericTypeParser(FriendlyTypeFullNameParser.GenericParser);

        #endregion

        #region members

        private static ParserExtensions.Parser<IEnumerable<char>> ArrayBrackets =>
            i =>
                from start in Consume.Char('[').WithSurroundingWhiteSpaces()(i)
                from commas in Consume.Char(',').WithSurroundingWhiteSpaces().Many().Flatten()(start.OutputPoint)
                from end in Consume.Char(']')(commas.OutputPoint)
                select ParseSuccess<IEnumerable<char>>.FromSpan(
                    i,
                    end.ParsedSpan.End,
                    start.Value
                        .Concat(commas.Value)
                        .Append(end.Value));

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "StyleCop.CSharp.ReadabilityRules",
            "SA1118:Parameter should not span multiple lines",
            Justification = "Readability")]
        private static
            ParserExtensions.Parser<(IEnumerable<char> TypeName, IEnumerable<ITypeFullNameToken> GenericTypes)>
            DeclaringTypeParser(
                ParserExtensions.Parser<(string GenericPostfix, IEnumerable<ITypeFullNameToken> GenericTypes)>
                    genericParser) =>
            i =>
                from name in Consume.CSharpFullType(i)
                from genericTuple in genericParser.Optional()(name.OutputPoint)
                from plus in Consume.Char('+')(genericTuple.OutputPoint)
                select ParseSuccess<(IEnumerable<char>, IEnumerable<ITypeFullNameToken>)>.FromSpan(
                    i,
                    plus.OutputPoint,
                    (name.Value.Concat(genericTuple.Value.Match(
                            tuple => tuple.GenericPostfix.Append(plus.Value),
                            string.Empty)),
                        genericTuple.Value.Map(tuple => tuple.GenericTypes)
                            .SomeOrProvided(Enumerable.Empty<ITypeFullNameToken>())));

        private static ParserExtensions.Parser<ITypeFullNameToken> FullTypeNameAndGenericTypeParser(ParserExtensions.Parser<(string GenericPostfix, IEnumerable<ITypeFullNameToken> GenericTypes)> genericParser) =>
            input =>

                // Parse the namespace
                from nameSpace in Consume.CSharpType.And(Consume.Char('.')).Many()(input)

                // When the type is a inner type parse first the outer type
                from declaringType in DeclaringTypeParser(genericParser).Optional()(nameSpace.OutputPoint)

                // parse the type name
                from typeName in Consume.CSharpType(declaringType.OutputPoint)

                // parse the prefix of a generic like ´1 and the generic types when exists
                from genericTuple in genericParser.Optional()(typeName.OutputPoint)

                // parse tailing [ ] bracket if is it an array.
                from arrayBrackets in ArrayBrackets.Many()(genericTuple.OutputPoint)

                select ParseSuccess<ITypeFullNameToken>.FromSpan(
                    input,
                    arrayBrackets.ParsedSpan.End,
                    new TypeFullNameToken(
                        nameSpace.Value
                            .Select(chars => chars.AsString())
                            .Aggregate(string.Empty, (a, b) => a + b),
                        typeName.Value.AsString(),
                        declaringType.Value
                            .Map(tuple => tuple.TypeName)
                            .Map(chars => chars.AsString()),
                        genericTuple.Value.Map(tuple => tuple.GenericPostfix),
                        BuildGenericParameter(
                            declaringType.Value.Map(tuple => tuple.GenericTypes),
                            genericTuple.Value.Map(tuple => tuple.GenericTypes)),
                        arrayBrackets.Value
                            .Select(chars => chars.AsString())
                            .ToImmutableArray(),
                        input.Content.Substring(input.Position, arrayBrackets.OutputPoint.Position - input.Position)));

        private static ImmutableArray<ITypeFullNameToken> BuildGenericParameter(params Maybe<IEnumerable<ITypeFullNameToken>>[] maybes)
        {
            var builder = ImmutableArray.CreateBuilder<ITypeFullNameToken>();

            foreach (var maybe in maybes)
            {
                maybe.IfSome(tokens => builder.AddRange(tokens));
            }

            return builder.ToImmutable();
        }

        #endregion
    }
}
using System;

namespace Twizzar.SharedKernel.CoreInterfaces.FunctionalParser
{
    /// <inheritdoc />
    public class ParseSuccess<T> : IParseSuccess<T>
    {
        private ParseSuccess(ParserPoint outputPoint, T value, ParserSpan parsedSpan)
        {
            this.OutputPoint = outputPoint;
            this.Value = value;
            this.ParsedSpan = parsedSpan;
        }

        #region Implementation of IParseSuccess<out T>

        /// <inheritdoc />
        public ParserPoint OutputPoint { get; }

        /// <inheritdoc />
        public ParserSpan ParsedSpan { get; }

        /// <inheritdoc />
        public T Value { get; }

        /// <inheritdoc />
        public IParseSuccess<TNew> WithValue<TNew>(TNew newValue) =>
            new ParseSuccess<TNew>(this.OutputPoint, newValue, this.ParsedSpan);

        /// <inheritdoc />
        public IParseSuccess<TNew> Map<TNew>(Func<T, TNew> mapFunc) =>
            this.WithValue(mapFunc(this.Value));

        /// <summary>
        /// Create a new parse success from a parsed span.
        /// The <see cref="OutputPoint"/> will be the end.
        /// </summary>
        /// <param name="start">The start of the span.</param>
        /// <param name="end">The end of the span.</param>
        /// <param name="value">The value.</param>
        /// <returns>A new instance of <see cref="ParseSuccess{T}"/>.</returns>
        public static ParseSuccess<T> FromSpan(ParserPoint start, ParserPoint end, T value) =>
            new(end, value, new ParserSpan(start, end));

        #endregion
    }
}
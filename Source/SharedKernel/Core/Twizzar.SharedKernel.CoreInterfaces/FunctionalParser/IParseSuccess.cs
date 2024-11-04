using System;

namespace Twizzar.SharedKernel.CoreInterfaces.FunctionalParser
{
    /// <summary>
    /// Represents a parsing result.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    public interface IParseSuccess<out T>
    {
        /// <summary>
        /// Gets the new position after parsing.
        /// </summary>
        ParserPoint OutputPoint { get; }

        /// <summary>
        /// Gets the parsed span.
        /// </summary>
        ParserSpan ParsedSpan { get; }

        /// <summary>
        /// Gets the resulting value.
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Create a new instance with a different value.
        /// </summary>
        /// <typeparam name="TNew">The type of the value.</typeparam>
        /// <param name="newValue">The new value.</param>
        /// <returns>A new instance of <see cref="IParseSuccess{T}"/>.</returns>
        IParseSuccess<TNew> WithValue<TNew>(TNew newValue);

        /// <summary>
        /// Convert the value to a new value with a map function.
        /// </summary>
        /// <typeparam name="TNew">The new value type.</typeparam>
        /// <param name="mapFunc">The map function.</param>
        /// <returns>A new <see cref="IParseSuccess{T}"/>.</returns>
        IParseSuccess<TNew> Map<TNew>(Func<T, TNew> mapFunc);
    }
}
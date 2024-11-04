using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.SharedKernel.CoreInterfaces.FunctionalParser
{
    /// <summary>
    /// Represents a pointer to a char in a text to parse.
    /// </summary>
    public class ParserPoint
    {
        #region ctors

        private ParserPoint(string content, int position)
        {
            EnsureHelper.GetDefault.Parameter(content, nameof(content))
                .IsNotNullAndNotEmpty()
                .ThrowOnFailure();

            EnsureHelper.GetDefault.Parameter(position, nameof(position))
                .IsGreaterEqualThan(0)
                .IsLessThan(content.Length)
                .ThrowOnFailure();

            this.Content = content;
            this.Position = position;
            this.Current = this.Content[this.Position];
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the content of the Input.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Gets the position of the input.
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// Gets the current char.
        /// </summary>
        public char Current { get; }

        /// <summary>
        /// Gets a value indicating whether the input has a next character.
        /// <remarks>The input ends with the char \0</remarks>
        /// </summary>
        public bool HasNext => this.Position < this.Content.Length - 1;

        /// <summary>
        /// Gets a value indicating whether the input has a previous character.
        /// </summary>
        public bool HasPrevious => this.Position > 0;

        /// <summary>
        /// Gets the line and column.
        /// </summary>
        public (int Line, int Column) LineAndColumn => GetLineAndColumnFromPosition(this.Content, this.Position);

        #endregion

        #region members

        /// <summary>
        /// Convert the point to int.
        /// </summary>
        /// <param name="p">The point.</param>
        public static implicit operator int(ParserPoint p) => p.Position;

        /// <inheritdoc />
        public override string ToString() => this.Content.Insert(this.Position, "^");

        /// <summary>
        /// Create a new parser input.
        /// </summary>
        /// <param name="content">The content of the input.</param>
        /// <returns>A new <see cref="ParserPoint"/>.</returns>
        public static ParserPoint New(string content) => New(content + '\0', 0);

        /// <summary>
        /// Create a new parser input.
        /// </summary>
        /// <param name="content">The content of the input.</param>
        /// <param name="position">The position of the input.</param>
        /// <returns>A new <see cref="ParserPoint"/>.</returns>
        public static ParserPoint New(string content, int position) => new(content, position);

        /// <summary>
        /// Get the content between two inputs.
        /// </summary>
        /// <param name="start">The start of the input.</param>
        /// <param name="end">The end of the input (exclusive).</param>
        /// <returns>The content as a string.</returns>
        public static string GetContent(ParserPoint start, ParserPoint end)
        {
            if (start.Content != end.Content)
            {
                throw new InternalException("Cannot get the Content of two inputs with different Content.");
            }

            var length = end.Position - start.Position;
            return start.Content.Substring(start.Position, length);
        }

        /// <summary>
        /// Get the next input parser.
        /// </summary>
        /// <returns>A new input parse with the position this.Position + 1.</returns>
        public ParserPoint Next() => new(this.Content, this.Position + 1);

        /// <summary>
        /// Get the previous input parser.
        /// </summary>
        /// <returns>A new input parse with the position this.Position + 1.</returns>
        public ParserPoint Previous() => new(this.Content, this.Position - 1);

        private static (int Line, int Column) GetLineAndColumnFromPosition(string text, int position)
        {
            var line = 1;
            var column = 1;

            for (var i = 0; i < position; i++)
            {
                if (text[i] == '\n')
                {
                    line++;
                    column = 1;
                }
                else if (text[i] != '\r')
                {
                    column++;
                }
            }

            return (line, column);
        }

        #endregion
    }
}
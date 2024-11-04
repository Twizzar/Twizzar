using System.Collections.Generic;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.SharedKernel.CoreInterfaces.FunctionalParser
{
    /// <summary>
    /// Represents a span in the text.
    /// </summary>
    public class ParserSpan : ValueObject, IHasLogger, IHasEnsureHelper
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserSpan"/> class.
        /// </summary>
        /// <param name="start">The start of the span.</param>
        /// <param name="end">The end exclusive.</param>
        public ParserSpan(ParserPoint start, ParserPoint end)
        {
            this.EnsureMany()
                .Parameter(start, nameof(start))
                .Parameter(end, nameof(end))
                .ThrowWhenNull();

            this.Start = start;
            this.End = end;
            this.Length = this.End.Position - this.Start.Position;
            this.Content = ParserPoint.GetContent(start, end);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserSpan"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="length">The length of the span.</param>
        public ParserSpan(ParserPoint start, int length)
        {
            this.EnsureMany()
                .Parameter(start, nameof(start))
                .ThrowWhenNull();

            this.Start = start;
            this.End = ParserPoint.New(start.Content, start.Position + length);
            this.Length = length;
            this.Content = ParserPoint.GetContent(start, this.End);
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the start of the span.
        /// </summary>
        public ParserPoint Start { get; }

        /// <summary>
        /// Gets the end of the span.
        /// </summary>
        public ParserPoint End { get; }

        /// <summary>
        /// Gets the length of the span.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Gets the containing text of the span.
        /// </summary>
        public string Content { get; }

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Start;
            yield return this.End;
            yield return this.Length;
            yield return this.Content;
        }

        #endregion
    }
}
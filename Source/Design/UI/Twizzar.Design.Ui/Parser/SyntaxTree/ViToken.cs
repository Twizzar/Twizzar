using System.Collections.Generic;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.Ui.Parser.SyntaxTree
{
    /// <inheritdoc cref="IViToken" />
    public abstract class ViToken : ValueObject, IViToken
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViToken"/> class.
        /// </summary>
        /// <param name="start">The start index.</param>
        /// <param name="length">The length of the token.</param>
        /// <param name="containingText">The containing text.</param>
        protected ViToken(int start, int length, string containingText)
        {
            this.Start = start;
            this.Length = length;
            this.ContainingText = containingText;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public int Start { get; }

        /// <inheritdoc />
        public int Length { get; }

        /// <inheritdoc />
        public string ContainingText { get; }

        #endregion

        #region members

        /// <inheritdoc />
        public abstract IViToken With(int start, int length, string containingText);

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Start;
            yield return this.Length;
            yield return this.ContainingText;
        }

        #endregion
    }
}
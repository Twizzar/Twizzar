using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Ui.Parser.SyntaxTree.Literals
{
    /// <inheritdoc cref="IViEnumToken" />
    public class ViEnumToken : ViToken, IViEnumToken
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViEnumToken"/> class.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="containingText"></param>
        /// <param name="declaringType"></param>
        /// <param name="enumName"></param>
        /// <param name="enumType"></param>
        public ViEnumToken(
            int start,
            int length,
            string containingText,
            string enumName,
            Maybe<string> declaringType,
            Maybe<string> enumType)
            : base(start, length, containingText)
        {
            this.EnumName = enumName;
            this.DeclaringType = declaringType;
            this.EnumType = enumType;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public string EnumName { get; }

        /// <inheritdoc />
        public Maybe<string> EnumType { get; }

        /// <inheritdoc />
        public Maybe<string> DeclaringType { get; }

        #endregion

        #region members

        /// <inheritdoc />
        public override IViToken With(int start, int length, string containingText) =>
            new ViEnumToken(start, length, containingText, this.EnumName, this.DeclaringType, this.EnumType);

        /// <summary>
        /// Crate a <see cref="ViStringToken"/> where the containingText start with a double quotes and ends with a double quotes.
        /// </summary>
        /// <param name="start">The start index.</param>
        /// <param name="length">The length of the token.</param>
        /// <param name="enumName">The enum name.</param>
        /// <param name="declaringType">The declaring Type if many they will be separated by a full stop.</param>
        /// <param name="enumType">Optional enum type.</param>
        /// <returns>A new instance of <see cref="ViStringToken"/>.</returns>
        public static ViEnumToken CreateWithoutWhitespaces(
            int start,
            int length,
            string enumName,
            Maybe<string> declaringType,
            Maybe<string> enumType) =>
            new(
                start,
                length,
                enumName,
                enumName,
                declaringType,
                enumType);

        #endregion
    }
}
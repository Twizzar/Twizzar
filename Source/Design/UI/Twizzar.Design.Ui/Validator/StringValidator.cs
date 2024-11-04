using System.Linq;
using System.Threading.Tasks;
using Twizzar.Design.Ui.Helpers;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Keywords;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.Ui.Validator
{
    /// <summary>
    /// Validator for strings.
    /// </summary>
    public sealed class StringValidator : BaseTypeValidator
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="StringValidator"/> class.
        /// </summary>
        /// <param name="typeDescription">The type description.</param>
        public StringValidator(IBaseDescription typeDescription)
            : base(typeDescription)
        {
            this.ValidInput = this.GetInputValues().Append(KeyWords.Null);
        }

        #endregion

        #region members

        /// <inheritdoc />
        protected override string Name => "string";

        /// <inheritdoc />
        protected override Task<IViToken> ValidateAsync(IViToken token) =>
            token switch
            {
                IViStringToken x =>
                    Task.FromResult<IViToken>(x),

                IViNullKeywordToken x =>
                    Task.FromResult<IViToken>(x),

                _ => base.ValidateAsync(token),
            };

        #endregion
    }
}
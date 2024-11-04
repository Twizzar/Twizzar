using System.Threading.Tasks;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.Ui.Validator
{
    /// <summary>
    /// Validator for chars.
    /// </summary>
    public class CharValidator : BaseTypeValidator
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="CharValidator"/> class.
        /// </summary>
        /// <param name="typeDescription">The type description.</param>
        public CharValidator(IBaseDescription typeDescription)
            : base(typeDescription)
        {
            this.ValidInput = this.GetInputValues();
        }

        #endregion

        #region properties

        /// <inheritdoc />
        protected override string Name => this.TypeDescription.IsNullableBaseType ? "nullable char" : "char";

        #endregion

        #region members

        /// <inheritdoc />
        protected override Task<IViToken> ValidateAsync(IViToken token) =>
            token switch
            {
                IViCharToken x =>
                    Task.FromResult<IViToken>(x),

                _ => base.ValidateAsync(token),
            };

        #endregion
    }
}
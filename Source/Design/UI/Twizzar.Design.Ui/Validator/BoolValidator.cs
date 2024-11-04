using System.Collections.Generic;
using System.Threading.Tasks;
using Twizzar.Design.Ui.Helpers;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Keywords;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals;
using Twizzar.Design.Ui.Parser.SyntaxTree;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.Ui.Validator
{
    /// <summary>
    /// Validator for booleans.
    /// </summary>
    public class BoolValidator : BaseTypeValidator
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="BoolValidator"/> class.
        /// </summary>
        /// <param name="typeDescription">The type description.</param>
        public BoolValidator(IBaseDescription typeDescription)
            : base(typeDescription)
        {
            this.ValidInput = this.GetInputValues();
        }

        #endregion

        #region properties

        /// <inheritdoc />
        protected override string Name => this.TypeDescription.IsNullableBaseType ? "nullable boolean" : "boolean";

        #endregion

        #region members

        #region Overrides of BaseTypeValidator

        /// <inheritdoc />
        protected sealed override IEnumerable<string> GetInputValues()
        {
            if (this.TypeDescription.IsNullableBaseType)
            {
                yield return KeyWords.Null;
            }

            yield return "true";
            yield return "false";

            if (this.DefaultValue == KeyWords.Undefined)
            {
                yield return this.DefaultValue;
            }
        }

        #endregion

        /// <inheritdoc />
        protected override Task<IViToken> ValidateAsync(IViToken token) =>
            token switch
            {
                IViBoolToken x =>
                    Task.FromResult<IViToken>(x),
                IViUniqueKeywordToken x =>
                    Task.FromResult<IViToken>(
                        new ViInvalidToken(
                            x.Start,
                            x.Length,
                            x.ContainingText,
                            $"{KeyWords.Unique} is not valid for boolean.")),

                _ => base.ValidateAsync(token),
            };

        #endregion
    }
}
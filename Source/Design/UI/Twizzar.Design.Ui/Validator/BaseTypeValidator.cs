using System.Collections.Generic;
using System.Threading.Tasks;
using Twizzar.Design.Ui.Helpers;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Keywords;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.Ui.Validator
{
    /// <summary>
    /// A base type validator.
    /// </summary>
    public abstract class BaseTypeValidator : TypeValidator
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTypeValidator"/> class.
        /// </summary>
        /// <param name="typeDescription">The type description.</param>
        protected BaseTypeValidator(IBaseDescription typeDescription)
            : base(typeDescription)
        {
            this.Initialized();
        }

        #endregion

        #region members

        /// <summary>
        /// Get the input values.
        /// </summary>
        /// <returns>All valid input values.</returns>
        protected virtual IEnumerable<string> GetInputValues()
        {
            if (this.TypeDescription.IsNullableBaseType)
            {
                yield return KeyWords.Null;
            }

            yield return KeyWords.Unique;
            if (this.DefaultValue == KeyWords.Undefined)
            {
                yield return this.DefaultValue;
            }
        }

        /// <inheritdoc />
        protected override Task<IViToken> ValidateAsync(IViToken token) =>
            token switch
            {
                IViUniqueKeywordToken x =>
                    Task.FromResult<IViToken>(x),

                _ => base.ValidateAsync(token),
            };

        #endregion
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals;
using Twizzar.Design.Ui.Parser.SyntaxTree;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Ui.Validator
{
    /// <summary>
    /// Validator for numeric.
    /// </summary>
    public class NumericValidator : BaseTypeValidator
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericValidator"/> class.
        /// </summary>
        /// <param name="typeDescription">The type description.</param>
        public NumericValidator(IBaseDescription typeDescription)
            : base(typeDescription)
        {
            this.ValidInput = this.GetInputValues();
        }

        #endregion

        #region properties

        /// <inheritdoc />
        protected override string Name => this.TypeDescription.IsNullableBaseType ? "nullable numeric" : "numeric";

        #endregion

        #region members

        /// <inheritdoc/>
        public override IViToken Prettify(IViToken token) =>
            token switch
            {
                IViNumericToken x =>
                    this.Prepare(x),
                _ => token,
            };

        /// <inheritdoc />
        protected override Task<IViToken> ValidateAsync(IViToken token) =>
            token switch
            {
                IViNumericToken x =>
                    this.ValidateNumericTokenAsync(x),

                _ => base.ValidateAsync(token),
            };

        private Task<IViToken> ValidateNumericTokenAsync(IViNumericToken token)
        {
            if (token.NumericWithSuffix.Suffix.Match(c => this.GetAllowedSuffixes().Contains(c), true))
            {
                return Task.FromResult<IViToken>(token);
            }

            return Task.FromResult<IViToken>(
                        new ViInvalidToken(
                            token.Start,
                            token.Length,
                            token.ContainingText,
                            $"Suffix {token.NumericWithSuffix.Suffix} is not valid for {this.TypeDescription.TypeFullName.FullName}."));
        }

        private IViToken Prepare(IViNumericToken token)
        {
            if (token.NumericWithSuffix.Suffix.IsNone)
            {
                return this.AppendMissingSuffix(token);
            }

            return token;
        }

        private IViToken AppendMissingSuffix(IViNumericToken token)
        {
            return token.With(token.NumericWithSuffix with { Suffix = this.GetMandatorySuffix() });
        }

        private Maybe<char> GetMandatorySuffix()
        {
            Maybe<char> suffix = Maybe.None();

            var typeFullName = this.TypeDescription.TypeFullName.FullName;

            if (typeFullName == typeof(float).FullName)
            {
                suffix = 'f';
            }
            else if (typeFullName == typeof(decimal).FullName)
            {
                suffix = 'm';
            }

            return suffix;
        }

        private IEnumerable<char> GetAllowedSuffixes()
        {
            var typeFullName = this.TypeDescription.TypeFullName.FullName;

            if (typeFullName == typeof(float).FullName)
            {
                yield return 'f';
                yield return 'F';
            }
            else if (typeFullName == typeof(double).FullName)
            {
                yield return 'd';
                yield return 'D';
            }
            else if (typeFullName == typeof(decimal).FullName)
            {
                yield return 'm';
                yield return 'M';
            }
        }

        #endregion
    }
}
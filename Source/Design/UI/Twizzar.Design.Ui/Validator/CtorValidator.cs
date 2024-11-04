using System.Threading.Tasks;
using Twizzar.Design.Ui.Helpers;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Parser.SyntaxTree;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

#pragma warning disable SA1509 // Opening braces should not be preceded by blank line

namespace Twizzar.Design.Ui.Validator
{
    /// <summary>
    /// Validator for constructors.
    /// </summary>
    public sealed class CtorValidator : BaseValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CtorValidator"/> class.
        /// </summary>
        /// <param name="ctorMethodDescription"></param>
        public CtorValidator(IBaseDescription ctorMethodDescription)
            : base(ctorMethodDescription)
        {
            this.Initialized();
        }

        #region members

        /// <inheritdoc />
        public override string DefaultValue => KeyWords.Undefined;

        /// <inheritdoc />
        public override string Tooltip =>
            ((IMethodDescription)this.TypeDescription)?
            .FriendlyParameterFullTypes
            .ToDisplayString(string.Empty, "(", ")");

        /// <inheritdoc />
        protected override Task<IViToken> ValidateAsync(IViToken token) =>
            token switch
            {
                IViCtorToken x =>
                    Task.FromResult<IViToken>(x),

                { } other =>
                    Task.FromResult<IViToken>(
                        new ViInvalidToken(
                            other.Start,
                            other.Length,
                            other.ContainingText,
                            $"Only {nameof(IViCtorToken)} allowed")),
            };

        #endregion
    }
}
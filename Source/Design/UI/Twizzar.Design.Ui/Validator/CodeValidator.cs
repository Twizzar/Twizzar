using System.Threading.Tasks;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Parser.SyntaxTree;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Ui.Validator
{
    /// <summary>
    /// Validator for code.
    /// </summary>
    public class CodeValidator : BaseValidator
    {
        private readonly IMemberConfiguration _memberConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeValidator"/> class.
        /// </summary>
        /// <param name="typeDescription"></param>
        /// <param name="memberConfiguration"></param>
        public CodeValidator(IBaseDescription typeDescription, IMemberConfiguration memberConfiguration)
            : base(typeDescription)
        {
            this.EnsureMany()
                .Parameter(typeDescription, nameof(typeDescription))
                .Parameter(memberConfiguration, nameof(memberConfiguration))
                .ThrowWhenNull();

            this.Initialized();
            this._memberConfiguration = memberConfiguration;
        }

        #region members

        /// <inheritdoc />
        public override string DefaultValue => string.Empty;

        /// <inheritdoc />
        public override string Tooltip =>
            $"Manually configured by the user via the source code. Value cannot be adjusted via the UI. {this._memberConfiguration.Source}.";

        /// <inheritdoc />
        protected override Task<IViToken> ValidateAsync(IViToken token) =>
            token switch
            {
                IViCodeToken x =>
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
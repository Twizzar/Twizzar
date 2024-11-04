using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Validator;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Parser.SyntaxTree;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Ui.Validator
{
    /// <summary>
    /// Base class for validators.
    /// </summary>
    public abstract class BaseValidator : IValidator
    {
        #region fields

        private IEnumerable<string> _validInput = Enumerable.Empty<string>();

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseValidator"/> class.
        /// </summary>
        /// <param name="typeDescription"></param>
        protected BaseValidator(IBaseDescription typeDescription)
        {
            this.EnsureParameter(typeDescription, nameof(typeDescription))
                .ThrowWhenNull();

            this.TypeDescription = typeDescription;
        }

        #endregion

        #region events

        /// <inheritdoc />
        public event Action OnInitialized;

        /// <inheritdoc />
        public event Action<IEnumerable<string>> OnValidInputsChanged;

        #endregion

        #region properties

        /// <inheritdoc cref="IHasEnsureHelper" />
        public ILogger Logger { get; set; }

        /// <inheritdoc cref="IHasEnsureHelper"/>
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> ValidInput
        {
            get => this._validInput;
            protected set
            {
                this._validInput = value;
                this.OnValidInputsChanged?.Invoke(value);
            }
        }

        /// <inheritdoc />
        public abstract string DefaultValue { get; }

        /// <inheritdoc />
        public virtual string Tooltip { get; protected set; } = null;

        /// <inheritdoc />
        public virtual string AdornerText { get; protected set; }

        /// <inheritdoc />
        public IBaseDescription TypeDescription { get; }

        #endregion

        #region members

        /// <inheritdoc />
        public async Task<IViToken> ValidateAsync(IResult<IViToken, ParseFailure> token)
        {
            this.Tooltip = null;
            this.AdornerText = null;

            return token.AsResultValue() switch
            {
                SuccessValue<IViToken> successValue => await this.ValidateAsync(successValue.Value),

                FailureValue<ParseFailure> failureValue =>
                    new ViInvalidToken(
                        failureValue.Value.Span.Start.Position,
                        failureValue.Value.Span.Length,
                        failureValue.Value.Span.Content,
                        failureValue.Value.Message),

                _ => throw new PatternErrorBuilder(nameof(token)).IsNotOneOf(
                    nameof(SuccessValue<IViToken>),
                    nameof(FailureValue<ParseFailure>)),
            };
        }

        /// <inheritdoc />
        public virtual IViToken Prettify(IViToken token) => token;

        /// <inheritdoc />
        public virtual Task InitializeAsync() => Task.CompletedTask;

        /// <summary>
        /// Mark as initialized.
        /// </summary>
        protected void Initialized() => this.OnInitialized?.Invoke();

        /// <summary>
        /// Checks if the type is nullable.
        /// </summary>
        /// <param name="typeDescription">The base type description.</param>
        /// <returns>True when it can be null; else false.</returns>
        protected virtual bool IsNullable(IBaseDescription typeDescription) => typeDescription.IsNullableBaseType;

        /// <summary>
        /// ValidateAsync a single token.
        /// </summary>
        /// <param name="token">The token to validate.</param>
        /// <returns>A new <see cref="ItemValueSegment"/>.</returns>
        protected abstract Task<IViToken> ValidateAsync(IViToken token);

        #endregion
    }
}
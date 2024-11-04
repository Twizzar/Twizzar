using System.Threading.Tasks;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Ui.Helpers;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Keywords;
using Twizzar.Design.Ui.Parser.SyntaxTree;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

#pragma warning disable SA1509 // Opening braces should not be preceded by blank line

namespace Twizzar.Design.Ui.Validator
{
    /// <summary>
    /// Parser for a type.
    /// </summary>
    public abstract class TypeValidator : BaseValidator
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeValidator"/> class.
        /// </summary>
        /// <param name="typeDescription">The type description.</param>
        protected TypeValidator(IBaseDescription typeDescription)
            : base(typeDescription)
        {
            this.Initialized();
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public override string DefaultValue =>
            this.TypeDescription switch
            {
                IPropertyDescription _ => KeyWords.Undefined,
                IFieldDescription _ => KeyWords.Undefined,
                IMethodDescription _=> KeyWords.Undefined,
                IMemberDescription _ => KeyWords.Undefined,

                // for types and parameters
                { } desc when desc.IsBaseType || desc.IsNullableBaseType => BaseTypeDefault(desc),
                { IsBaseType: false, IsNullableBaseType: false } => this.TypeDescription.TypeFullName.GetFriendlyCSharpTypeName(),
                _ => KeyWords.Undefined,
            };

        /// <summary>
        /// Gets the name of the type for providing better failure messages.
        /// </summary>
        protected abstract string Name { get; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override Task<IViToken> ValidateAsync(IViToken token) =>
            token switch
            {
                ViEmptyToken x =>
                    Task.FromResult<IViToken>(x),

                IViNullKeywordToken x when this.TypeDescription.IsNullableBaseType =>
                    Task.FromResult<IViToken>(x),

                IViUndefinedKeywordToken x when this.TypeDescription is IMemberDescription =>
                    Task.FromResult<IViToken>(x),

                { } other =>
                    Task.FromResult<IViToken>(
                        new ViInvalidToken(
                            other.Start,
                            other.Length,
                            other.ContainingText,
                            $"{other.GetType().Name} is not valid for a {this.Name}.")),
            };

        private static string BaseTypeDefault(IBaseDescription baseDescription)
        {
            // get typeFullname or underlying nullable type or original one.
            var typeFullName = baseDescription.TypeFullName
                .NullableGetUnderlyingType()
                .SomeOrProvided(baseDescription.TypeFullName);

            return typeFullName.Equals(TypeFullName.CreateFromType(typeof(bool))) ? "true" : KeyWords.Unique;
        }

        #endregion
    }
}
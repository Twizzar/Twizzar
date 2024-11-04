using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals;
using Twizzar.Design.Ui.Parser.SyntaxTree;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.Ui.Validator
{
    /// <summary>
    /// Validator for enums.
    /// </summary>
    public class EnumValidator : BaseTypeValidator
    {
        #region fields

        private readonly HashSet<string> _enumValues;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumValidator"/> class.
        /// </summary>
        /// <param name="typeDescription"></param>
        public EnumValidator(IBaseDescription typeDescription)
            : base(typeDescription)
        {
            this._enumValues = typeDescription
                .GetReturnTypeDescription()
                .GetEnumNames()
                .SomeOrProvided(Array.Empty<string>())
                .ToHashSet();

            this.ValidInput = this.GetInputValues()
                .Concat(this._enumValues);
        }

        #endregion

        #region properties

        /// <inheritdoc />
        protected override string Name => "enum";

        #endregion

        #region members

        /// <inheritdoc />
        protected override Task<IViToken> ValidateAsync(IViToken token)
        {
            switch (token)
            {
                case IViEnumToken x:
                    var enumType = this.TypeDescription.TypeFullName.GetFriendlyCSharpTypeName();

                    // Enum type does not match
                    if (x.EnumType.Map(s => s.Trim() != this.TypeDescription.TypeFullName.GetFriendlyCSharpTypeName())
                        .SomeOrProvided(false))
                    {
                        return Task.FromResult<IViToken>(
                            new ViInvalidToken(
                                x.Start,
                                x.Length,
                                x.ContainingText,
                                $"EnumType {enumType} expected but got {x.EnumType.GetValueUnsafe()}"));
                    }

                    // enum value is not valid
                    if (!this._enumValues.Contains(x.EnumName))
                    {
                        return Task.FromResult<IViToken>(
                            new ViInvalidToken(
                                x.Start,
                                x.Length,
                                x.ContainingText,
                                $"The value {x.EnumName} is not valid for the enum {this.TypeDescription.TypeFullName.GetFriendlyCSharpFullName()}"));
                    }

                    return Task.FromResult<IViToken>(x);
                default:
                    return base.ValidateAsync(token);
            }
        }

        #endregion
    }
}
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.CoreInterfaces.Resources;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Ui.Helpers;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Keywords;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Link;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.Design.Ui.Parser.SyntaxTree;
using Twizzar.Design.Ui.Parser.SyntaxTree.Link;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Ui.Validator
{
    /// <summary>
    /// Validator for complex types.
    /// </summary>
    public class ComplexValidator : TypeValidator
    {
        #region fields

        private readonly IFixtureItemInformation _fixtureItemInformation;
        private readonly IAssignableTypesQuery _assignableTypesQuery;

        private ImmutableDictionary<string, IBaseDescription[]> _descriptionsByAssignableName =
            ImmutableDictionary<string, IBaseDescription[]>.Empty;

        private Maybe<IBaseDescription> _lastDescription;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexValidator"/> class.
        /// </summary>
        /// <param name="typeDescription"></param>
        /// <param name="fixtureItemInformation"></param>
        /// <param name="assignableTypesQuery"></param>
        public ComplexValidator(
            IBaseDescription typeDescription,
            IFixtureItemInformation fixtureItemInformation,
            IAssignableTypesQuery assignableTypesQuery)
            : base(typeDescription)
        {
            this.EnsureMany()
                .Parameter(fixtureItemInformation, nameof(fixtureItemInformation))
                .Parameter(assignableTypesQuery, nameof(assignableTypesQuery))
                .ThrowWhenNull();

            this._fixtureItemInformation = fixtureItemInformation;
            this._assignableTypesQuery = assignableTypesQuery;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public override string Tooltip { get; protected set; }

        /// <inheritdoc />
        public override string AdornerText { get; protected set; }

        /// <inheritdoc />
        protected override string Name => this.TypeDescription.TypeFullName.GetTypeName();

        #endregion

        #region members

        /// <summary>
        /// Loads the assignable types.
        /// </summary>
        /// <returns>async task.</returns>
        public override async Task InitializeAsync()
        {
            var builder = ImmutableArray.CreateBuilder<string>();

            if (!this.TypeDescription.IsStruct)
            {
                builder.Add(KeyWords.Null);
            }

            var assignableTypes = (await this._assignableTypesQuery
                    .GetAssignableTypesAsync(this.TypeDescription))
                .ToList();

            builder.AddRange(assignableTypes.Select(t => t.TypeFullName.GetFriendlyCSharpFullName()).Distinct());

            this._descriptionsByAssignableName = assignableTypes
                .GroupBy(t => t.TypeFullName.GetFriendlyCSharpTypeName())
                .ToImmutableDictionary(names => names.Key, names => names.ToArray());

            this.ValidInput = builder.ToImmutable();
            this.Initialized();
        }

        /// <inheritdoc />
        public override IViToken Prettify(IViToken token) =>
            token switch
            {
                IViLinkToken x =>
                    this.Prepare(x),
                _ => token,
            };

        /// <inheritdoc />
        protected override Task<IViToken> ValidateAsync(IViToken token) =>
            token switch
            {
                ViEmptyToken x =>
                    this.ValidateEmptyTokenAsync(x),

                IViNullKeywordToken x =>
                    Task.FromResult<IViToken>(x),

                IViLinkToken x =>
                    this.ValidateLinkAsync(x),

                _ => base.ValidateAsync(token),
            };

        private Task<IViToken> ValidateEmptyTokenAsync(ViEmptyToken viEmptyToken)
        {
            if (this.DefaultValue != KeyWords.Undefined)
            {
                this.Tooltip = this.GetToolTip(this.TypeDescription);
                this.AdornerText = GetAdornerText(this.TypeDescription);
            }

            return Task.FromResult<IViToken>(viEmptyToken);
        }

        private async Task<IViToken> ValidateLinkAsync(IViLinkToken token)
        {
            if (token.TypeToken.AsMaybeValue() is not SomeValue<IViTypeToken> some)
            {
                // no type available, only link name defined
                return token;
            }

            var result = await this.FindAssignableTypeAsync(some.Value.TypeFullNameToken);
            this._lastDescription = result;
            result.IfSome(
                description =>
                {
                    this.Tooltip = this.GetToolTip(description);
                    this.AdornerText = GetAdornerText(description);
                });

            return result.Match<IViToken>(
                name => token.WithTypeToken(some.Value.WithToken(name.TypeFullName.GetTypeFullNameToken())),
                () => new ViInvalidToken(
                    token.Start,
                    token.Length,
                    token.ContainingText,
                    $"{some.Value.TypeFullNameToken.ToFriendlyCSharpTypeName()} is not assignable to {this.TypeDescription.TypeFullName}."));
        }

        private static string GetAdornerText(IBaseDescription description) =>
            description.IsInterface
                ? MessagesDesign.Adorner_Interface
                : description.IsClass
                    ? MessagesDesign.Adorner_Class
                    : description.IsStruct
                        ? MessagesDesign.Adorner_Struct
                        : string.Empty;

        private string GetToolTip(IBaseDescription description)
        {
            var fullname = description.TypeFullName.GetFriendlyCSharpFullName();
            var postfix = this.TypeDescription is IParameterDescription
                ? MessagesDesign.ToolTip_Postfix
                : string.Empty;

            return description.IsInterface
                ? string.Format(MessagesDesign.ToolTip_WithoutLinkWithIsInterfaceType, fullname, postfix)
                : string.Format(MessagesDesign.ToolTip_WithoutLinkWithNotIsInterfaceType, fullname, postfix);
        }

        private async Task<Maybe<IBaseDescription>> FindAssignableTypeAsync(ITypeFullNameToken typeFullNameToken)
        {
            var maybeTypeFullNames =
                this._descriptionsByAssignableName.GetMaybe(typeFullNameToken.ToFriendlyCSharpTypeName());

            return maybeTypeFullNames.AsMaybeValue() switch
            {
                SomeValue<IBaseDescription[]> typeFullNames when typeFullNames.Value.Length == 1 =>
                    Maybe.Some(typeFullNames.Value.First()),

                SomeValue<IBaseDescription[]> typeFullNames when typeFullNames.Value.Length > 1 =>
                    this.TryExtractDescription(typeFullNameToken, typeFullNames.Value),

                _ => await this._assignableTypesQuery.IsAssignableTo(
                    this.TypeDescription,
                    TypeFullName.CreateFromToken(typeFullNameToken),
                    this._fixtureItemInformation.Id.RootItemPath),
            };
        }

        /// <summary>
        /// Try to find a matching type full name in typeFullNames;
        /// else check for matching type name in _lastTypeFullName.
        /// If booth fail return None.
        /// </summary>
        private Maybe<IBaseDescription> TryExtractDescription(
            ITypeFullNameToken typeFullNameToken,
            IEnumerable<IBaseDescription> typeFullNames) =>
                typeFullNames.FirstOrNone(name => name.TypeFullName.FullName.Equals(typeFullNameToken.ToFullName()))
                    .BindNone(
                        () =>
                            this._lastDescription.AsMaybeValue() switch
                            {
                                SomeValue<TypeFullName> typeFullName
                                    when AreFriendlyCSharpNamesEqual(typeFullNameToken, typeFullName) =>
                                    this._lastDescription,
                                _ => Maybe.None(),
                            });

        private static bool AreFriendlyCSharpNamesEqual(
            ITypeFullNameToken typeFullNameToken,
            SomeValue<TypeFullName> typeFullName) =>
            typeFullNameToken.ToFriendlyCSharpTypeName() ==
            typeFullName.Value.TypeFullNameToken.ToFriendlyCSharpTypeName();

        private IViToken Prepare(IViLinkToken token)
        {
            // if no type is specified
            if (token.TypeToken.AsMaybeValue() is not SomeValue<IViTypeToken> someTypeToken)
            {
                return token.WithTypeToken(
                    ViTypeToken.Create(this.TypeDescription.TypeFullName.GetTypeFullNameToken()));
            }

            var typeToken = someTypeToken.Value;

            var maybeTypeFullNames =
                this._descriptionsByAssignableName.GetMaybe(typeToken.TypeFullNameToken.ToFriendlyCSharpTypeName());

            // if the type name was found in assignable types
            if (maybeTypeFullNames.AsMaybeValue() is SomeValue<IBaseDescription[]> descriptions)
            {
                // if there is only one type for this type name
                if (descriptions.Value.Length == 1)
                {
                    var typeFullName = descriptions.Value.First().TypeFullName;
                    return token.WithTypeToken(ViTypeToken.Create(typeFullName.GetTypeFullNameToken()));
                }
                else
                {
                    var match = descriptions.Value.FirstOrNone(
                        description => Equals(description.TypeFullName.GetTypeFullNameToken(), typeToken.TypeFullNameToken));

                    // the input token is a type full name and was found in the assignable types
                    // so we can identify uniquely the type provided.
                    return match.Match<IViToken>(
                        some: description => token.WithTypeToken(ViTypeToken.Create(description.TypeFullName.GetTypeFullNameToken())),
                        none: () => new ViInvalidToken(
                            token.Start,
                            token.Length,
                            token.ContainingText,
                            $"{typeToken.TypeFullNameToken.ToFriendlyCSharpTypeName()} is not assignable to {this.TypeDescription.TypeFullName}."));
                }
            }

            // if the type name was not found in assignable types
            else
            {
                // This case can occur when the assignable types are not initialized.
                // We expect that the token was validated before so the type should be valid.
                // Therefore we only trim the full type name to a type name.
                var typeName = someTypeToken.Value.TypeFullNameToken.ToFriendlyCSharpTypeName();

                typeToken =
                    typeToken.With(typeToken.Start, typeToken.Start + typeName.Length, typeName) as IViTypeToken;

                return token.WithTypeToken(typeToken);
            }
        }

        #endregion
    }
}
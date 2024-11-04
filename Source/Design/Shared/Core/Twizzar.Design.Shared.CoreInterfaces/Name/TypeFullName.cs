using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Shared.CoreInterfaces.Name
{
    /// <summary>
    /// Class for the type full name.
    /// </summary>
    public class TypeFullName : ValueObject, ITypeFullName
    {
        #region fields

        private readonly Lazy<IResult<ITypeFullNameToken, ParseFailure>> _cachedToken;

        #endregion

        #region ctors

        private TypeFullName(string fullName)
        {
            this.FullName = fullName;
            this._cachedToken = new Lazy<IResult<ITypeFullNameToken, ParseFailure>>(this.ParseToken);
        }

        private TypeFullName(ITypeFullNameToken token)
        {
            this.FullName = token.ToFullName();

            this._cachedToken = new Lazy<IResult<ITypeFullNameToken, ParseFailure>>(
                () =>
                    Result.Success<ITypeFullNameToken, ParseFailure>(token));
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the full name.
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// Gets the type full name token.
        /// </summary>
        public ITypeFullNameToken TypeFullNameToken => this.GetTokenUnsafe();

        /// <summary>
        /// Gets the underlying token.
        /// </summary>
        public ITypeFullNameToken Token => this.GetTokenUnsafe();

        #endregion

        #region members

        /// <summary>
        /// Cast the typeFullName to a string.
        /// </summary>
        /// <param name="tFullName">The typeFullName.</param>
        public static implicit operator string(TypeFullName tFullName) =>
            tFullName?.FullName;

        /// <summary>
        /// Create a <see cref="TypeFullName"/> from a string.
        /// </summary>
        /// <param name="fullName">string of the full name.</param>
        /// <returns>A new TypeFullName.</returns>
        public static TypeFullName Create(string fullName)
        {
            EnsureHelper.GetDefault.Parameter(fullName, nameof(fullName))
                .IsNotNull()
                .ThrowOnFailure();

            return new TypeFullName(fullName);
        }

        /// <summary>
        /// Create a <see cref="TypeFullName"/> from a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A new TypeFullName.</returns>
        public static TypeFullName CreateFromType(Type type)
        {
            EnsureHelper.GetDefault.Parameter(type, nameof(type))
                .IsNotNull()
                .ThrowOnFailure();

            if (!string.IsNullOrEmpty(type.FullName))
            {
                return new TypeFullName(type.FullName);
            }

            if (type.IsNested && type.DeclaringType != null)
            {
                return new TypeFullName(type.DeclaringType.FullName + "+" + type.Name);
            }

            return new TypeFullName(type.Namespace + "." + type.Name);
        }

        /// <summary>
        /// Create a <see cref="TypeFullName"/> from a <see cref="ITypeFullNameToken"/>.
        /// </summary>
        /// <param name="token">The type full name.</param>
        /// <returns>A new TypeFullName.</returns>
        public static TypeFullName CreateFromToken(ITypeFullNameToken token)
        {
            EnsureHelper.GetDefault.Parameter(token, nameof(token))
                .IsNotNull()
                .ThrowOnFailure();

            return new TypeFullName(token);
        }

        /// <summary>
        /// Get the namespace of the full type.
        /// </summary>
        /// <returns>The namespace as a string.</returns>
        public string GetNameSpace()
        {
            var ns = this.GetTokenUnsafe().Namespace;
            return ns.Length > 0
                ? ns.Remove(ns.Length - 1)
                : ns;
        }

        /// <summary>
        /// Gets the declaring type if the type is nested with a leading <c>.</c>,
        /// if the type is not nested this will return an empty string.
        /// </summary>
        /// <returns></returns>
        public string GetFriendlyDeclaringType() =>
            this.GetTokenUnsafe()
                .FriendlyDeclaringType;

        /// <summary>
        /// Gets the namespace and the Declaring type.
        /// </summary>
        /// <returns>The namespace and the declaring type separated with a .</returns>
        public string GetNameSpaceWithDeclaringType()
        {
            var ns = this.Token.ToNamespaceWithDeclaringType();
            return ns.Length > 0
                ? ns.Remove(ns.Length - 1)
                : ns;
        }

        /// <inheritdoc />
        public string GetTypeName() =>
            this.GetTokenUnsafe().Typename +
            this.GetTokenUnsafe().GenericPostfix.SomeOrProvided(string.Empty);

        /// <inheritdoc />
        public ImmutableArray<ITypeFullName> GenericTypeArguments() =>
            this.GetTokenUnsafe()
                .GenericTypeArguments
                .Select(token => (ITypeFullName)new TypeFullName(token))
                .ToImmutableArray();

        /// <summary>
        /// Gets the type full name without the generic part.
        /// </summary>
        /// <returns>Success when the parse was successful; otherwise a parse failure.</returns>
        public string GetTypeFullNameWithoutGenerics() =>
            this.GetTokenUnsafe().Namespace +
            this.GetTokenUnsafe().DeclaringType.SomeOrProvided(string.Empty) +
            this.GetTokenUnsafe().Typename +
            this.GetTokenUnsafe().GenericPostfix.SomeOrProvided(string.Empty);

        /// <summary>
        /// Tries to parse the type fullname to an friendly csharp name.
        /// </summary>
        /// <returns>On parse success the name; else a parse failure.</returns>
        public string GetFriendlyCSharpFullName() =>
            this.GetTokenUnsafe().ToFriendlyCSharpFullTypeName();

        /// <summary>
        /// Tries to parse the type fullname to an friendly csharp type name.
        /// </summary>
        /// <returns>On parse success the type name; else a parse failure.</returns>
        public string GetFriendlyCSharpTypeName() =>
            this.GetTokenUnsafe().ToFriendlyCSharpTypeName();

        /// <inheritdoc/>
        public string GetFriendlyCSharpTypeFullName() =>
            this.GetTokenUnsafe().ToFriendlyCSharpFullTypeName();

        /// <inheritdoc />
        public bool Equals(ITypeFullName other) =>
            this.FullName == other?.FullName;

        /// <inheritdoc />
        public override string ToString() =>
            this.FullName;

        /// <inheritdoc />
        public bool IsNullable() =>
            this.GetTypeName().StartsWith("Nullable", StringComparison.InvariantCulture);

        /// <summary>
        /// Determines whether the given TypeFullName is an array.
        /// </summary>
        /// <returns>True if it is a array type.</returns>
        public bool IsArray() =>
            !this.GetTokenUnsafe().ArrayBrackets.IsEmpty;

        /// <summary>
        /// Gets the array structure.
        /// </summary>
        /// <returns>The array dimension.</returns>
        public ImmutableArray<int> ArrayDimension() =>
            this.GetTokenUnsafe().ArrayDimension;

        /// <summary>
        /// Gets a maybe of the array element type.
        /// </summary>
        /// <returns>None if the type is not an array and the element type if it is.</returns>
        public Maybe<TypeFullName> ArrayElementType() =>
            this.GetTokenUnsafe()
                .ArrayElementType
                .Map(Create);

        /// <inheritdoc />
        public Maybe<ITypeFullName> NullableGetUnderlyingType()
        {
            if (this.IsNullable())
            {
                var genericParameters = this.GetTokenUnsafe().GenericTypeArguments;

                if (genericParameters.Length == 1)
                {
                    return Create(genericParameters[0].ToFullName());
                }
            }

            return Maybe.None();
        }

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.FullName;
        }

        private IResult<ITypeFullNameToken, ParseFailure> ParseToken() =>
            TypeFullNameParser.MetaTypeParser.And(Consume.EOF, (token, unit) => token)
                .Parse(this.FullName)
                .MapSuccess(success => success.Value);

        private ITypeFullNameToken GetTokenUnsafe() =>
            this._cachedToken.Value.Match(
                token => token,
                failure => throw new InternalException(
                    $"The type: {this.FullName} cannot be parsed: {failure.Message}"));

        #endregion
    }
}
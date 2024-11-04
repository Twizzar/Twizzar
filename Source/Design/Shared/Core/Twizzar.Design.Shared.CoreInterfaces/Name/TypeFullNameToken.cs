using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Shared.CoreInterfaces.Name
{
    /// <summary>
    /// TypeFull name token represent a parse type full name.
    /// </summary>
    public class TypeFullNameToken : ValueObject, ITypeFullNameToken
    {
        #region ctors

        /// <summary>
        /// Type name without array brackets (if any available).
        /// </summary>
        private readonly string _typeNameOnly;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeFullNameToken"/> class.
        /// </summary>
        /// <param name="ns">The namespace part this should end with a dot. For example <c>System.Collections.Immutable.</c>.</param>
        /// <param name="typeName">The type name without some generic modifiers like <c>´1</c>.</param>
        /// <param name="declaringType">The outer type which declares the nested inner type. With a + at the end.</param>
        /// <param name="genericPostfix">The generic postfix to the type name for example <c>´1</c>.</param>
        /// <param name="genericParameters">The generic type arguments.</param>
        /// <param name="containingText"></param>
        /// <param name="arrayBrackets"></param>
        public TypeFullNameToken(
            string ns,
            string typeName,
            Maybe<string> declaringType,
            Maybe<string> genericPostfix,
            ImmutableArray<ITypeFullNameToken> genericParameters,
            ImmutableArray<string> arrayBrackets,
            string containingText)
            : this(
                ns,
                typeName,
                declaringType,
                genericPostfix,
                Maybe.None(),
                genericParameters,
                arrayBrackets,
                containingText)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeFullNameToken"/> class.
        /// </summary>
        /// <param name="ns">The namespace part this should end with a dot. For example <c>System.Collections.Immutable.</c>.</param>
        /// <param name="typeName">The type name without some generic modifiers like <c>´1</c>.</param>
        /// <param name="declaringType">the outer type which declares the nested inner type. With a + at the end.</param>
        /// <param name="genericPostfix">The generic postfix to the type name for example <c>´1</c>.</param>
        /// <param name="containingAssembly">The containing assembly with a leading comma for example: ,mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089.</param>
        /// <param name="genericTypeArguments">The generic type arguments.</param>
        /// <param name="containingText">The parsed text.</param>
        /// <param name="arrayBrackets"></param>
        private TypeFullNameToken(
            string ns,
            string typeName,
            Maybe<string> declaringType,
            Maybe<string> genericPostfix,
            Maybe<string> containingAssembly,
            ImmutableArray<ITypeFullNameToken> genericTypeArguments,
            ImmutableArray<string> arrayBrackets,
            string containingText)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(ns, nameof(ns))
                .Parameter(typeName, nameof(typeName))
                .Parameter(containingText, nameof(containingText))
                .Parameter(genericTypeArguments, nameof(genericTypeArguments))
                .Parameter(arrayBrackets, nameof(arrayBrackets))
                .ThrowWhenNull();

            this._typeNameOnly = typeName;
            this.Namespace = ns;
            this.DeclaringType = declaringType;
            this.GenericPostfix = genericPostfix;
            this.ContainingAssembly = containingAssembly;
            this.GenericTypeArguments = genericTypeArguments;
            this.ContainingText = containingText;
            this.ArrayBrackets = arrayBrackets;

            this.ArrayDimension = this.ArrayBrackets.Select(arr => arr.Split(',').Length).ToImmutableArray();
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ImmutableArray<int> ArrayDimension { get; }

        /// <inheritdoc />
        public Maybe<string> DeclaringType { get; }

        /// <inheritdoc />
        public string FriendlyDeclaringType => this.DeclaringType
            .Map(s => s.Replace("+", "."))
            .SomeOrProvided(string.Empty);

        /// <inheritdoc />
        public ImmutableArray<string> ArrayBrackets { get; }

        /// <inheritdoc />
        public string ContainingText { get; }

        /// <inheritdoc />
        public string Namespace { get; }

        /// <inheritdoc />
        public string Typename => $"{this._typeNameOnly}{this.ArrayBrackets.ToDisplayString(string.Empty)}";

        /// <inheritdoc />
        public Maybe<string> ArrayElementType =>
            this.ArrayBrackets.Length > 0
                ? Maybe.Some($"{this.Namespace}{this.DeclaringType.SomeOrProvided(string.Empty)}{this._typeNameOnly}{this.ToGenericPostfix()}{this.GenericArguments()}")
                : Maybe.None();

        /// <inheritdoc />
        public Maybe<string> GenericPostfix { get; }

        /// <inheritdoc />
        public Maybe<string> ContainingAssembly { get; }

        /// <inheritdoc />
        public ImmutableArray<ITypeFullNameToken> GenericTypeArguments { get; }

        #endregion

        #region members

        /// <inheritdoc />
        public ITypeFullNameToken AddContainingAssembly(string containingAssembly) =>
            new TypeFullNameToken(
                this.Namespace,
                this._typeNameOnly,
                this.DeclaringType,
                this.GenericPostfix,
                containingAssembly,
                this.GenericTypeArguments,
                this.ArrayBrackets,
                this.ContainingText);

        /// <inheritdoc />
        public string ToNamespaceWithDeclaringType() =>
            $"{this.Namespace}{this.DeclaringType.Map(s => s.Replace("+", ".")).SomeOrProvided(string.Empty)}";

        /// <inheritdoc />
        public string ToFriendlyCSharpFullTypeName() =>
            $"{this.ToNamespaceWithDeclaringType()}{this._typeNameOnly}{this.FriendlyTypeParameters(token => token.ToFriendlyCSharpFullTypeName())}{this.ArrayBrackets.ToDisplayString(string.Empty)}";

        /// <inheritdoc />
        public string ToFriendlyCSharpTypeName() =>
            $"{this._typeNameOnly}{this.FriendlyTypeParameters(token => token.ToFriendlyCSharpTypeName())}{this.ArrayBrackets.ToDisplayString(string.Empty)}";

        /// <inheritdoc />
        public string ToNameWithGenericPostfix() =>
            this.Typename + this.ToGenericPostfix();

        /// <inheritdoc />
        public string ToFullName(bool withContainingAssembly = false)
            => withContainingAssembly
                ? $"{this.Namespace}{this.DeclaringType.SomeOrProvided(string.Empty)}{this._typeNameOnly}{this.ToGenericPostfix()}{this.GenericArguments()}{this.ArrayBrackets.ToDisplayString(string.Empty)}{this.ToContainingAssembly()}"
                : $"{this.Namespace}{this.DeclaringType.SomeOrProvided(string.Empty)}{this._typeNameOnly}{this.ToGenericPostfix()}{this.GenericArguments()}{this.ArrayBrackets.ToDisplayString(string.Empty)}";

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.ToFullName().Trim();
        }

        private string GenericArguments() =>
            this.GenericTypeArguments.IsEmpty
                ? string.Empty
                : this.GenericTypeArguments
                    .Select(token => $"[{token.ToFullName(true)}]")
                    .ToDisplayString(",", "[", "]");

        private string ToGenericPostfix() =>
            this.GenericPostfix.SomeOrProvided(string.Empty);

        private string ToContainingAssembly() =>
            this.ContainingAssembly.SomeOrProvided(string.Empty);

        private string FriendlyTypeParameters(Func<ITypeFullNameToken, string> getName) =>
            this.GenericTypeArguments.IsEmpty
                ? string.Empty
                : this.GenericTypeArguments
                    .Select(getName)
                    .ToDisplayString(", ", "<", ">");

        #endregion
    }
}
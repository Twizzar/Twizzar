using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Shared.Infrastructure.VisualStudio2019.Name;

/// <inheritdoc cref="ITypeFullNameToken" />
public record SymbolTypeFullNameToken : ITypeFullNameToken
{
    #region fields

    private readonly Lazy<ImmutableArray<ITypeFullNameToken>> _genericTypeArgumentsLazy;
    private readonly Lazy<string> _friendlyDeclaringType;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolTypeFullNameToken"/> class.
    /// </summary>
    /// <param name="symbol"></param>
    public SymbolTypeFullNameToken(ITypeSymbol symbol)
    {
        if (symbol is IArrayTypeSymbol arraySymbol)
        {
            var arrayInfo = GetArrayInformation(arraySymbol).ToArray();

            this.Symbol = arrayInfo.Last().ElementType;
            this.ArrayElementType = arraySymbol.ElementType.ToString();
            this.ArrayDimension = arrayInfo.Select(tuple => tuple.Dimension).ToImmutableArray();
            this.ArrayBrackets = arrayInfo.Select(tuple => tuple.Bracket).ToImmutableArray();
        }
        else
        {
            this.Symbol = symbol;
            this.ArrayElementType = Maybe.None<string>();
            this.ArrayDimension = ImmutableArray<int>.Empty;
            this.ArrayBrackets = ImmutableArray<string>.Empty;
        }

        this.Namespace = this.Symbol.ContainingNamespace?.IsGlobalNamespace == true
            ? string.Empty
            : this.Symbol.ContainingNamespace + ".";

        this.GenericPostfix = this.Symbol is INamedTypeSymbol { TypeArguments.Length: > 0 } nSymbol
            ? $"`{nSymbol.TypeArguments.Length}"
            : string.Empty;

        this.ContainingAssembly = this.Symbol.ContainingAssembly?.ToString() ?? Maybe.None<string>();

        this.DeclaringType = this.Symbol.ContainingType != null
            ? $"{this.Symbol.ContainingType.MetadataName}+"
            : string.Empty;

        this._friendlyDeclaringType = new Lazy<string>(
            () => this.Symbol switch
            {
                { TypeKind: TypeKind.TypeParameter } => string.Empty,
                _ when this.Symbol?.ContainingType is not null =>
                    new SymbolTypeFullNameToken(this.Symbol.ContainingType)
                        .ToFriendlyCsharpTypeNameWithFullNameGenerics() + ".",
                _ => string.Empty,
            });
        this.ContainingText = this.Symbol.MetadataName;

        this._genericTypeArgumentsLazy = new Lazy<ImmutableArray<ITypeFullNameToken>>(
            () =>
                this.Symbol is INamedTypeSymbol namedTypeSymbol
                    ? namedTypeSymbol.TypeArguments
                        .Select(s => (ITypeFullNameToken)new SymbolTypeFullNameToken(s))
                        .ToImmutableArray()
                    : ImmutableArray<ITypeFullNameToken>.Empty);

        this.Typename =
            $"{this.Symbol.Name}{this.ArrayBrackets.ToDisplayString(string.Empty)}";
    }

    #endregion

    #region properties

    /// <inheritdoc/>
    public string Namespace { get; init; }

    /// <inheritdoc/>
    public string Typename { get; init; }

    /// <inheritdoc/>
    public Maybe<string> GenericPostfix { get; init; }

    /// <inheritdoc/>
    public Maybe<string> ContainingAssembly { get; init; }

    /// <inheritdoc/>
    public ImmutableArray<string> ArrayBrackets { get; init; }

    /// <inheritdoc/>
    public ImmutableArray<int> ArrayDimension { get; init; }

    /// <inheritdoc/>
    public Maybe<string> DeclaringType { get; init; }

    /// <inheritdoc/>
    public string FriendlyDeclaringType => this._friendlyDeclaringType.Value;

    /// <inheritdoc/>
    public string ContainingText { get; init; }

    /// <inheritdoc/>
    public Maybe<string> ArrayElementType { get; init; }

    /// <inheritdoc/>
    public ImmutableArray<ITypeFullNameToken> GenericTypeArguments => this._genericTypeArgumentsLazy.Value;

    /// <summary>
    /// Gets the underlying symbol.
    /// </summary>
    public ITypeSymbol Symbol { get; init; }

    #endregion

    #region members

    /// <inheritdoc />
    public virtual bool Equals(SymbolTypeFullNameToken other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.ArrayBrackets.Equals(other.ArrayBrackets) &&
               this.ArrayDimension.Equals(other.ArrayDimension) &&
               this.ContainingAssembly.Equals(other.ContainingAssembly) &&
               this.ContainingText == other.ContainingText &&
               this.GenericTypeArguments.Equals(other.GenericTypeArguments) &&
               this.Namespace == other.Namespace &&
               this.Typename == other.Typename;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = this.ArrayBrackets.GetHashCode();
            hashCode = (hashCode * 397) ^ this.ArrayDimension.GetHashCode();
            hashCode = (hashCode * 397) ^ this.ContainingAssembly.GetHashCode();
            hashCode = (hashCode * 397) ^ (this.ContainingText != null ? this.ContainingText.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ this.GenericTypeArguments.GetHashCode();
            hashCode = (hashCode * 397) ^ (this.Namespace != null ? this.Namespace.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (this.Typename != null ? this.Typename.GetHashCode() : 0);
            return hashCode;
        }
    }

    /// <inheritdoc/>
    public string ToFriendlyCSharpFullTypeName() =>
        $"{this.ToNamespaceWithDeclaringType()}{this.Symbol.Name}{this.FriendlyTypeParameters(token => token.ToFriendlyCSharpFullTypeName())}{this.ArrayBrackets.ToDisplayString(string.Empty)}";

    /// <inheritdoc/>
    public string ToFriendlyCSharpTypeName() =>
        $"{this.Symbol.Name}{this.FriendlyTypeParameters(token => token.ToFriendlyCSharpTypeName())}{this.ArrayBrackets.ToDisplayString(string.Empty)}";

    /// <inheritdoc />
    public string ToNameWithGenericPostfix() =>
        this.Typename + this.ToGenericPostfix();

    /// <summary>
    /// Gets the Type with declaring types if the type is nested and
    /// if the type is generic get the generic type parameters as a their full type name.
    /// </summary>
    /// <returns></returns>
    private string ToFriendlyCsharpTypeNameWithFullNameGenerics() =>
        $"{this._friendlyDeclaringType.Value}{this.Symbol.Name}{this.FriendlyTypeParameters(token => token.ToFriendlyCSharpFullTypeName())}{this.ArrayBrackets.ToDisplayString(string.Empty)}";

    /// <inheritdoc/>
    public string ToFullName(bool withContainingAssembly = false) =>
        withContainingAssembly
            ? $"{this.Namespace}{this.DeclaringType.SomeOrProvided(string.Empty)}{this.Symbol.Name}{this.ToGenericPostfix()}{this.GenericArguments()}{this.ArrayBrackets.ToDisplayString(string.Empty)}, {this.ToContainingAssembly()}"
            : $"{this.Namespace}{this.DeclaringType.SomeOrProvided(string.Empty)}{this.Symbol.Name}{this.ToGenericPostfix()}{this.GenericArguments()}{this.ArrayBrackets.ToDisplayString(string.Empty)}";

    /// <inheritdoc/>
    public string ToNamespaceWithDeclaringType() =>
        $"{this.Namespace}{this._friendlyDeclaringType.Value}";

    /// <inheritdoc/>
    public ITypeFullNameToken AddContainingAssembly(string containingAssembly) =>
        this;

    private static IEnumerable<(string Bracket, int Dimension, ITypeSymbol ElementType)> GetArrayInformation(
        IArrayTypeSymbol arraySymbol)
    {
        var brackets = "[" +
                       new string(Enumerable.Repeat(',', arraySymbol.Rank - 1).ToArray()) +
                       "]";

        var dimension = arraySymbol.Rank;
        var elementType = arraySymbol.ElementType;

        yield return (brackets, dimension, elementType);

        if (arraySymbol.ElementType is IArrayTypeSymbol innerArrayType)
        {
            foreach (var infos in GetArrayInformation(innerArrayType))
            {
                yield return infos;
            }
        }
    }

    private static ITypeSymbol GetArrayElementType(IArrayTypeSymbol arraySymbol) =>
        arraySymbol.ElementType is IArrayTypeSymbol innerArraySymbol
            ? GetArrayElementType(innerArraySymbol)
            : arraySymbol.ElementType;

    private string ToContainingAssembly() =>
        this.ContainingAssembly.SomeOrProvided(string.Empty);

    private string ToGenericPostfix() =>
        this.GenericPostfix.SomeOrProvided(string.Empty);

    private string GenericArguments() =>
        this.GenericTypeArguments.IsEmpty
            ? string.Empty
            : this.GenericTypeArguments
                .Select(token => $"[{token.ToFullName(true)}]")
                .ToDisplayString(",", "[", "]");

    private string FriendlyTypeParameters(Func<ITypeFullNameToken, string> getName) =>
        this.GenericTypeArguments.IsEmpty
            ? string.Empty
            : this.GenericTypeArguments
                .Select(getName)
                .ToDisplayString(", ", "<", ">");

    #endregion
}
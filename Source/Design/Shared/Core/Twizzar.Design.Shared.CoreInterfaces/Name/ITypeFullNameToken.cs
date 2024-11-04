using System.Collections.Immutable;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Shared.CoreInterfaces.Name
{
    /// <summary>
    /// TypeFull name token represent a parse type full name.
    /// </summary>
    public interface ITypeFullNameToken
    {
        /// <summary>
        /// Gets the namespace part this should end with a dot. For example <c>System.Collections.Immutable.</c>.
        /// </summary>
        string Namespace { get; }

        /// <summary>
        /// Gets the type name without some generic modifiers like <c>´1</c>.
        /// </summary>
        string Typename { get; }

        /// <summary>
        /// Gets the generic postfix to the type name for example <c>´1</c>.
        /// </summary>
        Maybe<string> GenericPostfix { get; }

        /// <summary>
        /// Gets the containing assembly with a leading comma for example: ,mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089.
        /// </summary>
        Maybe<string> ContainingAssembly { get; }

        /// <summary>
        /// Gets the generic type arguments.
        /// </summary>
        ImmutableArray<ITypeFullNameToken> GenericTypeArguments { get; }

        /// <summary>
        /// Gets the array brackets if this type is an array.
        /// </summary>
        ImmutableArray<string> ArrayBrackets { get; }

        /// <summary>
        /// Gets the structure of the array.
        /// </summary>
        ImmutableArray<int> ArrayDimension { get; }

        /// <summary>
        /// Gets the outer type which declares the nested inner type. With a + at the end.
        /// </summary>
        Maybe<string> DeclaringType { get; }

        /// <summary>
        /// Gets the all the outer types which declares the nested inner type as an friendly representation.
        /// All type separated with a <c>.</c> and with a tailing <c></c>.
        /// When the type is not nested this returns an empty string.
        /// </summary>
        string FriendlyDeclaringType { get; }

        /// <summary>
        /// Gets the parsed text.
        /// </summary>
        string ContainingText { get; }

        /// <summary>
        /// Gets the array element type or none, if TypeFullName is not an array.
        /// </summary>
        Maybe<string> ArrayElementType { get; }

        /// <summary>
        /// Add a containing assembly and returns a new <see cref="ITypeFullNameToken"/>.
        /// </summary>
        /// <param name="containingAssembly">he containing assembly for example: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089.</param>
        /// <returns>A new instance of <see cref="ITypeFullNameToken"/>.</returns>
        ITypeFullNameToken AddContainingAssembly(string containingAssembly);

        /// <summary>
        /// Return the namespace and the containing type.
        /// </summary>
        /// <example>Com.Company.OuterClass.</example>
        /// <returns>The namespace with the containing type without the +.</returns>
        string ToNamespaceWithDeclaringType();

        /// <summary>
        /// Returns a friendly c# name. For System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
        /// this will be System.Nullable&lt;int&gt;.
        /// </summary>
        /// <returns>The name as an string.</returns>
        string ToFriendlyCSharpFullTypeName();

        /// <summary>
        /// Returns a friendly c# type name. For System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
        /// this will be Nullable&lt;int&gt;.
        /// </summary>
        /// <returns>The name as an string.</returns>
        string ToFriendlyCSharpTypeName();

        /// <summary>
        /// Gets the name with the generic postfix.
        /// <example>
        /// List`1
        /// </example>
        /// </summary>
        /// <returns></returns>
        string ToNameWithGenericPostfix();

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <param name="withContainingAssembly">True when the containing assembly should be appended.</param>
        /// <returns>This will return the same like calling <c>typeof(type).FullName</c>.</returns>
        string ToFullName(bool withContainingAssembly = false);
    }
}
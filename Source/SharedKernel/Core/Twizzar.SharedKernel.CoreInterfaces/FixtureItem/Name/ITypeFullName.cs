using System;
using System.Collections.Immutable;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name
{
    /// <summary>
    /// The twizzar representation of a type full name.
    /// </summary>
    public interface ITypeFullName : IEquatable<ITypeFullName>
    {
        /// <summary>
        /// Gets the full name.
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Get the name of the full type.
        /// </summary>
        /// <returns>The type name as a string.</returns>
        string GetTypeName();

        /// <summary>
        /// Check if the type is a nullable struct.
        /// </summary>
        /// <returns>True if its a nullable struct; else false.</returns>
        bool IsNullable();

        /// <summary>
        /// Get the generic type arguments.
        /// </summary>
        /// <returns>An immutable array with new typeFullNames.</returns>
        ImmutableArray<ITypeFullName> GenericTypeArguments();

        /// <summary>
        /// Gets the underlying type of a nullable. For Nullable&lt;System.Int32&gt; this would be System.Int32.
        /// </summary>
        /// <returns>A new <see cref="ITypeFullName"/> of the underlying type.</returns>
        Maybe<ITypeFullName> NullableGetUnderlyingType();

        /// <summary>
        /// Tries to parse the type name to an friendly csharp type name.
        /// </summary>
        /// <returns>On parse success the type name; else a parse failure.</returns>
        public string GetFriendlyCSharpTypeName();

        /// <summary>
        /// Tries to parse the type fullname to an friendly csharp type name.
        /// </summary>
        /// <returns>On parse success the type name; else a parse failure.</returns>
        public string GetFriendlyCSharpTypeFullName();

        /// <summary>
        /// Gets the declaring type if the type is nested with a leading <c>.</c>,
        /// if the type is not nested this will return an empty string.
        /// </summary>
        /// <returns></returns>
        public string GetFriendlyDeclaringType();
    }
}
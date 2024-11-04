using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Name
{
    /// <summary>
    /// Type full name implementation for the runtime.
    /// </summary>
    public class TypeFullName : ValueObject, ITypeFullName
    {
        #region ctors

        private TypeFullName(Type type)
        {
            EnsureHelper.GetDefault.Parameter(type, nameof(type)).ThrowWhenNull();
            this.FullName = GetFullName(type);
            this.Type = type;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the reflection type.
        /// </summary>
        public Type Type { get; }

        /// <inheritdoc />
        public string FullName { get; }

        #endregion

        #region members

        /// <summary>
        /// Create a new TypeFullName.
        /// </summary>
        /// <param name="type">The reflection type.</param>
        /// <returns>A new instance of <see cref="TypeFullName"/>.</returns>
        public static TypeFullName Create(Type type) => new(type);

        /// <inheritdoc />
        public string GetTypeName() => this.Type.Name;

        /// <inheritdoc />
        public bool IsNullable() => this.Type.IsNullableType();

        /// <inheritdoc />
        public ImmutableArray<ITypeFullName> GenericTypeArguments() =>
            this.Type.GenericTypeArguments.Select(Create).Cast<ITypeFullName>().ToImmutableArray();

        /// <inheritdoc />
        public Maybe<ITypeFullName> NullableGetUnderlyingType() =>
            Maybe.ToMaybe(Nullable.GetUnderlyingType(this.Type))
                .Map<ITypeFullName>(Create);

        /// <inheritdoc />
        public string GetFriendlyCSharpTypeName() => GetFriendlyCSharpTypeName(this.Type);

        /// <inheritdoc />
        public string GetFriendlyCSharpTypeFullName() =>
            $"{this.Type.Namespace}.{this.GetFriendlyDeclaringType()}{this.GetFriendlyCSharpTypeName()}";

        /// <inheritdoc />
        public bool Equals(ITypeFullName other) => this.FullName == other?.FullName;

        /// <inheritdoc />
        public string GetFriendlyDeclaringType() =>
            this.Type.DeclaringType != null ? this.Type.DeclaringType.Name + "." : string.Empty;

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.FullName;
        }

        private static string GetFullName(Type type)
        {
            if (!string.IsNullOrEmpty(type.FullName))
            {
                return type.FullName;
            }

            if (type.IsNested && type.DeclaringType != null)
            {
                return type.DeclaringType.FullName + "+" + type.Name;
            }

            return type.Namespace + "." + type.Name;
        }

        private static string GetFriendlyCSharpTypeName(Type type) =>
            type.IsGenericType
                ? type.Name.Split('`')[0] +
                  "<" +
                  string.Join(", ", type.GetGenericArguments().Select(GetFriendlyCSharpTypeName).ToArray()) +
                  ">"
                : type.Name;

        #endregion
    }
}
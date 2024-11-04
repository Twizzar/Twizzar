using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Fixture.Member
{
    /// <inheritdoc cref="IMemberPath{TFixtureItem}" />
    [SuppressMessage(
        "Major Code Smell",
        "S4035:Classes implementing \"IEquatable<T>\" should be sealed",
        Justification = "Equality will be the same for all implementations.")]
    public abstract class MemberPath<TFixtureItem> : IMemberPath<TFixtureItem>, IEquatable<MemberPath<TFixtureItem>>
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberPath{TFixtureItem}"/> class.
        /// </summary>
        /// <param name="viMemberName">The member name.</param>
        /// <param name="viReturnType">The member return type.</param>
        /// <param name="viParent">The parent.</param>
        private protected MemberPath(string viMemberName, Type viReturnType, Maybe<MemberPath<TFixtureItem>> viParent)
        {
            this.ViMemberName = viMemberName ?? throw new ArgumentNullException(nameof(viMemberName));
            this.ViReturnType = viReturnType ?? throw new ArgumentNullException(nameof(viReturnType));
            this.ViParent = viParent;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        string IMemberPath<TFixtureItem>.UniqueMemberName =>
            (this is IMethodMemberPath<TFixtureItem> methodPath)
                ? $"{methodPath.MemberName}{GetMethodPostfix(methodPath)}"
                : this.ViMemberName;

        /// <inheritdoc/>
        string IMemberPath<TFixtureItem>.Name => this.ViName;

        /// <inheritdoc />
        string IMemberPath<TFixtureItem>.UniqueName => this.ViUniqueName;

        /// <inheritdoc/>
        string IMemberPath<TFixtureItem>.MemberName => this.ViMemberName;

        /// <inheritdoc/>
        Type IMemberPath<TFixtureItem>.ReturnType => this.ViReturnType;

        /// <inheritdoc/>
        Type IMemberPath<TFixtureItem>.DeclaredType => this.ViDeclaredType;

        /// <inheritdoc/>
        Maybe<IMemberPath<TFixtureItem>> IMemberPath<TFixtureItem>.Parent =>
            this.ViParent.Map(path => (IMemberPath<TFixtureItem>)path);

        /// <summary>
        /// Gets the parent of this member.
        /// </summary>
        protected MemberPath<TFixtureItem> TzParent => this.ViParent.GetValueUnsafe();

        /// <summary>
        /// Gets the declared type.
        /// </summary>
        protected abstract Type ViDeclaredType { get; }

        /// <summary>
        /// Gets the name of the path.
        /// </summary>
        private string ViName =>
            this.ViParent
                .Map(path => path)
                .Map(path => $"{path.ViName}.")
                .SomeOrProvided(string.Empty) +
            this.ViMemberName;

        private string ViUniqueName =>
            this.ViParent
                .Map(path => path)
                .Map(path => $"{path.ViName}.")
                .SomeOrProvided(string.Empty) +
            ((IMemberPath<TFixtureItem>)this).UniqueMemberName;

        private string ViMemberName { get; }

        private Type ViReturnType { get; }

        private Maybe<MemberPath<TFixtureItem>> ViParent { get; }

        #endregion

        #region members

        /// <summary>
        /// Check if tow paths are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>True if they are equal; otherwise false.</returns>
        public static bool operator ==(MemberPath<TFixtureItem> left, MemberPath<TFixtureItem> right) =>
            Equals(left, right);

        /// <summary>
        /// Check if tow paths are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>True if they are not equal; otherwise false.</returns>
        public static bool operator !=(MemberPath<TFixtureItem> left, MemberPath<TFixtureItem> right) =>
            !Equals(left, right);

        /// <inheritdoc />
        public override string ToString() => this.ViUniqueName;

        /// <inheritdoc />
        public bool Equals(MemberPath<TFixtureItem> other)
        {
            if (other is null)
            {
                return false;
            }

            return ReferenceEquals(this, other) || Equals(this.ViName, other.ViName);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == this.GetType() && this.Equals((MemberPath<TFixtureItem>)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() => this.ViName != null ? this.ViName.GetHashCode() : 0;

        private static string GetMethodPostfix(IMethodMemberPath<TFixtureItem> methodPath)
        {
            var genericArgumentsString = GetGenericArgumentsString(methodPath);
            var parameterString = GetParameterString(methodPath);

            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(genericArgumentsString))
            {
                sb.Append(genericArgumentsString);
            }

            if (!string.IsNullOrEmpty(parameterString))
            {
                sb.Append($"__{parameterString}");
            }

            return sb.ToString();
        }

        private static string GetGenericArgumentsString(IMethodMemberPath<TFixtureItem> methodPath) =>
            methodPath.GenericArguments.Select(argument => argument.Name).ToDisplayString(string.Empty).ToSourceVariableCodeFriendly();

        private static string GetParameterString(IMethodMemberPath<TFixtureItem> methodPath) =>
            methodPath.Parameters.Select(type => type.DeclaredTypeName).ToDisplayString("_").ToSourceVariableCodeFriendly();

        #endregion
    }
}
using System.Collections.Generic;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using ViCommon.Functional.Monads.MaybeMonad;

#pragma warning disable CS1591, S107, SA1600

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// Discovers the attributes of a property and provides access to property metadata.
    /// </summary>
    public abstract class PropertyDescription : BaseTypeDescription, IPropertyDescription
    {
        #region ctors

        protected PropertyDescription(IBaseTypeService baseTypeService)
            : base(baseTypeService)
        {
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public string Name { get; protected set; }

        /// <inheritdoc />
        public abstract ITypeDescription DeclaredDescription { get; }

        /// <inheritdoc />
        public bool CanRead { get; protected set; }

        /// <inheritdoc />
        public bool CanWrite { get; protected set; }

        /// <inheritdoc />
        public abstract bool IsStatic { get; }

        /// <inheritdoc />
        public abstract Maybe<IMethodDescription> SetMethod { get; }

        /// <inheritdoc />
        public abstract Maybe<IMethodDescription> GetMethod { get; }

        /// <inheritdoc />
        public abstract OverrideKind OverrideKind { get; }

        #endregion

        #region members

        /// <inheritdoc />
        public override string ToString()
        {
            var staticStr = this.IsStatic ? " static" : string.Empty;
            var getStr = this.CanRead ? " get;" : string.Empty;
            var setStr = this.CanWrite ? " set;" : string.Empty;

            return $"{this.AccessModifier}{staticStr} {this.TypeFullName?.GetFriendlyCSharpTypeName()} {this.Name} {{ {getStr}{setStr}}}";
        }

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.AccessModifier;
            yield return this.TypeFullName;
            yield return this.Name;
            yield return this.CanRead;
            yield return this.CanWrite;
            yield return this.IsStatic;
            yield return this.SetMethod;
            yield return this.GetMethod;
            yield return this.OverrideKind;
        }

        #endregion
    }
}
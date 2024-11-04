using System.Collections.Generic;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using ViCommon.Functional.Monads.MaybeMonad;

#pragma warning disable CS1591, S107, SA1600

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// Discovers the attributes of a field and provides access to field metadata.
    /// </summary>
    public abstract class FieldDescription : BaseTypeDescription, IFieldDescription
    {
        #region ctors

        protected FieldDescription(IBaseTypeService baseTypeService)
            : base(baseTypeService)
        {
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ITypeFullName DeclaringType { get; protected set; }

        /// <inheritdoc />
        public bool IsStatic { get; protected set; }

        /// <inheritdoc />
        public bool IsConstant { get; protected set; }

        /// <inheritdoc />
        public bool IsReadonly { get; protected set; }

        /// <inheritdoc />
        public Maybe<object> ConstantValue { get; protected set; }

        /// <inheritdoc />
        public string Name { get; protected set; }

        /// <inheritdoc />
        public abstract ITypeDescription DeclaredDescription { get; }

        #endregion

        #region members

        /// <inheritdoc />
        public override string ToString()
        {
            var constStr = this.IsConstant ? "const" : string.Empty;
            var readonlyStr = this.IsReadonly ? "readonly" : string.Empty;
            var staticStr = this.IsStatic ? "static" : string.Empty;
            var constValue = this.ConstantValue.Map(o => $" = {o}").SomeOrProvided(string.Empty);

            return
                $"{this.AccessModifier} {staticStr} {constStr}{readonlyStr} {this.TypeFullName?.GetFriendlyCSharpTypeName()} {this.Name}{constValue}";
        }

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.AccessModifier;
            yield return this.TypeFullName;
            yield return this.Name;
            yield return this.IsConstant;
            yield return this.IsReadonly;
            yield return this.IsStatic;
            yield return this.ConstantValue;
        }

        #endregion
    }
}
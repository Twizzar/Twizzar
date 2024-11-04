using System.Collections.Generic;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using ViCommon.Functional.Monads.MaybeMonad;
using static System.String;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// Gets the attributes for this parameter.
    /// </summary>
    public abstract class ParameterDescription : BaseTypeDescription, IParameterDescription
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterDescription"/> class.
        /// </summary>
        /// <param name="baseTypeService"></param>
        protected ParameterDescription(IBaseTypeService baseTypeService)
            : base(baseTypeService)
        {
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public string Name { get; protected set; }

        /// <inheritdoc />
        public Maybe<ParameterDefaultValue> DefaultValue { get; protected set; }

        /// <inheritdoc />
        public bool IsIn { get; protected set; }

        /// <inheritdoc />
        public bool IsOptional { get; protected set; }

        /// <inheritdoc />
        public bool IsOut { get; protected set; }

        /// <inheritdoc />
        public int Position { get; protected set; }

        #endregion

        #region members

        /// <inheritdoc />
        public override string ToString()
        {
            var inStr = this.IsIn ? " in" : Empty;
            var outStr = this.IsOut ? " out" : Empty;

            var optionalStr = this.IsOptional
                ? $" = {this.DefaultValue}"
                : Empty;

            return $"{this.TypeFullName?.GetFriendlyCSharpTypeName()}{inStr}{outStr} {this.Name}{optionalStr}";
        }

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.AccessModifier;
            yield return this.Name;
            yield return this.TypeFullName;
            yield return this.DefaultValue;
            yield return this.IsIn;
            yield return this.IsOptional;
            yield return this.IsOut;
            yield return this.Position;
        }

        #endregion
    }
}
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations
{
    /// <summary>
    /// Wrapper for enum classes.
    /// </summary>
    /// <param name="TypeFullName">Gets the Enum class type full name.</param>
    /// <param name="Value">Gets the value of the enum.</param>
    public record EnumValue(ITypeFullName TypeFullName, string Value)
    {
        #region members

        /// <inheritdoc />
        public override string ToString() =>
            $"{this.TypeFullName.GetFriendlyDeclaringType()}{this.TypeFullName.GetFriendlyCSharpTypeName()}.{this.Value}";

        #endregion
    }
}
using System.Collections.Generic;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations
{
    /// <summary>
    /// Configuration item which describes member configured to return a single value.
    /// </summary>
    public sealed record ValueMemberConfiguration : MemberConfiguration<ValueMemberConfiguration>
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueMemberConfiguration"/> class.
        /// </summary>
        /// <param name="name">The name of the member.</param>
        /// <param name="value">The value of the member.</param>
        /// <param name="source"></param>
        public ValueMemberConfiguration(string name, object value, IConfigurationSource source)
            : base(name, source)
        {
            EnsureHelper.GetDefault.Parameter(value, nameof(value)).IsNotNull().ThrowOnFailure();

            this.Value = value;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Value { get; init; }

        /// <summary>
        /// Gets the display value.
        /// </summary>
        public string DisplayValue =>
            this.Value switch
            {
                string s => $"\"{s}\"",
                char c => $"'{c}'",
                EnumValue e => e.ToString(),
                SimpleLiteralValue s => s.ToString(),
                object o => o.ToString(),
            };

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetAdditionalEqualityComponents()
        {
            yield return this.Value;
        }

        #endregion
    }
}
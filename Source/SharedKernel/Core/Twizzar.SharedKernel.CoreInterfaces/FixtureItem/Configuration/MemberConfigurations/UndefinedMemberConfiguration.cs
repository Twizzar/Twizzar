using System.Collections.Generic;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations
{
    /// <summary>
    /// Configuration item for a member which is undefined.
    /// </summary>
    public sealed record UndefinedMemberConfiguration : MemberConfiguration<UndefinedMemberConfiguration>
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="UndefinedMemberConfiguration"/> class.
        /// </summary>
        /// <param name="name">name of the member.</param>
        /// <param name="type">the type of the member.</param>
        /// <param name="source"></param>
        public UndefinedMemberConfiguration(string name, ITypeFullName type, IConfigurationSource source)
            : base(name, source)
        {
            EnsureHelper.GetDefault.Parameter(type, nameof(type)).ThrowWhenNull();

            this.Name = name;
            this.Type = type;
            this.Source = source;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the type.
        /// </summary>
        public ITypeFullName Type { get; init; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetAdditionalEqualityComponents()
        {
            yield return this.Type;
        }

        #endregion
    }
}
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations
{
    /// <summary>
    /// Configuration item for a member of a class or interface.
    /// </summary>
    public interface IMemberConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets the member name. This should be unique.
        /// For method this will return methodName__allParameterTypesSeparatedByUnderscore.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the name of the path segment, this is usually the same as name but can be if there are some naming conflicts the name with underlines prefixed.
        /// </summary>
        public string MemberPathName { get; }

        /// <summary>
        /// Gets the configuration location.
        /// </summary>
        public IConfigurationSource Source { get; }

        /// <summary>
        /// Create a new member configuration with a different source.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>A new instance of <see cref="IMemberConfiguration"/>.</returns>
        public IMemberConfiguration WithSource(IConfigurationSource source);

        /// <summary>
        /// Create a new member configuration with a different name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IMemberConfiguration WithName(string name);

        /// <summary>
        /// Checks if two member configurations are equals.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="ignoreSource">When true the source will not be compared.</param>
        /// <returns>True if equals; else false.</returns>
        public bool Equals(IMemberConfiguration other, bool ignoreSource);

        /// <summary>
        /// Get the hasCode.
        /// </summary>
        /// <param name="ignoreSource">When true the source will be ignored.</param>
        /// <returns></returns>
        public int GetHashCode(bool ignoreSource);

        #endregion
    }
}

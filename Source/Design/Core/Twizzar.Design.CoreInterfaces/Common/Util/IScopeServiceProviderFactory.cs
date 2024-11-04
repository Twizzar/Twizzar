namespace Twizzar.Design.CoreInterfaces.Common.Util
{
    /// <summary>
    /// Factory for creating a <see cref="IScopedServiceProvider"/>.
    /// </summary>
    public interface IScopeServiceProviderFactory
    {
        /// <summary>
        /// Create a new <see cref="IScopedServiceProvider"/>.
        /// </summary>
        /// <returns>A new <see cref="IScopedServiceProvider"/>.</returns>
        IScopedServiceProvider CreateNew();
    }
}
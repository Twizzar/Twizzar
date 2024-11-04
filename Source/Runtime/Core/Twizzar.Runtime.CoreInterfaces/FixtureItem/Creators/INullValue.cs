namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators
{
    /// <summary>
    /// Marks a return type as null. Used to work around the autofac check if a returned instance is null.
    /// </summary>
    public interface INullValue
    {
        /// <summary>
        /// Converts this marker to null.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>Null.</returns>
        T Convert<T>();
    }
}
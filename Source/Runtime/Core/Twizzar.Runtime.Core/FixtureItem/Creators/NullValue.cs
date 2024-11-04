using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;

namespace Twizzar.Runtime.Core.FixtureItem.Creators
{
    /// <summary>
    /// Marks a return type as null. Used to work around the autofac check if a returned instance is null.
    /// </summary>
    public class NullValue : INullValue
    {
        /// <inheritdoc />
        public T Convert<T>() =>
            default;
    }
}

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators
{
    /// <summary>
    /// Service for creating unique values for the specified enum T.
    /// </summary>
    /// <typeparam name="T">The type of the enum.</typeparam>
    public interface IUniqueEnumCreator<out T> : IUniqueCreator<T>
    {
    }
}
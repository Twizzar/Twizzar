using TwizzarInternal.Fixture;

namespace Twizzar.TestCommon;

/// <summary>
/// Class for easy migration form the old version of twizzar.
/// </summary>
public static class Build
{
    /// <summary>
    /// Create a new instance with twizzar.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T New<T>() =>
        new ItemBuilder<T>().Build();
}
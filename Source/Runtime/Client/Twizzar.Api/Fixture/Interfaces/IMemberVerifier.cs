namespace Twizzar.Fixture;

/// <summary>
/// Service for verifying members.
/// </summary>
public interface IMemberVerifier
{
    /// <summary>
    /// Verify that the select member with the given conditions is called exactly n times.
    /// </summary>
    /// <param name="times">The number of calls expected.</param>
    public void Called(int times);

    /// <summary>
    /// Verify that the select member with the given conditions is called at least once.
    /// </summary>
    public void CalledAtLeastOnce();
}
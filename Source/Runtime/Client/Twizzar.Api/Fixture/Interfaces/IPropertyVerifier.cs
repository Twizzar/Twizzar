namespace Twizzar.Fixture;

/// <summary>
/// Service for further configuration the property verification.
/// </summary>
/// <typeparam name="TDeclaredType">The declared type of the selected property.</typeparam>
public interface IPropertyVerifier<in TDeclaredType>
{
    /// <summary>
    /// Verify the getter of the property.
    /// </summary>
    /// <returns></returns>
    IPropertySetOrGetVerifier Get();

    /// <summary>
    /// Verify the setter with any assigned value.
    /// </summary>
    /// <returns></returns>
    IPropertySetOrGetVerifier Set();

    /// <summary>
    /// Verify the setter when a certain value is assigned.
    /// </summary>
    /// <param name="value">The assigned value expected.</param>
    /// <returns></returns>
    IPropertySetOrGetVerifier Set(TDeclaredType value);
}
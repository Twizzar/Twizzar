namespace Twizzar.Design.CoreInterfaces.Command.Services;

/// <summary>
/// Service for persistence of settings.
/// </summary>
public interface ISettingsWriter
{
    /// <summary>
    /// Set the status of the analytics.
    /// </summary>
    /// <param name="enabled"></param>
    void SetAnalyticsEnabled(bool enabled);
}
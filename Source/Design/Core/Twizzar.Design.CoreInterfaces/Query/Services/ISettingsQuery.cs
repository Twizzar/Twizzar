using Twizzar.Design.CoreInterfaces.Common.VisualStudio;

namespace Twizzar.Design.CoreInterfaces.Query.Services;

/// <summary>
/// Service for getting settings.
/// </summary>
public interface ISettingsQuery : IInitializableService
{
    /// <summary>
    /// Get a value indication whether the analytics service is enabled.
    /// </summary>
    /// <returns></returns>
    bool GetAnalyticsEnabled();
}
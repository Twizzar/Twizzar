using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.TestCreation.Config;

namespace Twizzar.Design.CoreInterfaces.TestCreation;

/// <summary>
/// Service for getting the mapping context.
/// </summary>
public interface IMappingService
{
    /// <summary>
    /// Get the mapping information and add it to the <see cref="CreationInfo"/> and return it.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="config"></param>
    /// <returns>Same <see cref="CreationInfo"/> but the template context was added.</returns>
    Task<CreationInfo> MapAsync(CreationInfo source, TestCreationConfig config);
}
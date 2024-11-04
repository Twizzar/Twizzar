namespace Twizzar.Design.CoreInterfaces.TestCreation.Mapping;

/// <summary>
/// Service for replacing variables in the mapping config with real values.
/// </summary>
public interface IMappingReplacementService
{
    /// <summary>
    /// Get a new <see cref="MappingConfig"/> where variables are replaced by their real value,
    /// found in <see cref="CreationInfo"/>.
    /// </summary>
    /// <param name="config">The mapping raw config.</param>
    /// <param name="creationInfo">Infos which will be used for the replacement.</param>
    /// <returns></returns>
    MappingConfig ReplacePlaceholders(MappingConfig config, CreationInfo creationInfo);
}
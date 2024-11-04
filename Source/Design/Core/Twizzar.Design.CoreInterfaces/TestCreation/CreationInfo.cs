namespace Twizzar.Design.CoreInterfaces.TestCreation;

/// <summary>
/// All important information for creating a unit test.
/// <remarks>All paths are absolute!</remarks>
/// </summary>
/// <param name="Solution">Path to the solution.</param>
/// <param name="Project">Path to the project.</param>
/// <param name="File">Path to the file.</param>
/// <param name="Namespace">Namespace.</param>
/// <param name="Type">The type name.</param>
/// <param name="Member">The member name.</param>
public record CreationInfo(
    string Solution,
    string Project,
    string File,
    string Namespace,
    string Type,
    string Member);
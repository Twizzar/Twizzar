namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn.ConfigWriter;

/// <summary>
/// Represents a request for edit a configuration member.
/// </summary>
public interface IConfigurationMemberEdit
{
    /// <summary>
    /// Gets the member path.
    /// </summary>
    public string MemberPath { get; }
}
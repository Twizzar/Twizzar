using System.Diagnostics.CodeAnalysis;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn.ConfigWriter;

/// <summary>
/// Request to remove the member form the config.
/// </summary>
/// <param name="MemberPath"></param>
[ExcludeFromCodeCoverage]
public record struct RemoveConfigurationMemberEdit(string MemberPath) : IConfigurationMemberEdit;
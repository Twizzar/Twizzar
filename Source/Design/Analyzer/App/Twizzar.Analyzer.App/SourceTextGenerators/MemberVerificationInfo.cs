using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Analyzer.Core.SourceTextGenerators;

/// <summary>
/// Information for creating the WithParamIs extension methods.
/// </summary>
/// <param name="MemberDescription"></param>
/// <param name="MemberPathName"></param>
public record MemberVerificationInfo(IBaseDescription MemberDescription, string MemberPathName);
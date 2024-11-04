using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.Design.CoreInterfaces.TestCreation.Mapping;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Mapping;

/// <inheritdoc cref="IMappingReplacementService" />
public class MappingReplacementService : IMappingReplacementService
{
    #region members

    /// <inheritdoc />
    public MappingConfig ReplacePlaceholders(MappingConfig config, CreationInfo creationInfo) =>
        new(
            ProjectMapping: Replace(config.ProjectMapping, creationInfo),
            ProjectPathMapping: Replace(config.ProjectPathMapping, creationInfo),
            FileMapping: Replace(config.FileMapping, creationInfo),
            FilePathMapping: Replace(config.FilePathMapping, creationInfo),
            NamespaceMapping: Replace(config.NamespaceMapping, creationInfo),
            TypeMapping: Replace(config.TypeMapping, creationInfo),
            MemberMapping: Replace(config.MemberMapping, creationInfo));

    private static ImmutableArray<MappingEntry> Replace(
        IEnumerable<MappingEntry> entries,
        CreationInfo creationInfo) =>
        entries
            .Select(entry => Replace(entry, creationInfo))
            .ToImmutableArray();

    private static MappingEntry Replace(MappingEntry entry, CreationInfo creationInfo) =>
        new(
            Pattern: entry.Pattern.Map(pattern => Replace(pattern, creationInfo)),
            Replacement: Replace(entry.Replacement, creationInfo));

    private static string Replace(string text, CreationInfo creationInfo) =>
        text
            .Replace("$projectUnderTest$", creationInfo.Project.SplitPath().FileName)
            .Replace("$solutionPath$", creationInfo.Solution.SplitPath().Prefix)
            .Replace("$fileUnderTest$", creationInfo.File.SplitPath().FileName)
            .Replace("$typeUnderTest$", creationInfo.Type)
            .Replace("$namespaceUnderTest$", creationInfo.Namespace)
            .Replace("$memberUnderTest$", creationInfo.Member);

    #endregion
}
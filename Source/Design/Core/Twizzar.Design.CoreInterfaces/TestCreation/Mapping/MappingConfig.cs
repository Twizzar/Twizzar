using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.TestCreation.Mapping;

/// <summary>
/// Mapping configuration.
/// </summary>
/// <param name="ProjectMapping"></param>
/// <param name="ProjectPathMapping"></param>
/// <param name="FileMapping"></param>
/// <param name="FilePathMapping"></param>
/// <param name="NamespaceMapping"></param>
/// <param name="TypeMapping"></param>
/// <param name="MemberMapping"></param>
[ExcludeFromCodeCoverage]
public sealed record MappingConfig(
    ImmutableArray<MappingEntry> ProjectMapping,
    ImmutableArray<MappingEntry> ProjectPathMapping,
    ImmutableArray<MappingEntry> FileMapping,
    ImmutableArray<MappingEntry> FilePathMapping,
    ImmutableArray<MappingEntry> NamespaceMapping,
    ImmutableArray<MappingEntry> TypeMapping,
    ImmutableArray<MappingEntry> MemberMapping)
{
    #region members

    /// <inheritdoc />
    public bool Equals(MappingConfig other) =>
        other is not null &&
        this.ProjectMapping.SequenceEqual(other.ProjectMapping) &&
        this.ProjectMapping.SequenceEqual(other.ProjectPathMapping) &&
        this.FileMapping.SequenceEqual(other.FileMapping) &&
        this.FileMapping.SequenceEqual(other.FilePathMapping) &&
        this.NamespaceMapping.SequenceEqual(other.NamespaceMapping) &&
        this.TypeMapping.SequenceEqual(other.TypeMapping) &&
        this.MemberMapping.SequenceEqual(other.MemberMapping);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = this.ProjectMapping.GetHashCodeOfElements();
            hashCode = (hashCode * 397) ^ this.ProjectPathMapping.GetHashCodeOfElements();
            hashCode = (hashCode * 397) ^ this.FileMapping.GetHashCodeOfElements();
            hashCode = (hashCode * 397) ^ this.FilePathMapping.GetHashCodeOfElements();
            hashCode = (hashCode * 397) ^ this.NamespaceMapping.GetHashCodeOfElements();
            hashCode = (hashCode * 397) ^ this.TypeMapping.GetHashCodeOfElements();
            hashCode = (hashCode * 397) ^ this.MemberMapping.GetHashCodeOfElements();
            return hashCode;
        }
    }

    #endregion
}

/// <summary>
/// One patter match entry in the mapping config.
/// </summary>
/// <param name="Pattern"></param>
/// <param name="Replacement"></param>
[ExcludeFromCodeCoverage]
public record MappingEntry(Maybe<string> Pattern, string Replacement);
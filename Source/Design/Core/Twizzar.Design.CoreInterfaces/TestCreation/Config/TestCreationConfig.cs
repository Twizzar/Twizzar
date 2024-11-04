using System.Collections.Immutable;

using Twizzar.Design.CoreInterfaces.TestCreation.Mapping;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.TestCreation.Config;

/// <summary>
/// The configuration for the test creation.
/// </summary>
/// <param name="MappingConfig">All mapping information.</param>
/// <param name="AdditionalNugetPackages">Additional nuget package which will be added to the project.</param>
public record TestCreationConfig(
    MappingConfig MappingConfig,
    ImmutableArray<(string PackageId, Maybe<string> Version)> AdditionalNugetPackages);
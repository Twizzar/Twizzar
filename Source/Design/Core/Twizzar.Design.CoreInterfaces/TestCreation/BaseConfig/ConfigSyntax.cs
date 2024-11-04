using System;
using System.Collections.Immutable;

namespace Twizzar.Design.CoreInterfaces.TestCreation.BaseConfig;

/// <summary>
/// Config syntax parse by the <see cref="IBaseConfigParser"/>.
/// </summary>
/// <param name="ConfigVersion"></param>
/// <param name="Entries"></param>
public record ConfigSyntax(
    Version ConfigVersion,
    IImmutableList<ConfigEntry> Entries);

/// <summary>
/// One entry in the config file.
/// </summary>
/// <param name="Tag">Tag without whitespaces.</param>
/// <param name="Content">Content with whitespaces.</param>
public record ConfigEntry(string Tag, string Content);
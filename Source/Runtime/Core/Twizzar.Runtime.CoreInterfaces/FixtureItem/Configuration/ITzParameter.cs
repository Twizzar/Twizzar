using System;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;

/// <summary>
/// Info about a method parameter.
/// </summary>
public interface ITzParameter
{
    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the declared type name of the parameter.
    /// </summary>
    string DeclaredTypeName { get; }

    /// <summary>
    /// Gets the type of the parameter this can also be object for generics.
    /// </summary>
    Type Type { get; }
}
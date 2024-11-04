using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;

namespace Twizzar.Fixture;

/// <summary>
/// Infos about a method parameter.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class TzParameter : ITzParameter
{
    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="TzParameter"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="declaredTypeName"></param>
    /// <param name="type"></param>
    public TzParameter(string name, string declaredTypeName, Type type)
    {
        this.Name = name;
        this.DeclaredTypeName = declaredTypeName;
        this.Type = type;
    }

    #endregion

    #region properties

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string DeclaredTypeName { get; }

    /// <inheritdoc />
    public Type Type { get; }

    #endregion

    #region members

    /// <summary>
    /// Create an new instance of <see cref="TzParameter"/> where the parameter is non generic.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="type">The parameter type.</param>
    /// <returns></returns>
    public static TzParameter Create(string name, Type type) => new(name, type.Name, type);

    /// <summary>
    /// Create an new instance of <see cref="TzParameter"/> where the parameter is non generic.
    /// </summary>
    /// <param name="propertyInfo">The parameter info.</param>
    /// <returns></returns>
    public static TzParameter Create(PropertyInfo propertyInfo) => Create(propertyInfo.Name, propertyInfo.PropertyType);

    #endregion
}
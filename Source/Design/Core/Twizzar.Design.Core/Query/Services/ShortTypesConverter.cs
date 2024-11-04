using System.Collections.Generic;
using System.Linq;

using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;

using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Core.Query.Services;

/// <inheritdoc cref="IShortTypesConverter" />
public class ShortTypesConverter : IShortTypesConverter
{
    private static readonly Dictionary<string, string> ShortToFull = new()
    {
        { "int", typeof(int).FullName },
        { "uint", typeof(uint).FullName },
        { "long", typeof(long).FullName },
        { "ulong", typeof(ulong).FullName },
        { "short", typeof(short).FullName },
        { "ushort", typeof(ushort).FullName },
        { "byte", typeof(byte).FullName },
        { "sbyte", typeof(sbyte).FullName },
        { "decimal", typeof(decimal).FullName },
        { "float", typeof(float).FullName },
        { "double", typeof(double).FullName },
        { "char", typeof(char).FullName },
        { "string", typeof(string).FullName },
        { "bool", typeof(bool).FullName },
    };

    private readonly Dictionary<string, string> _fullToShort;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShortTypesConverter"/> class.
    /// </summary>
    public ShortTypesConverter()
    {
        this._fullToShort = ShortToFull.ToDictionary(pair => pair.Value, pair => pair.Key);
    }

    #region Implementation of IShortTypesConverter

    /// <inheritdoc />
    public bool IsShortType(string typeFullName) =>
        ShortToFull.ContainsKey(typeFullName);

    /// <inheritdoc />
    public string ConvertToShort(ITypeFullName typeFullName, string onNotShort) =>
        this._fullToShort.GetMaybe(typeFullName.FullName)
            .SomeOrProvided(onNotShort);

    /// <inheritdoc />
    public string ConvertToTypeFullName(string shortType) =>
        ShortToFull.GetMaybe(shortType.Trim())
            .SomeOrProvided(shortType);

    #endregion
}
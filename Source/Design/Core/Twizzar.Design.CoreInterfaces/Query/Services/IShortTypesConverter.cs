using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;

namespace Twizzar.Design.CoreInterfaces.Query.Services;

/// <summary>
/// Convert full type to short type converter or vise versa.
/// For example System.Int32 to int.
/// </summary>
public interface IShortTypesConverter
{
    /// <summary>
    /// Is the type a short type.
    /// </summary>
    /// <param name="typeFullName">The type full name, for example System.String.</param>
    /// <returns></returns>
    bool IsShortType(string typeFullName);

    /// <summary>
    /// Covert to short or use onNotShort when it is not a short.
    /// </summary>
    /// <param name="typeFullName"></param>
    /// <param name="onNotShort"></param>
    /// <returns></returns>
    string ConvertToShort(ITypeFullName typeFullName, string onNotShort);

    /// <summary>
    /// Convert a short to a type full name.
    /// </summary>
    /// <param name="shortType"></param>
    /// <returns></returns>
    string ConvertToTypeFullName(string shortType);
}
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Twizzar.Design.Shared.CoreInterfaces.Name;

/// <summary>
/// Extension methods for <see cref="ITypeFullNameToken"/>.
/// </summary>
public static class ITypeFullNameTokenExtensions
{
    /// <summary>
    /// Create a new  <see cref="ITypeFullNameToken"/> with the generic parameters added to it.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="genericParameters"></param>
    /// <returns></returns>
    public static ITypeFullNameToken WithGenericParameters(this ITypeFullNameToken token, IEnumerable<ITypeFullNameToken> genericParameters)
    {
        var gParameters = genericParameters.ToImmutableArray();

        if (gParameters.Length == 0)
        {
            return token;
        }

        var genericPostfix = $"`{gParameters.Length}";

        return new TypeFullNameToken(
            token.Namespace,
            token.Typename,
            token.DeclaringType,
            genericPostfix,
            gParameters,
            token.ArrayBrackets,
            token.ContainingText);
    }
}
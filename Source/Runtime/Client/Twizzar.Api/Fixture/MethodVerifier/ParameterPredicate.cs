using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ViCommon.Functional.Monads.MaybeMonad;
using ParameterExpression = System.Linq.Expressions.Expression<System.Func<object, System.Type, bool>>;

namespace Twizzar.Fixture.MethodVerifier;

/// <summary>
/// Parameter predicate for method verification.
/// </summary>
/// <param name="ParameterType">
/// The parameter type this is equal to the method parameter type, except the parameter type contains a generic parameter, then the user needs to specify a constructed type.
/// If this is None the user has not setuped a predicate for this parameter.
/// </param>
/// <param name="Expression"></param>
[ExcludeFromCodeCoverage]
public record ParameterPredicate(Maybe<Type> ParameterType, ParameterExpression Expression)
{
    /// <summary>
    /// The argument provided to a method is matching the predicate.
    /// </summary>
    /// <param name="argument"></param>
    /// <param name="parameterInfo"></param>
    /// <returns></returns>
    public bool IsMatching(object argument, ParameterInfo parameterInfo)
    {
        // this can throw a InvalidCastException.
        try
        {
            return this.Expression.Compile()
                .Invoke(argument, this.ParameterType.SomeOrProvided(parameterInfo.ParameterType));
        }
        catch
        {
            return false;
        }
    }
}
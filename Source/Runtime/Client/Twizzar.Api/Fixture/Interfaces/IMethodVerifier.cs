using System;
using System.Linq.Expressions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;

namespace Twizzar.Fixture;

/// <summary>
/// Service for verifying methods.
/// </summary>
/// <typeparam name="TFixtureItem">The fixture item type.</typeparam>
/// <typeparam name="TPathProvider">The path provider type.</typeparam>
/// <typeparam name="TMethodPathMember"></typeparam>
public interface IMethodVerifier<TFixtureItem, out TPathProvider, TMethodPathMember> : IMemberVerifier
    where TPathProvider : new()
{
    /// <summary>
    /// Add a condition to check if the method is called with a certain parameter value.
    /// <remarks>
    /// We also generate extensions method for all parameter and advise to use them.
    /// If the parameter is called paramA then there will be a method called <c>WhereParamA</c>.
    /// </remarks>
    /// </summary>
    /// <typeparam name="TParam">The parameter type.</typeparam>
    /// <param name="name">The parameter name.</param>
    /// <param name="value">The value expected.</param>
    /// <returns>Self for further configuration.</returns>
    public IMethodVerifier<TFixtureItem, TPathProvider, TMethodPathMember> WhereParamIs<TParam>(
        string name,
        TParam value);

    /// <summary>
    /// Add a condition to check if a method parameter on call satisfies a certain predicate.
    /// <remarks>
    /// We also generate extensions method for all parameter and advise to use them.
    /// If the parameter is called paramA then there will be a method called <c>WhereParamA</c>.
    /// </remarks>
    /// </summary>
    /// <typeparam name="TParam">The parameter type.</typeparam>
    /// <param name="name">The parameter name.</param>
    /// <param name="predicate">The predicate the parameter needs to satisfy.</param>
    /// <returns>Self for further configuration.</returns>
    IMethodVerifier<TFixtureItem, TPathProvider, TMethodPathMember> WhereParamIs<TParam>(
        string name,
        Expression<Func<TParam, bool>> predicate);

    /// <summary>
    /// Add a condition to check if the method is called with a certain parameter value.
    /// The parameter value gets pulled from a dependency of this fixture.
    /// <remarks>
    /// We also generate extensions method for all parameter and advise to use them.
    /// If the parameter is called paramA then there will be a method called <c>WhereParamA</c>.
    /// </remarks>
    /// </summary>
    /// <typeparam name="TParam">The parameter type.</typeparam>
    /// <param name="name">The parameter name.</param>
    /// <param name="selector">Function for selecting a dependency.</param>
    /// <returns>Self for further configuration.</returns>
    IMethodVerifier<TFixtureItem, TPathProvider, TMethodPathMember> WhereParamIs<TParam>(
        string name,
        Func<TPathProvider, IMemberPath<TFixtureItem>> selector);
}
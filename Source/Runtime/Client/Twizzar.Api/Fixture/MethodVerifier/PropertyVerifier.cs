using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Services;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Fixture.MethodVerifier;

/// <inheritdoc cref="IPropertyVerifier{TDeclaredType}"/>
[ExcludeFromCodeCoverage]
internal class PropertyVerifier<TFixtureItem, TDeclaredType, TPathProvider> :
    MemberVerifier<TFixtureItem>, IPropertyVerifier<TDeclaredType>
    where TPathProvider : new()
{
    #region fields

    private readonly PropertyInfo _propertyInfo;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyVerifier{TFixtureItem, TDeclaredType, TPathProvider}"/> class.
    /// </summary>
    /// <param name="methodPath"></param>
    /// <param name="instanceCacheQuery"></param>
    public PropertyVerifier(IMemberPath<TFixtureItem> methodPath, IInstanceCacheQuery instanceCacheQuery)
        : base(methodPath, instanceCacheQuery)
    {
        if (methodPath is not IPropertyMemberPath<TFixtureItem>)
        {
            throw new InternalException(
                $"The path provided to the {nameof(PropertyVerifier<TFixtureItem, TDeclaredType, TPathProvider>)} should be a property.");
        }

        if (this.MockType.GetProperty(methodPath.MemberName) is not { } propertyInfo)
        {
            throw new InternalException($"Cannot find property with the name {methodPath.MemberName}");
        }

        this._propertyInfo = propertyInfo;
    }

    #endregion

    #region members

    /// <inheritdoc />
    public IPropertySetOrGetVerifier Get() =>
        new PropertySetOrGetVerifier<TDeclaredType>(
            this.Mock,
            this.MockType,
            this._propertyInfo,
            false,
            Maybe.None());

    /// <inheritdoc />
    public IPropertySetOrGetVerifier Set()
    {
        if (!this._propertyInfo.CanRead)
        {
            throw new ArgumentException($"Property {this._propertyInfo.Name} has not setter.");
        }

        return new PropertySetOrGetVerifier<TDeclaredType>(
            this.Mock,
            this.MockType,
            this._propertyInfo,
            true,
            Maybe.None());
    }

    /// <inheritdoc />
    public IPropertySetOrGetVerifier Set(TDeclaredType value)
    {
        if (!this._propertyInfo.CanRead)
        {
            throw new ArgumentException($"Property {this._propertyInfo.Name} has not setter.");
        }

        return new PropertySetOrGetVerifier<TDeclaredType>(
            this.Mock,
            this.MockType,
            this._propertyInfo,
            true,
            value);
    }

    #endregion
}
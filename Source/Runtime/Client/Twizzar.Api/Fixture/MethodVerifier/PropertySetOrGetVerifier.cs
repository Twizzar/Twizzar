using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Moq;
using Twizzar.Runtime.CoreInterfaces.Helpers;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Fixture.MethodVerifier;

/// <inheritdoc cref="IPropertySetOrGetVerifier"/>
internal class PropertySetOrGetVerifier<TDeclaredType> : IPropertySetOrGetVerifier
{
    #region fields

    private readonly object _mock;
    private readonly Type _mockType;
    private readonly PropertyInfo _propertyInfo;
    private readonly Maybe<TDeclaredType> _value;
    private readonly bool _isSet;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertySetOrGetVerifier{TDeclaredType}"/> class.
    /// </summary>
    /// <param name="mock"></param>
    /// <param name="mockType"></param>
    /// <param name="propertyInfo"></param>
    /// <param name="isSet"></param>
    /// <param name="value"></param>
    public PropertySetOrGetVerifier(
        object mock,
        Type mockType,
        PropertyInfo propertyInfo,
        bool isSet,
        Maybe<TDeclaredType> value)
    {
        this._mock = mock;
        this._mockType = mockType;
        this._propertyInfo = propertyInfo;
        this._isSet = isSet;
        this._value = value;
    }

    #endregion

    #region members

    /// <inheritdoc />
    public void Called(int times)
    {
        this.Verify(times);
    }

    /// <inheritdoc />
    public void CalledAtLeastOnce()
    {
        this.Verify(null);
    }

    private void Verify(int? count)
    {
        try
        {
            ReflectionGenericMethodBuilder.Create(new Action<Mock<object>, PropertyInfo, int?>(this.Verify))
                .WithGenericTypes(this._mockType)
                .WithParameters(this._mock, this._propertyInfo, count)
                .WithInvocationObject(this)
                .Invoke();
        }
        catch (TargetInvocationException ex)
        {
            if (ex.InnerException is MockException mockException and { IsVerificationError: true })
            {
                var method = this._isSet
                    ? this._propertyInfo.SetMethod
                    : this._propertyInfo.GetMethod;

                var whereInformation = new StringBuilder();
                whereInformation.AppendLine($"Property name: {this._propertyInfo.Name}");
                if (this._isSet && this._value.AsMaybeValue() is SomeValue<TDeclaredType> v)
                {
                    whereInformation.AppendLine($"Property set value: {v.Value}");
                }

                throw VerificationException.Construct(
                    mockException,
                    method,
                    Maybe.ToMaybe(count),
                    this._mock,
                    this._mockType,
                    whereInformation.ToString(),
                    i => true);
            }
            else
            {
                throw;
            }
        }
    }

    private void Verify<TMockedObject>(
        Mock<TMockedObject> mock,
        PropertyInfo propertyInfo,
        int? count)
        where TMockedObject : class
    {
        var type = typeof(TMockedObject);
        var parameter = Expression.Parameter(type);
        Expression body = Expression.Property(parameter, propertyInfo);

        var itIsAny = typeof(It)
            .GetMethod(nameof(It.IsAny), BindingFlags.Public | BindingFlags.Static);

        Action<TMockedObject> action;

        if (this._value.AsMaybeValue() is SomeValue<TDeclaredType> value)
        {
            action = m => this._propertyInfo.SetMethod.Invoke(
                m,
                new object[]
                {
                    value.Value,
                });
        }
        else
        {
            action = m => this._propertyInfo.SetMethod.Invoke(
                m,
                new[]
                {
                    itIsAny.MakeGenericMethod(typeof(TDeclaredType)).Invoke(null, Array.Empty<object>()),
                });
        }

        var getLambdaExpression = Expression.Lambda<Func<TMockedObject, TDeclaredType>>(body, parameter);

        switch (count, this._isSet)
        {
            case (null, false):
                mock.VerifyGet(getLambdaExpression);
                break;
            case (null, true):
                mock.VerifySet(action);
                break;
            case ({ } c, false):
                mock.VerifyGet(getLambdaExpression, Times.Exactly(c));
                break;
            case ({ } c, true):
                mock.VerifySet(action, Times.Exactly(c));
                break;
        }
    }

    #endregion
}
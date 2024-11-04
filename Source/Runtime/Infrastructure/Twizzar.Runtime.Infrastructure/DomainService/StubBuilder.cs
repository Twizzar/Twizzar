using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Moq;
using Moq.Language;
using Moq.Language.Flow;

using Twizzar.Runtime.CoreInterfaces.Exceptions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.Functional.Monads.MaybeMonad;

using static Twizzar.Runtime.CoreInterfaces.Helpers.ReflectionGenericMethodBuilder;

namespace Twizzar.Runtime.Infrastructure.DomainService;

/// <summary>
/// Helper class for building up the Moq Mock.
/// </summary>
/// <typeparam name="T">The mocked type.</typeparam>
public class StubBuilder<T>
    where T : class
{
    #region static fields and constants

    private const BindingFlags ReflectionBindingFlags = BindingFlags.NonPublic |
                                                        BindingFlags.Public |
                                                        BindingFlags.Instance;

    #endregion

    #region fields

    private readonly Mock<T> _mock;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="StubBuilder{T}"/> class.
    /// </summary>
    public StubBuilder()
    {
        this._mock = new Mock<T>();
        this._mock.SetupAllProperties();
    }

    #endregion

    #region members

    /// <summary>
    /// Setup a property value.
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="value"></param>
    public void SetupPropertyValue(PropertyInfo propertyInfo, object value)
    {
        if (propertyInfo.CanWrite && (value == null || value.GetType() == propertyInfo.PropertyType))
        {
            // set value with reflection
            propertyInfo.SetValue(this._mock.Object, value);
        }
        else
        {
            Create(this.SetupProperty<object>)
                .WithInvocationObject(this)
                .WithGenericTypes(propertyInfo.PropertyType)
                .WithParameters(propertyInfo, this._mock, value)
                .Invoke();
        }
    }

    /// <summary>
    /// Start setting up a method.
    /// </summary>
    /// <param name="methodInfo">The method info sue <see cref="FindMethodInfo"/> to get the correct method info.</param>
    /// <returns>A new method builder.</returns>
    public MethodBuilder Method(MethodInfo methodInfo) => new(methodInfo, this._mock);

    /// <summary>
    /// Returns the mock.
    /// </summary>
    /// <returns></returns>
    public Mock<T> Build() => this._mock;

    /// <summary>
    /// Finds the property info of a property description. This property can be on declared type of the method or on a implemented interface.
    /// </summary>
    /// <param name="propertyDescription"></param>
    /// <returns></returns>
    public static Maybe<PropertyInfo> FindPropertyInfo(IPropertyDescription propertyDescription)
    {
        var type = typeof(T);

        var propInfo = type.GetProperty(propertyDescription.Name, ReflectionBindingFlags);

        if (propInfo != null)
        {
            return propInfo;
        }

        foreach (var @interface in type.GetInterfaces())
        {
            var interfacePropInfo = @interface.GetProperty(propertyDescription.Name, ReflectionBindingFlags);

            if (interfacePropInfo != null)
            {
                return interfacePropInfo;
            }
        }

        return Maybe.None();
    }

    /// <summary>
    /// Find the method info of a method description. This method can be on declared type of the method or on a implemented interface.
    /// </summary>
    /// <param name="methodDescription"></param>
    /// <returns>Some <see cref="MethodInfo"/> or None if not found.</returns>
    public static Maybe<MethodInfo> FindMethodInfo(IMethodDescription methodDescription) =>
        ((IRuntimeMethodDescription)methodDescription).GetMethodInfo();

    private void SetupProperty<TResult>(
        PropertyInfo property,
        Mock<T> mock,
        object value)
    {
        var declaringType = property.DeclaringType;

        var parameter = Expression.Parameter(declaringType);
        var body = Expression.PropertyOrField(parameter, property.Name);
        var lambdaExpression = Expression.Lambda<Func<T, TResult>>(body, parameter);

        switch (value)
        {
            case TResult rawValue:
                this._mock.Setup(lambdaExpression).Returns(rawValue);
                break;
            case null:
                this._mock.Setup(lambdaExpression).Returns(null);
                break;
            case Func<TResult> func:
                this._mock.Setup(lambdaExpression).Returns(func);
                break;
            default:
                var exp = new ResolveTypeException(
                    $"The provided value {value} is not assignable to the property {property.Name}");

                this.Log(exp);
                throw exp;
        }
    }

    #endregion

    #region Nested type: MethodBuilder

    /// <summary>
    /// Helper class for building up the Moq Methods.
    /// </summary>
    public class MethodBuilder
    {
        #region fields

        private readonly Mock<T> _mock;
        private readonly MethodInfo _methodInfo;
        private readonly IList<object> _callbacks = new List<object>();
        private Maybe<object> _value = Maybe.None();

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodBuilder"/> class.
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="mock"></param>
        internal MethodBuilder(MethodInfo methodInfo, Mock<T> mock)
        {
            this._methodInfo = methodInfo;
            this._mock = mock;
        }

        #endregion

        #region members

        /// <summary>
        /// Add a method value.
        /// </summary>
        /// <param name="valueOrDelegate">This should either of the return type of the method or a delegate with all parameters of the method and the same return type as the method.</param>
        public void AddMethodValue(object valueOrDelegate) =>
            this._value = Maybe.Some(valueOrDelegate ?? new NullValue());

        /// <summary>
        /// Add a method callback.
        /// </summary>
        /// <param name="callback">The callback should be an action with all the parameters of the method.</param>
        public void AddMethodCallback(object callback) => this._callbacks.Add(callback);

        /// <summary>
        /// Finish the setup.
        /// </summary>
        public void Setup()
        {
            // nothing setuped.
            if (this._value.IsNone && this._callbacks.Count == 0)
            {
                return;
            }

            foreach (var typeSelector in MoqTypeSelector.GetTypeSelectors(this._methodInfo, this._value))
            {
                Create(this.Setup<object>)
                    .WithInvocationObject(this)
                    .WithGenericTypes(typeSelector.ReturnType)
                    .WithParameters(typeSelector)
                    .Invoke();
            }
        }

        /// <summary>
        /// Gets the default value:
        /// For objects null
        /// For structs calls the default constructor.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetDefault(Type type) =>
            type.IsValueType
                ? Activator.CreateInstance(type)
                : null;

        private void Setup<TResult>(MoqTypeSelector typeSelector)
        {
            var lambdaExpression =
                GetMethodLambdaExpression<T, TResult>(
                    this._methodInfo,
                    this._methodInfo.DeclaringType,
                    typeSelector);

            var mockSetup = this._mock.Setup(lambdaExpression);

            if (this._callbacks.LastOrDefault() is { } callbacks)
            {
                SetupCallback(callbacks, mockSetup);
            }

            this._value.IfSome(value =>
                SetupReturnValue(value, mockSetup));
        }

        private static void SetupReturnValue<TResult>(object value, IReturns<T, TResult> mockSetup)
        {
            switch (value)
            {
                case Delegate:
                    var funcType = value.GetType();
                    var invokeInfo = funcType.GetMethod("Invoke") ??
                        throw new ArgumentException($"The type ({funcType.FullName}) provided is not a delegate and not a valid type.");
                    var d = Delegate.CreateDelegate(funcType, value, invokeInfo);

                    mockSetup.Returns(new InvocationFunc(invocation =>
                    {
                        try
                        {
                            var result = d.DynamicInvoke(invocation.Arguments.ToArray());

                            if (result != null && result.GetType().IsAssignableTo(invocation.Method.ReturnType))
                            {
                                return result;
                            }
                        }
                        catch (Exception)
                        {
                            // do nothing
                        }
                        return GetDefault(invocation.Method.ReturnType);
                    }));

                    break;

                case NullValue:
                    mockSetup.Returns(null);
                    break;
                default:
                    mockSetup.Returns(
                        new InvocationFunc(invocation =>
                            value.GetType().IsAssignableTo(invocation.Method.ReturnType)
                                ? value
                                : GetDefault(invocation.Method.ReturnType)));

                    break;
            }
        }

        private static void SetupCallback<TResult>(object callback, ISetup<T, TResult> mockSetup)
        {
            var actionType = callback.GetType();
            var invokeInfo = actionType.GetMethod("Invoke") ?? throw new ResolveTypeException(
                    $"The type ({actionType.FullName}) provided is not a delegate and not a valid type.");
            var d = Delegate.CreateDelegate(actionType, callback, invokeInfo);

            mockSetup.Callback(new InvocationAction(invocation =>
            {
                if (invocation.Arguments.Select(o => o.GetType())
                    .Zip(d.Method.GetParameters(), (type, info) => (type, info))
                    .All(tuple => tuple.info.ParameterType.IsAssignableFrom(tuple.type)))
                {
                    d.DynamicInvoke(invocation.Arguments.ToArray());
                }
            }));
        }

        /// <summary>
        /// Gets the lambda expression for selecting a certain method.
        /// For example:
        /// <code>x => x.MyMethod(It.IsAny&lt;int&gt;())</code>
        /// </summary>
        /// <typeparam name="TMockedObject"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="methodInfo"></param>
        /// <param name="type"></param>
        /// <param name="typeSelector"></param>
        /// <returns></returns>
        private static Expression<Func<TMockedObject, TResult>> GetMethodLambdaExpression<TMockedObject, TResult>(
            MethodInfo methodInfo,
            Type type,
            MoqTypeSelector typeSelector)
        {
            var lambdaParameter = Expression.Parameter(type);
            var itAnyMethod = typeof(It).GetMethod("IsAny", BindingFlags.Public | BindingFlags.Static)!;

            // create It.IsAny<T> parameters and ask the type selector for the T type.
            var parameterExpressions =
                methodInfo.GetParameters()
                    .Select(pInfo => Expression.Call(
                        itAnyMethod.MakeGenericMethod(
                            typeSelector.ConstructIsAnyType(pInfo.ParameterType))))
                    .ToList();

            var method = typeSelector.ConstructMethod(methodInfo);

            var body = Expression.Call(lambdaParameter, method, parameterExpressions);
            return Expression.Lambda<Func<TMockedObject, TResult>>(body, lambdaParameter);
        }

        #endregion
    }

    #endregion

}

/// <summary>
/// Marker record for null values.
/// </summary>
public record NullValue();

/// <summary>
/// Marker record for the void type.
/// </summary>
public record VoidType();
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moq;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Name;
using Twizzar.Runtime.Infrastructure.DomainService.GenericParameterTree;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.Infrastructure.DomainService;

/// <summary>
/// For generics it is important to determine the right type for the generic parameters. Because when the user does not configure the type twizzar needs to determine the right generic argument, which is not trivial. It is also important to consistent assign the same Type else the setup will not work.
/// For simple generic Methods <see cref="It.IsAnyType"/> can be used as generic argument.
/// But if the generic parameter has a constrain then the return type and the constrain need to be considered.
/// For a struct constrain <see cref="It.IsValueType"/> can be used.
/// For specific constrains the constrain type and the setup value type are booth important and in this case we will create tow setups one for the constrain type and one for the value type; if both are the same one setup is used.
/// </summary>
public class MoqTypeSelector
{
    #region fields

    private readonly Dictionary<Type, Type> _genericCache;

    #endregion

    #region ctors

    private MoqTypeSelector(Type returnType, Type originalType = null)
        : this(
            returnType,
            originalType is null
                ? new()
                : new Dictionary<Type, Type>() { { originalType, returnType } })
    {
    }

    private MoqTypeSelector(Type returnType, Dictionary<Type, Type> genericCache)
    {
        this._genericCache = genericCache;
        this.ReturnType = returnType;
    }

    #endregion

    #region properties

    /// <summary>
    /// Gets the return type.
    /// </summary>
    public Type ReturnType { get; }

    #endregion

    #region members

    /// <summary>
    /// Get one or many <see cref="MoqTypeSelector"/>.
    /// </summary>
    /// <param name="methodInfo"></param>
    /// <param name="maybeValueOrDelegate"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException">Multi constrain is not supported.</exception>
    public static IEnumerable<MoqTypeSelector> GetTypeSelectors(
        MethodInfo methodInfo,
        Maybe<object> maybeValueOrDelegate)
    {
        var valueType = maybeValueOrDelegate.Match(GetValueType, () => typeof(object));
        return GetTypeSelectors(methodInfo.ReturnType, methodInfo, valueType);
    }

    /// <summary>
    /// Get the T for It.IsAny&lt;T&gt;.
    /// </summary>
    /// <param name="parameterType"></param>
    /// <returns></returns>
    public Type ConstructIsAnyType(Type parameterType)
    {
        Type[] GetGenericArguments() =>
            parameterType.GetGenericArguments()
                .Select(this.ConstructIsAnyType)
                .ToArray();

        return parameterType switch
        {
            // When the parameter is a generic parameter for example T
            { IsGenericParameter: true, DeclaringMethod: not null } =>
                this.GetMoqType(parameterType),

            // When the parameter is generic for example List<T>
            { IsGenericType: true } =>
                parameterType.GetGenericTypeDefinition().MakeGenericType(GetGenericArguments()),

            // For arrays use this method on the element type and then create an array type
            { IsArray: true } =>
                this.ConstructIsAnyType(parameterType.GetElementType()).MakeArrayType(),

            _ => parameterType,
        };
    }

    /// <summary>
    /// For each generic parameter in the method get determine the type and construct the method.
    /// If the method is not generic return the provide <see cref="MethodInfo"/>.
    /// </summary>
    /// <param name="methodInfo"></param>
    /// <returns></returns>
    public MethodInfo ConstructMethod(MethodInfo methodInfo)
    {
        if (methodInfo.IsGenericMethodDefinition)
        {
            return methodInfo
                .MakeGenericMethod(methodInfo.GetGenericArguments()
                    .Select(this.GetMoqType)
                    .ToArray());
        }

        return methodInfo;
    }

    /// <summary>
    /// Add a generic type to the generic cache.
    /// </summary>
    /// <param name="genericType"></param>
    /// <param name="constructedType"></param>
    public void AddToGenericCache(Type genericType, Type constructedType)
    {
        this._genericCache[genericType] = constructedType;
    }

    /// <summary>
    /// For a generic argument get the correct type.
    /// </summary>
    /// <param name="genericArgument"></param>
    /// <returns></returns>
    public Type GetMoqType(Type genericArgument) =>
        this._genericCache.GetMaybe(genericArgument)
            .SomeOrProvided(() =>
            {
                var t = GetNewMoqType(genericArgument);
                this._genericCache.Add(genericArgument, t);
                return t;
            });

    private static IEnumerable<MoqTypeSelector> GetTypeSelectors(Type type, MethodInfo methodInfo, Type valueType)
    {
        switch (type)
        {
            // void Method(..)
            case not null when type == typeof(void):
                yield return new MoqTypeSelector(typeof(VoidType));

                break;

            // Arrays
            case { IsArray: true }:
                foreach (var moqTypeSelector in GetTypeSelectors(type.GetElementType(), methodInfo, valueType))
                {
                    yield return new MoqTypeSelector(
                        moqTypeSelector.ReturnType.MakeArrayType(),
                        moqTypeSelector._genericCache);
                }

                break;

            // Either generic type (IList<T>) or generic parameter (T)
            case { IsGenericType: true } or { IsGenericParameter: true }:
                foreach (var moqTypeSelector in GetTypeSelectorForGenericType(type, methodInfo, valueType))
                {
                    yield return moqTypeSelector;
                }

                break;

            // Non generic class
            default:
                yield return new MoqTypeSelector(methodInfo.ReturnType);

                break;
        }
    }

    private static IEnumerable<MoqTypeSelector> GetTypeSelectorForGenericType(
        Type type,
        MethodInfo methodInfo,
        Type valueType)
    {
        var root = GenericParameterTreeBuilder.Build(type, valueType)
            .Match(x => x, f => throw new Exception(f.Message));

        var possibleGenericTypeInfo = FindPossibleGenericTypeInfos(methodInfo, root);

        // if we have more than one Generic Parameter we want to provide all possible combinations (CartesianProduct)
        // so if we have a method like this: Tuple<T, K> Method<T, K>() where T: IA where K : IA
        // where:
        // interface IA
        // interface IA2 : IA
        //
        // and the user gives us the value Tuple<IA2, IA2> then we want to setup the following methods:
        // Method<IA , IA>  = Tuple<IA2, IA2>
        // Method<IA , IA1> = Tuple<IA2, IA2>
        // Method<IA1, IA>  = Tuple<IA2, IA2>
        // Method<IA1, IA1> = Tuple<IA2, IA2>
        // possible combinations 2 * 2 = 4
        foreach (var tuples in possibleGenericTypeInfo.CartesianProduct())
        {
            var dict = tuples.ToDictionary(tuple => tuple.Node);

            Type ConstructRecursive(IGenericTypeNode node) =>
                node switch
                {
                    GenericParameterNode leafeNode when dict.TryGetValue(leafeNode, out var value) =>
                        value.ReturnValue,
                    Node n =>
                        n.UboundType.IsGenericType
                            ? n.UboundType.GetGenericTypeDefinition()
                                .MakeGenericType(
                                    node.Children.Select(ConstructRecursive).ToArray())
                            : n.UboundType,
                    _ =>
                        throw new InternalException(
                            $"Cannot construct the return the of the method {methodInfo.Name}, because it was not possible to match the method return type {type.ToTypeFullName().GetFriendlyCSharpTypeName()} to the given type {valueType.ToTypeFullName().GetFriendlyCSharpTypeName()}"),
                };

            var returnType = ConstructRecursive(root);
            var cachedGenerics = dict.Values.ToDictionary(tuple => tuple.Node.UnboundType, tuple => tuple.ReturnValue);
            yield return new MoqTypeSelector(returnType, cachedGenerics);
        }
    }

    /// <summary>
    /// Gets the type of the value if a value was provide.
    /// If it is a delegate gets the return type of the delegate.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static Type GetValueType(object type) =>
        type switch
        {
            Delegate d => d.Method.ReturnType,
            _ => type.GetType(),
        };

    private static IEnumerable<List<(GenericParameterNode Node, Type ReturnValue)>> FindPossibleGenericTypeInfos(
        MemberInfo methodInfo,
        IGenericTypeNode root)
    {
        // Traverse through all the generic parameters
        foreach (var node in root.TraverseDepthFirst().OfType<GenericParameterNode>())
        {
            var constraints = node.UnboundType.GetGenericParameterConstraints();

            if (constraints.Length > 1)
            {
                throw new NotSupportedException(
                    $"The Method {methodInfo.Name} is generic and has more than one type constrain on the generic parameter {node.UnboundType.Name} which is not supported at the moment.");
            }
            else if (constraints.Length == 1)
            {
                yield return GetTypesForConstrain(constraints.Single(), node.ValueType)
                    .Select(t => (node, t))
                    .ToList();
            }
            else
            {
                var t = GetNewMoqType(node.UnboundType);
                yield return new List<(GenericParameterNode, Type)> { (node, t) };
            }
        }
    }

    private static IEnumerable<Type> GetTypesForConstrain(Type constrain, Type valueType)
    {
        // if the type is a Moq mock object.
        if (valueType.Namespace == "Castle.Proxies")
        {
            var seq = valueType.FindInterfaces(
                (type, criteria) =>
                    type.Namespace != "Castle.DynamicProxy" && !(type.Namespace?.Contains("Moq") ?? false),
                null);

            valueType = seq.First();
        }

        if (valueType.IsAssignableTo(constrain) && constrain != valueType)
        {
            yield return valueType;
        }

        if (constrain == typeof(ValueType))
        {
            yield return typeof(It.IsValueType);
        }
        else
        {
            yield return constrain;
        }
    }

    private static Type GetNewMoqType(Type genericArgument)
    {
        if (!genericArgument.IsGenericMethodParameter())
        {
            return genericArgument;
        }

        return genericArgument.GetGenericParameterConstraints() switch
        {
            // if there is one constrain and its a struct constrain
            { Length: 1 } x when x.Single() == typeof(ValueType) =>
                typeof(It.IsValueType),

            // if there is only one non struct constrain.
            { Length: 1 } x when x.Single() is { } constrain =>
                constrain,

            // if there is more tha one constrain
            { Length: > 1 } =>
                throw new NotSupportedException("Multi constrain is not supported"),

            // no constrains
            _ =>
                typeof(It.IsAnyType),
        };
    }

    #endregion
}
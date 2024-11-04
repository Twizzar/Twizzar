using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using Moq;

using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Services;
using Twizzar.Runtime.CoreInterfaces.Helpers;
using Twizzar.Runtime.Infrastructure.DomainService;
using Twizzar.Runtime.Infrastructure.DomainService.GenericParameterTree;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;

using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Fixture.MethodVerifier
{
    /// <inheritdoc cref="IMethodVerifier{TFixtureItem, PathProvider, TMethodPathMember}"/>
    internal class MethodVerifier<TFixtureItem, TPathProvider, TMethodPathMember> : MemberVerifier<TFixtureItem>,
        IMethodVerifier<TFixtureItem,
            TPathProvider,
            TMethodPathMember>
        where TPathProvider : new()
    {
        #region fields

        private readonly DefaultDictionary<string, ParameterPredicate> _parameterPredicates =
            new(s => new ParameterPredicate(Maybe.None(), (o, type) => true));

        private readonly MethodInfo _methodInfo;
        private readonly MoqTypeSelector _typeSelector;

        private readonly IImmutableSet<string> _parameterNames;
        private readonly DefaultDictionary<string, string> _parameterDescription = new(s => "any");

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodVerifier{TFixtureItem, TPathProvider, TMethodPathMember}"/> class.
        /// </summary>
        /// <param name="methodPath"></param>
        /// <param name="instanceCacheQuery"></param>
        public MethodVerifier(IMemberPath<TFixtureItem> methodPath, IInstanceCacheQuery instanceCacheQuery)
            : base(methodPath, instanceCacheQuery)
        {
            if (methodPath is not IMethodMemberPath<TFixtureItem> methodMemberPath)
            {
                throw new InternalException(
                    $"The path provided to the {nameof(MethodVerifier<TFixtureItem, TPathProvider, TMethodPathMember>)} should be a method.");
            }

            this._methodInfo = this.GetMethodInfo(methodMemberPath)
                .SomeOrProvided(() =>
                    throw ConstructMethodNotFoundException(methodMemberPath));

            this._typeSelector = MoqTypeSelector.GetTypeSelectors(this._methodInfo, Maybe.None()).First();
            this._parameterNames = this._methodInfo.GetParameters().Select(info => info.Name).ToImmutableHashSet();
        }

        #endregion

        #region members

        /// <inheritdoc />
        public IMethodVerifier<TFixtureItem, TPathProvider, TMethodPathMember> WhereParamIs<TParam>(
            string name,
            TParam value) =>
            this.WhereParam<TParam>(name, param => param.Equals(value), $"equals to {value}");

        /// <inheritdoc />
        public IMethodVerifier<TFixtureItem, TPathProvider, TMethodPathMember> WhereParamIs<TParam>(
            string name,
            Expression<Func<TParam, bool>> predicate) =>
            this.WhereParam(name, predicate, predicate.ToString());

        /// <inheritdoc />
        public IMethodVerifier<TFixtureItem, TPathProvider, TMethodPathMember> WhereParamIs<TParam>(
            string name,
            Func<TPathProvider, IMemberPath<TFixtureItem>> selector)
        {
            var path = selector(new TPathProvider());

            var value = this.Get(path)
                .SomeOrProvided(() => $"Cannot find the instance at path {path}");

            return this.WhereParam<TParam>(name, o => o.Equals(value), $"is equal to {value}");
        }

        /// <inheritdoc />
        public void Called(int times)
        {
            this.Called(Maybe.Some(times));
        }

        /// <inheritdoc />
        public void CalledAtLeastOnce()
        {
            this.Called(Maybe.None());
        }

        private IMethodVerifier<TFixtureItem, TPathProvider, TMethodPathMember> WhereParam<TParam>(
            string name,
            Expression<Func<TParam, bool>> predicate,
            string description)
        {
            if (!this._parameterNames.Contains(name))
            {
                throw new ArgumentException(
                    $"The selected method {this._methodInfo.Name} has no parameter with the name {name}. Available parameters: {this._parameterNames.ToCommaSeparated()}. Assure that the right overload is selected.",
                    nameof(name));
            }

            var o = Expression.Parameter(typeof(object));
            var t = Expression.Parameter(typeof(Type));

            var exp = Expression.Lambda<Func<object, Type, bool>>(
                Expression.Invoke(predicate, Expression.Convert(o, typeof(TParam))),
                o,
                t);

            return this.WhereParam(name, exp, description, typeof(TParam));
        }

        private IMethodVerifier<TFixtureItem, TPathProvider, TMethodPathMember> WhereParam(
            string name,
            Expression<Func<object, Type, bool>> predicate,
            string description,
            Type parameterType)
        {
            this._methodInfo.GetParameters()
                .FirstOrNone(info => info.Name == name)
                .IfSome(info =>
                {
                    GenericParameterTreeBuilder.Build(info.ParameterType, parameterType)
                        .IfSuccess(root =>
                            root.TraverseDepthFirst().OfType<GenericParameterNode>().ForEach(node =>
                                this._typeSelector.AddToGenericCache(node.UnboundType, node.ValueType)));
                });

            this._parameterPredicates[name] = new(parameterType, predicate);
            this._parameterDescription[name] = description;
            return this;
        }

        private Maybe<MethodInfo> GetMethodInfo(IMethodMemberPath<TFixtureItem> methodMemberPath)
        {
            var parameters = methodMemberPath.Parameters;

            var methodInfo = GetMethods(this.MockType)
                .Where(info => info.Name == methodMemberPath.MemberName)
                .Where(info => info.GetGenericArguments().Select(type => type.Name)
                    .SequenceEqual(methodMemberPath.GenericArguments.Select(argument => argument.Name)))
                .Where(info => parameters.Length == 0 ||
                               info.GetParameters()
                                   .Select(pInfo => pInfo.ParameterType)
                                   .Select(t => t.Name)
                                   .SequenceEqual(parameters.Select(p => p.DeclaredTypeName)))
                .OrderBy(info => info.GetParameters().Length)
                .FirstOrDefault();

            return Maybe.ToMaybe(methodInfo);
        }

        private static IEnumerable<MethodInfo> GetMethods(Type type) =>
            type.GetInterfaces().Prepend(type)
                .SelectMany(t => t.GetMethods());

        private static Exception ConstructMethodNotFoundException(
            IMethodMemberPath<TFixtureItem> path)
        {
            var requirements = new StringBuilder();
            requirements.AppendLine($"Method name is {path.MemberName}");

            if (path.Parameters.Length != 0)
            {
                requirements.AppendLine(
                    $"Has exactly the parameter types: {path.Parameters.Select(p => p.DeclaredTypeName).ToCommaSeparated()}");
            }

            if (path.GenericArguments.Length != 0)
            {
                requirements.AppendLine(
                    $"Has generic arguments with the names: {path.GenericArguments.Select(x => x.Name).ToCommaSeparated()}");
            }

            throw new ArgumentException(
                $"No method found which fulfills the following requirements: \n{requirements}");
        }

        private void Called(Maybe<int> count)
        {
            try
            {
                this.Verify(this.Mock, this._methodInfo, this.MockType, count);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException is MockException mockException and { IsVerificationError: true })
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"Method name: {this._methodInfo.Name}");

                    if (this._methodInfo.GetParameters().Length > 0)
                    {
                        sb.AppendLine($"Parameters:");

                        foreach (var info in this._methodInfo.GetParameters())
                        {
                            sb.AppendLine(
                                $"    {info.ParameterType.Name} {info.Name}: {this._parameterDescription[info.Name]}");
                        }
                    }

                    throw VerificationException.Construct(
                        mockException,
                        this._methodInfo,
                        count,
                        this.Mock,
                        this.MockType,
                        sb.ToString(),
                        i => this.ArgumentsMatchingThePredicates(i.Arguments));
                }

                throw;
            }
        }

        private bool ArgumentsMatchingThePredicates(IReadOnlyList<object> arguments)
        {
            var parameters = this._methodInfo.GetParameters();

            if (arguments.Count != parameters.Length)
            {
                return false;
            }

            for (var i = 0; i < this._methodInfo.GetParameters().Length; i++)
            {
                var parameterInfo = this._methodInfo.GetParameters()[i];
                var argument = arguments[i];

                if (!this._parameterPredicates[parameterInfo.Name].IsMatching(argument, parameterInfo))
                {
                    return false;
                }
            }

            return true;
        }

        private void Verify(object mock, MethodInfo methodInfo, Type mockType, Maybe<int> count)
        {
            var returnType = this._typeSelector.ConstructMethod(this._methodInfo)
                .ReturnType;

            if (returnType == typeof(void))
            {
                returnType = typeof(VoidType);
            }

            ReflectionGenericMethodBuilder.Create(this.Verify<object, object>)
                .WithInvocationObject(this)
                .WithGenericTypes(mockType, returnType)
                .WithParameters(mock, methodInfo, count)
                .Invoke();
        }

        private void Verify<TMockedObject, TDeclaredType>(
            Mock<TMockedObject> mock,
            MethodInfo methodInfo,
            Maybe<int> count)
            where TMockedObject : class
        {
            if (typeof(TDeclaredType) == typeof(VoidType))
            {
                var lambdaExpression = this.GetVerifyExpression<TMockedObject, Action<TMockedObject>>(methodInfo);
                count.Do(
                        i => mock.Verify(lambdaExpression, Times.Exactly(i)),
                        () => mock.Verify(lambdaExpression));
            }
            else
            {
                var lambdaExpression = this.GetVerifyExpression<TMockedObject, Func<TMockedObject, TDeclaredType>>(methodInfo);
                count.Do(
                        i => mock.Verify(lambdaExpression, Times.Exactly(i)),
                        () => mock.Verify(lambdaExpression));
            }
        }

        private Expression<TExpressionType> GetVerifyExpression<TMockedObject, TExpressionType>(
            MethodInfo methodInfo)
            where TMockedObject : class
        {
            var type = typeof(TMockedObject);
            var parameter = Expression.Parameter(type);
            var parameterExpressions = this.GetParameterExpression(methodInfo);
            var body = Expression.Call(parameter, this._typeSelector.ConstructMethod(methodInfo), parameterExpressions);
            return Expression.Lambda<TExpressionType>(body, parameter);
        }

        private IEnumerable<MethodCallExpression> GetParameterExpression(MethodInfo methodInfo)
        {
            var itIss = typeof(It).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(info => info.Name == nameof(It.Is))
                .Where(info => info.GetParameters().Length == 1)
                .Where(info => !info.GetParameters()[0].ParameterType.ContainsGenericParameters);

            var itIs = itIss.Single();

            foreach (var pInfo in methodInfo.GetParameters())
            {
                var predicate = this._parameterPredicates[pInfo.Name];
                var innerType = predicate.ParameterType.SomeOrProvided(
                    this._typeSelector.ConstructIsAnyType(pInfo.ParameterType));
                var exp = Expression.Call(itIs.MakeGenericMethod(innerType), predicate.Expression);
                yield return exp;
            }
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Services;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;

[assembly: InternalsVisibleTo("Twizzar.Api.Tests")]

namespace Twizzar.Fixture.Verifier
{
    /// <inheritdoc cref="ICtorVerifier{T}"/>
    internal class CtorVerifier<T> : ICtorVerifier<T>
    {
        #region fields

        private readonly IFixtureItemContainer _fixtureItemContainer;

        private readonly DefaultDictionary<string, ICtorParameterVerificationInfo> _parameterConfig =
            new(new DummyParameter());

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="CtorVerifier{T}"/> class.
        /// </summary>
        /// <param name="fixtureItemContainer"></param>
        public CtorVerifier(IFixtureItemContainer fixtureItemContainer)
        {
            this._fixtureItemContainer = fixtureItemContainer;
        }

        #endregion

        #region members

        /// <inheritdoc />
        public ICtorVerifier<T> SetupParameter(string parameterName, object instance)
        {
            this._parameterConfig.Add(parameterName, new InstanceParameterConfig(instance));
            return this;
        }

        /// <inheritdoc />
        public ICtorVerifier<T> IgnoreParameter(string parameterName, object defaultValue = default)
        {
            this._parameterConfig.Add(parameterName, new IgnoreParameterConfig(defaultValue));
            return this;
        }

        /// <inheritdoc />
        public void ShouldThrowArgumentNullException()
        {
            foreach (var constructorInfo in typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.Instance))
            {
                var allParameters = constructorInfo.GetParameters()
                    .Select(info => this.CreateParameter(info.ParameterType, info.Name, this._parameterConfig))
                    .ToImmutableArray();

                for (var i = 0; i < allParameters.Length; i++)
                {
                    var parameterInfo = constructorInfo.GetParameters()[i];

                    if (!constructorInfo.GetParameters().ElementAt(i).ParameterType.CanBeNull())
                    {
                        continue;
                    }

                    if (this._parameterConfig[parameterInfo.Name] is IgnoreParameterConfig)
                    {
                        continue;
                    }

                    var parameters = allParameters.SetItem(i, null);

                    VerifyException(constructorInfo, parameterInfo, parameters, i);
                }

                VerifyValidCase(constructorInfo, allParameters);
            }
        }

        private static void VerifyException(ConstructorInfo constructorInfo, ParameterInfo parameterInfo, ImmutableArray<object> parameters, int i)
        {
            try
            {
                constructorInfo.Invoke(parameters.ToArray());
            }
            catch (TargetInvocationException exception)
            {
                var innerException = UnpackException(exception);

                if (innerException is not ArgumentNullException argumentException)
                {
                    throw new CtorVerifierException($"Exception was not of type {nameof(ArgumentNullException)} for parameter {i}-{parameterInfo.Name}.");
                }

                if (argumentException.ParamName != parameterInfo.Name)
                {
                    throw new CtorVerifierException(
                        $"ArgumentNullException ParamName is not correct for parameter {i}-{parameterInfo.Name}," +
                        $" actual {argumentException.ParamName} but expected {parameterInfo.Name}.");
                }

                // everything fine
                return;
            }

            // no exception thrown, error state
            throw new CtorVerifierException($"No exception thrown for parameter {i}-{parameterInfo.Name}, " +
                                            "expected ArgumentNullException when parameter is null but got no exception");
        }

        private static void VerifyValidCase(ConstructorInfo constructorInfo, ImmutableArray<object> allParameters)
        {
            try
            {
                constructorInfo.Invoke(allParameters.ToArray());
            }
            catch (Exception ex)
            {
                throw new CtorVerifierException($"Expected no exception for activating instance but got {ex.InnerException?.GetType()}");
            }
        }

        private object CreateParameter(
            Type type,
            string name,
            IReadOnlyDictionary<string, ICtorParameterVerificationInfo> config)
        {
            if (config.ContainsKey(name))
            {
                switch (config[name])
                {
                    case InstanceParameterConfig instanceParameter:
                        return instanceParameter.Instance;
                    case IgnoreParameterConfig ignoreParameter:
                        return ignoreParameter.DefaultValue;
                }
            }

            Func<object> f = this.CreateDummyParameter<object>;

            return f.Method.GetGenericMethodDefinition()
                .MakeGenericMethod(type)
                .Invoke(this, Array.Empty<object>());
        }

        private object CreateDummyParameter<TParamType>() =>
            this._fixtureItemContainer.GetInstance<TParamType>();

        private static Exception UnpackException(Exception exp)
        {
            if (exp.InnerException is null)
            {
                return exp;
            }

            return UnpackException(exp.InnerException);
        }

        #endregion

        #region Nested type: DummyParameter

        private sealed class DummyParameter : ICtorParameterVerificationInfo
        {
        }

        #endregion

        #region Nested type: ICtorParameterVerificationInfo

        [SuppressMessage(
            "StyleCop.CSharp.OrderingRules",
            "SA1201:Elements should appear in the correct order",
            Justification = "Inner interface only used here.")]
        [SuppressMessage(
            "StyleCop.CSharp.DocumentationRules",
            "SA1600:Elements should be documented",
            Justification = "Inner interface only used here.")]
        private interface ICtorParameterVerificationInfo
        {
        }

        #endregion

        #region Nested type: IgnoreParameter

        private sealed class IgnoreParameterConfig : ICtorParameterVerificationInfo
        {
            #region ctors

            public IgnoreParameterConfig(object defaultValue)
            {
                this.DefaultValue = defaultValue;
            }

            #endregion

            #region properties

            public object DefaultValue { get; }

            #endregion
        }

        #endregion

        #region Nested type: InstanceParameter

        private sealed class InstanceParameterConfig : ICtorParameterVerificationInfo
        {
            #region ctors

            public InstanceParameterConfig(object instance)
            {
                this.Instance = instance;
            }

            #endregion

            #region properties

            public object Instance { get; }

            #endregion
        }

        #endregion
    }
}
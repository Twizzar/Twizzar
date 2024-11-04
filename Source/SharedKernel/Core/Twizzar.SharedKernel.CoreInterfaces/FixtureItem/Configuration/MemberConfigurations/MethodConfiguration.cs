using System;
using System.Collections.Generic;
using System.Linq;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations
{
    /// <summary>
    /// Configuration for methods with the same name.
    /// </summary>
    public record MethodConfiguration : MemberConfiguration<MethodConfiguration>
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodConfiguration"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="methodName"></param>
        /// <param name="source"></param>
        /// <param name="returnValue"></param>
        /// <param name="parameterTypes"></param>
        /// <param name="returnType"></param>
        private MethodConfiguration(
            string name,
            string methodName,
            IConfigurationSource source,
            IMemberConfiguration returnValue,
            string returnType,
            string[] parameterTypes)
            : base(name, source)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(name, nameof(name))
                .Parameter(methodName, nameof(methodName))
                .Parameter(source, nameof(source))
                .Parameter(returnValue, nameof(returnValue))
                .Parameter(returnType, nameof(returnType))
                .Parameter(parameterTypes, nameof(parameterTypes))
                .ThrowWhenNull();

            this.ReturnValue = returnValue;
            this.ParameterTypes = parameterTypes;
            this.ReturnType = returnType;
            this.Source = source;
            this.MethodName = methodName;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the return value configuration.
        /// </summary>
        public IMemberConfiguration ReturnValue { get; init; }

        /// <summary>
        /// Gets the types of the parameter.
        /// </summary>
        public string[] ParameterTypes { get; init; }

        /// <summary>
        /// Gets the return type.
        /// </summary>
        public string ReturnType { get; init; }

        /// <summary>
        /// Gets the method name.
        /// </summary>
        public string MethodName { get; init; }

        #endregion

        #region members

        /// <summary>
        /// Create a new <see cref="MethodConfiguration"/> form <see cref="Type"/> information.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="methodName"></param>
        /// <param name="source"></param>
        /// <param name="returnValue"></param>
        /// <param name="returnType"></param>
        /// <param name="parameterTypeNames"></param>
        /// <returns></returns>
        public static MethodConfiguration Create(
            string name,
            string methodName,
            IConfigurationSource source,
            IMemberConfiguration returnValue,
            Type returnType,
            params string[] parameterTypeNames) =>
            new(
                name,
                methodName,
                source,
                returnValue.WithSource(source),
                returnType.FullName,
                parameterTypeNames.ToArray());

        /// <summary>
        /// Create a new <see cref="MethodConfiguration"/> form a <see cref="IMethodDescription"/>.
        /// </summary>
        /// <param name="methodDescription"></param>
        /// <param name="source"></param>
        /// <param name="returnValue"></param>
        /// <returns></returns>
        public static MethodConfiguration Create(
            IMethodDescription methodDescription,
            IConfigurationSource source,
            IMemberConfiguration returnValue) =>
            new(
                methodDescription.UniqueName,
                methodDescription.Name,
                source,
                returnValue.WithSource(source),
                methodDescription.TypeFullName.FullName,
                methodDescription.DeclaredParameters
                    .Select(description => description.TypeFullName.GetTypeName())
                    .ToArray());

        /// <inheritdoc />
        protected override IEnumerable<object> GetAdditionalEqualityComponents()
        {
            yield return this.ParameterTypes;
            yield return this.ReturnType;
            yield return this.ReturnValue;
        }

        #endregion
    }
}
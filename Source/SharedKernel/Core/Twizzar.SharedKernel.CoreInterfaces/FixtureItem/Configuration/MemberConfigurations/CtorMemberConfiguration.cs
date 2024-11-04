using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.CoreInterfaces.Util;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations
{
    /// <summary>
    /// Member configuration for configure the used constructor.
    /// </summary>
    public record CtorMemberConfiguration :
        MemberConfiguration<CtorMemberConfiguration>,
        IMergeable
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="CtorMemberConfiguration"/> class.
        /// </summary>
        /// <param name="constructorParameters">The constructor parameter configurations.</param>
        /// <param name="constructorSignature">The constructor signature.</param>
        /// <param name="source"></param>
        public CtorMemberConfiguration(
            IEnumerable<IMemberConfiguration> constructorParameters,
            ImmutableArray<ITypeFullName> constructorSignature,
            IConfigurationSource source)
            : this(
                constructorParameters?.ToImmutableDictionary(configuration => configuration.Name),
                constructorSignature,
                source)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CtorMemberConfiguration"/> class.
        /// </summary>
        /// <param name="constructorParameters">The constructor parameter configurations.</param>
        /// <param name="constructorSignature">The constructor signature.</param>
        /// <param name="source"></param>
        public CtorMemberConfiguration(
            IImmutableDictionary<string, IMemberConfiguration> constructorParameters,
            ImmutableArray<ITypeFullName> constructorSignature,
            IConfigurationSource source)
            : base(ConfigurationConstants.CtorMemberName, source)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(source, nameof(source))
                .Parameter(constructorParameters, nameof(constructorParameters))
                .ThrowWhenNull();

            this.ConstructorParameters = constructorParameters;
            this.ConstructorSignature = constructorSignature;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the constructor parameters.
        /// </summary>
        public IImmutableDictionary<string, IMemberConfiguration> ConstructorParameters { get; init; }

        /// <summary>
        /// Gets the constructor signature.
        /// </summary>
        public ImmutableArray<ITypeFullName> ConstructorSignature { get; init; }

        #endregion

        #region members

        /// <summary>
        /// Create a new <see cref="CtorMemberConfiguration"/>. Where the ConstructorSignature is not set.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="constructorParameters"></param>
        /// <returns>A new instance of <see cref="CtorMemberConfiguration"/>.</returns>
        public static CtorMemberConfiguration Create(
            IConfigurationSource source,
            params IMemberConfiguration[] constructorParameters) =>
            new(constructorParameters, ImmutableArray<ITypeFullName>.Empty, source);

        /// <summary>
        /// Create a new CtorMemberConfiguration with a new parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="parameterConfig">The parameter configuration.</param>
        /// <returns>A copy of the old configuration with the new parameter.</returns>
        public CtorMemberConfiguration WithParameter(string name, IMemberConfiguration parameterConfig)
        {
            EnsureHelper.GetDefault.Parameter(parameterConfig, nameof(parameterConfig)).ThrowWhenNull();
            EnsureHelper.GetDefault.Parameter(name, nameof(name))
                .IsNotNull()
                .IsTrue(
                    n => this.ConstructorParameters?.ContainsKey(n) == true,
                    $"{this.ConstructorParameters} does not contain a parameter with the name {name}")
                .ThrowOnFailure();

            return new CtorMemberConfiguration(
                this.ConstructorParameters.SetItem(name, parameterConfig),
                this.ConstructorSignature,
                this.Source);
        }

        /// <inheritdoc />
        public object Merge(object b)
        {
            if (b is not CtorMemberConfiguration otherCtor)
            {
                throw new InvalidOperationException(
                    $"b is not of type {nameof(CtorMemberConfiguration)} it is of type {b.GetType().Name}");
            }

            var merged = this.ConstructorParameters
                .Merge(otherCtor.ConstructorParameters);

            return new CtorMemberConfiguration(merged, this.ConstructorSignature, otherCtor.Source);
        }

        /// <inheritdoc />
        protected override IEnumerable<object> GetAdditionalEqualityComponents()
        {
            yield return this.ConstructorParameters;
            yield return this.ConstructorSignature;
        }

        #endregion
    }
}
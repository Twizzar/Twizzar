using System.Collections.Generic;
using System.Linq;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Services;
using Twizzar.Runtime.CoreInterfaces.Resources;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.ExceptionBuilders;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.Infrastructure.DomainService
{
    /// <summary>
    /// The hosted fixture container.
    /// </summary>
    public class FixtureItemContainer : IFixtureItemContainer
    {
        #region fields

        private readonly IResolver _resolver;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemContainer"/> class.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="registeredCodeInstanceContainer">Th know code instance container.</param>
        public FixtureItemContainer(
            IResolver resolver,
            IRegisteredCodeInstanceContainer registeredCodeInstanceContainer)
        {
            this.EnsureMany()
                .Parameter(resolver, nameof(resolver))
                .Parameter(registeredCodeInstanceContainer, nameof(registeredCodeInstanceContainer))
                .ThrowWhenNull();

            this._resolver = this.EnsureCtorParameterIsNotNull(resolver, nameof(resolver));
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public T GetInstance<T>(IItemConfig<T> config) =>
            this.GetInstanceSafe<T>(
                Maybe.ToMaybe(config)
                    .Map(itemConfig => itemConfig.Name));

        /// <inheritdoc />
        public T GetInstance<T>() =>
            this.GetInstanceSafe<T>(Maybe.None());

        /// <inheritdoc />
        public IEnumerable<T> GetInstances<T>(int count) => this.GetInstances<T>(count, null);

        /// <inheritdoc />
        public IEnumerable<T> GetInstances<T>(int count, IItemConfig<T> config)
        {
            this.EnsureParameter(count, nameof(count))
                .IsGreaterEqualThan(
                    0,
                    DefaultExceptionBuilder.ArgumentOutOfRangeExceptionBuilder(
                        ErrorMessagesRuntime.CanNotBeLessThanZero))
                .ThrowOnFailure();

            return Enumerable.Range(0, count).Select(i => this.GetInstance(config));
        }

        /// <inheritdoc />
        public void SetInstance<T>(T instance)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void SetInstance<T>(T instance, IItemConfig<T> config)
        {
            throw new System.NotImplementedException();
        }

        private T GetInstanceSafe<T>(Maybe<string> definitionId) =>
            definitionId.Match(
                some: s => this._resolver.ResolveNamed<T>(s),
                none: () => this._resolver.Resolve<T>());

        #endregion
    }
}
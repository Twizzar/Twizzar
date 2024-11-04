using System;
using System.Collections.Generic;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.Core.FixtureItem.Configuration
{
    /// <summary>
    /// The container which holds the instances set by SetInstance.
    /// </summary>
    public class RegisteredCodeInstanceContainer : IRegisteredCodeInstanceContainer
    {
        private readonly IBaseTypeService _baseTypeService;

        #region fields

        private readonly Dictionary<FixtureItemId, object> _knownInstance = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredCodeInstanceContainer"/> class.
        /// </summary>
        /// <param name="baseTypeService"></param>
        public RegisteredCodeInstanceContainer(IBaseTypeService baseTypeService)
        {
            this.EnsureParameter(baseTypeService, nameof(baseTypeService)).ThrowWhenNull();
            this._baseTypeService = baseTypeService;
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
        public void Add(FixtureItemId id, object instance)
        {
            this.EnsureParameter(id, nameof(id)).ThrowWhenNull();

            if (this._baseTypeService.IsBaseType(id.TypeFullName) ||
                this._baseTypeService.IsNullableBaseType(id.TypeFullName))
            {
                throw new InvalidOperationException("BaseTypes are not supported.");
            }

            if (this._knownInstance.ContainsKey(id))
            {
                var nameMessage = id.Name.Match(
                    name => $"the definition name {name}",
                    () => "no definition name");

                throw new InvalidOperationException($"The Instance of the type {id.TypeFullName} and {nameMessage} was already set at {id.ToPathString()}.");
            }

            this._knownInstance.Add(id, instance);
        }

        /// <inheritdoc />
        public Maybe<object> Get(FixtureItemId id) =>
            this.EnsureParameter(id, nameof(id))
                .IsNotNull()
                .MatchOrThrow(() => this._knownInstance.GetMaybe(id));

        #endregion
    }
}
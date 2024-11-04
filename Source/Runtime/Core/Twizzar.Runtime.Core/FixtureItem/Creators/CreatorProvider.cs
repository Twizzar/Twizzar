using System;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;

namespace Twizzar.Runtime.Core.FixtureItem.Creators
{
    /// <inheritdoc />
    public class CreatorProvider : ICreatorProvider
    {
        private readonly IBaseTypeCreator _baseTypeCreator;
        private readonly IMoqCreator _moqCreator;
        private readonly IConcreteTypeCreator _concreteTypeCreator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatorProvider"/> class.
        /// </summary>
        /// <param name="baseTypeCreator">The base type creator service.</param>
        /// <param name="moqCreator">The moq creator service.</param>
        /// <param name="concreteTypeCreator">The concrete type creator service.</param>
        public CreatorProvider(
            IBaseTypeCreator baseTypeCreator,
            IMoqCreator moqCreator,
            IConcreteTypeCreator concreteTypeCreator)
        {
            this._baseTypeCreator = baseTypeCreator;
            this._moqCreator = moqCreator;
            this._concreteTypeCreator = concreteTypeCreator;
        }

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public ICreator GetCreator(IFixtureItemDefinitionNode fixtureItemDefinition) =>
            fixtureItemDefinition switch
            {
                IBaseTypeNode _ => this._baseTypeCreator,
                IClassNode _ => this._concreteTypeCreator,
                IMockNode _ => this._moqCreator,
                _ => throw new ArgumentOutOfRangeException(nameof(fixtureItemDefinition)),
            };

        #endregion
    }
}

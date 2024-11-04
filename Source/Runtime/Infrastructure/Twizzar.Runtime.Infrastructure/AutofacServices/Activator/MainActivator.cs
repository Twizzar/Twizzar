using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.Services;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Name;
using Twizzar.Runtime.Infrastructure.AutofacServices.Creator;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.Infrastructure.AutofacServices.Activator
{
    /// <summary>
    /// The Core element for autofac activation. The class will
    /// use the autofac system to initialize the instances and acts as a
    /// central point between the autofac classes and the Twizzar creator.
    /// Implements the <see cref="IInstanceActivator"/> interface from autofac itself.
    /// </summary>
    public class MainActivator : InstanceActivator, IInstanceActivator, IHasLogger, IHasEnsureHelper
    {
        #region fields

        private readonly ICreatorProvider _creatorProvider;
        private readonly Maybe<string> _definitionId;

        private readonly IFixtureItemDefinitionQuery _fixtureItemDefinitionQuery;
        private readonly IRegisteredCodeInstanceContainer _registeredCodeInstanceContainer;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainActivator"/> class.
        /// </summary>
        /// <param name="type">The type for the given activator.</param>
        /// <param name="definitionId">The definition id for named resolving.</param>
        /// <param name="fixtureItemDefinitionQuery">The fixture item definition query.</param>
        /// <param name="creatorInfoProvider">The creator info provider.</param>
        /// <param name="registeredCodeInstanceContainer"></param>
        public MainActivator(
            Type type,
            Maybe<string> definitionId,
            IFixtureItemDefinitionQuery fixtureItemDefinitionQuery,
            ICreatorProvider creatorInfoProvider,
            IRegisteredCodeInstanceContainer registeredCodeInstanceContainer)
            : base(type)
        {
            this.EnsureMany()
                .Parameter(fixtureItemDefinitionQuery, nameof(fixtureItemDefinitionQuery))
                .Parameter(creatorInfoProvider, nameof(creatorInfoProvider))
                .Parameter(registeredCodeInstanceContainer, nameof(registeredCodeInstanceContainer))
                .ThrowWhenNull();

            this._fixtureItemDefinitionQuery = fixtureItemDefinitionQuery;
            this._creatorProvider = creatorInfoProvider;
            this._registeredCodeInstanceContainer = registeredCodeInstanceContainer;
            this._definitionId = definitionId;
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
        public object ActivateInstance(IComponentContext context, IEnumerable<Parameter> parameters)
        {
            this.EnsureMany()
                .Parameter(context, nameof(context))
                .Parameter(parameters, nameof(parameters))
                .ThrowWhenNull();

            // type already activated, then return the element in parameters.
            var activated = parameters
                .OfType<TypedParameter>()
                .FirstOrDefault(e =>
                    e.Type == this.LimitType);

            if (activated != null)
            {
                return activated.Value;
            }

            var id = this._definitionId.Match(
                some: name =>
                    FixtureItemId.CreateNamed(
                        name,
                        this.LimitType.ToTypeFullName()),
                none:
                FixtureItemId.CreateNameless(
                    this.LimitType.ToTypeFullName()));

            // Check if this id is a know instance set by SetInstance when yes return it.
            if (this._registeredCodeInstanceContainer.Get(id).AsMaybeValue() is SomeValue<object> knownInstance)
            {
                return knownInstance.Value;
            }

            var definition = this._fixtureItemDefinitionQuery.GetDefinitionNode(id).Result;
            var creator = this._creatorProvider.GetCreator(definition);

            // check if environment of the creator need to be updated
            if (creator is AutofacCreator autofacCreator)
            {
                autofacCreator.Update(context, parameters, context.ResolveNamed, context.Resolve);
            }

            // build instance from creatorInfo and return the newly created element
            return creator.CreateInstance(definition);
        }

        #endregion
    }
}
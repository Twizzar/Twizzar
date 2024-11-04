using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Twizzar.Runtime.CoreInterfaces.Exceptions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Name;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Services;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;

namespace Twizzar.Runtime.Infrastructure.AutofacServices.Creator
{
    /// <summary>
    /// Abstract crass for creator using the Autofac resolve and resolve named methods.
    /// </summary>
    public abstract class AutofacCreator : ICreator
    {
        #region fields

        private readonly IInstanceCacheRegistrant _instanceCacheRegistrant;
        private readonly IBaseTypeCreator _baseTypeCreator;

        private Maybe<Func<string, Type, object>> _resolverNamed;
        private Maybe<Func<Type, object>> _resolver;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacCreator"/> class.
        /// </summary>
        /// <param name="instanceCacheRegistrant"></param>
        /// <param name="baseTypeCreator"></param>
        protected AutofacCreator(
            IInstanceCacheRegistrant instanceCacheRegistrant,
            IBaseTypeCreator baseTypeCreator)
        {
            this.EnsureMany()
                .Parameter(instanceCacheRegistrant, nameof(instanceCacheRegistrant))
                .Parameter(baseTypeCreator, nameof(baseTypeCreator))
                .ThrowWhenNull();

            this._instanceCacheRegistrant = instanceCacheRegistrant;
            this._baseTypeCreator = baseTypeCreator;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <summary>
        /// Gets the Autofac context.
        /// </summary>
        protected Maybe<IComponentContext> AutofacContext { get; private set; }

        /// <summary>
        /// Gets the Autofac parameters.
        /// </summary>
        protected Maybe<IEnumerable<Parameter>> AutofacParameters { get; private set; }

        #endregion

        #region members

        /// <summary>
        /// Updates the state of the given class instance and keeps the autofac environment locally.
        /// </summary>
        /// <param name="autofacContext">The autofac context.</param>
        /// <param name="autofacParameters">An enumerable of autofac parameters.</param>
        /// <param name="resolverNamed">ResolveNamed func from autofac.</param>
        /// <param name="resolver">Resolve func form autofac.</param>
        public void Update(
            IComponentContext autofacContext,
            IEnumerable<Parameter> autofacParameters,
            Func<string, Type, object> resolverNamed,
            Func<Type, object> resolver)
        {
            this.EnsureMany()
                .Parameter(autofacContext, nameof(autofacContext))
                .Parameter(autofacParameters, nameof(autofacParameters))
                .Parameter(resolverNamed, nameof(resolverNamed))
                .Parameter(resolver, nameof(resolver))
                .ThrowWhenNull();

            this.AutofacContext = Some(autofacContext);
            this.AutofacParameters = Some(autofacParameters);
            this._resolverNamed = Some(resolverNamed);
            this._resolver = Some(resolver);
        }

        /// <summary>
        /// Resolve a type with autofac.
        /// </summary>
        /// <param name="id">The fixture id of the type.</param>
        /// <param name="type">The type.</param>
        /// <returns>An instance of the type.</returns>
        public object Resolve(FixtureItemId id, Type type)
        {
            this.EnsureMany()
                .Parameter(id, nameof(id))
                .Parameter(type, nameof(type))
                .ThrowWhenNull();

            var instance =
                from resolveName in this._resolverNamed
                from resolve in this._resolver
                select id.Name.Match(
                    some: name => resolveName(name, type),
                    none:
                    () => resolve(type));

            if (instance.AsMaybeValue() is SomeValue<object> o)
            {
                id.Name.IfSome(s => this._instanceCacheRegistrant.Register(s, o.Value));
                return o.Value;
            }
            else
            {
                throw new ResolveTypeException(
                    $"the {nameof(this.Update)} method needs to be called before calling {nameof(this.Resolve)}");
            }
        }

        /// <inheritdoc />
        public abstract object CreateInstance(IFixtureItemDefinitionNode definition);

        /// <summary>
        /// Register an instance to the instant cache registrant.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="id"></param>
        protected void RegisterInstance(object instance, FixtureItemId id)
        {
            id.Name.IfSome(s => this._instanceCacheRegistrant.Register(s, instance));
        }

        /// <summary>
        /// Resolve a <see cref="IValueDefinition"/> with autofac.
        /// </summary>
        /// <param name="definition">The value definition.</param>
        /// <param name="description">The description of the type.</param>
        /// <param name="parentId">The id of the parent of these member.</param>
        /// <returns>The resolved instance.</returns>
        protected object ResolveType(
            IValueDefinition definition,
            IBaseDescription description,
            FixtureItemId parentId)
        {
            this.EnsureParameter(definition, nameof(definition)).ThrowWhenNull();

            return definition switch
            {
                IRawValueDefinition _ => this.CreateBaseTypeInstance(definition, description, parentId),
                IUniqueDefinition _ => this.CreateBaseTypeInstance(definition, description, parentId),
                INullValueDefinition _ => this.CreateBaseTypeInstance(definition, description, parentId),
                ILinkDefinition x =>
                    this.Resolve(x.Link, ((TypeFullName)x.Link.TypeFullName).Type),

            _ => throw new ResolveTypeException(
                $"ValueDefinition is of the unknown type {definition.GetType().Name} at {parentId.ToPathString()}."),

            };
        }

        private object CreateBaseTypeInstance(
            IValueDefinition definition,
            IBaseDescription description,
            FixtureItemId parentId)
        {
            var value = this._baseTypeCreator.CreateInstance(definition, description);

            if (value is INullValue)
            {
                value = null;
            }

            parentId.Name.IfSome(s =>
                this._instanceCacheRegistrant.Register(s + "." + description.GetMemberPathName(), value));

            return value;
        }

        #endregion
    }
}
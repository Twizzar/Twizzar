using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.FixtureItem.Definition;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.CoreInterfaces.Resources;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.Failures;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.FixtureConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional;
using ViCommon.Functional.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;
using static Twizzar.Design.Core.Command.FixtureItem.Definition.FixtureItemDefinitionNodeHelper;
using static ViCommon.Functional.FunctionalCommon;
using IResult = ViCommon.Functional.Monads.ResultMonad.IResult<ViCommon.Functional.Unit,
    ViCommon.Functional.Monads.ResultMonad.Failure>;

namespace Twizzar.Design.Core.Command.FixtureItem.Definition
{
    /// <inheritdoc cref="IFixtureItemDefinitionNode" />
    public class FixtureItemDefinitionNode : Entity<FixtureItemDefinitionNode, FixtureItemId>,
        IFixtureItemDefinitionNode
    {
        #region fields

        private readonly IConfigurationItemFactory _configurationItemFactory;
        private readonly IEventBus _eventBus;
        private readonly IEventStore _eventStore;
        private readonly IDocumentFileNameQuery _documentFileNameQuery;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemDefinitionNode"/> class.
        /// </summary>
        /// <param name="id">The fixture item id.</param>
        /// <param name="typeDescription">The type description.</param>
        /// <param name="systemDefault">The system default service.</param>
        /// <param name="configurationItemFactory">The configuration item factory.</param>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="eventStore">The event store.</param>
        /// <param name="documentFileNameQuery"></param>
        public FixtureItemDefinitionNode(
            FixtureItemId id,
            ITypeDescription typeDescription,
            ISystemDefaultService systemDefault,
            IConfigurationItemFactory configurationItemFactory,
            IEventBus eventBus,
            IEventStore eventStore,
            IDocumentFileNameQuery documentFileNameQuery)
            : base(id)
        {
            this.EnsureMany()
                .Parameter(id, nameof(id))
                .Parameter(typeDescription, nameof(typeDescription))
                .Parameter(systemDefault, nameof(systemDefault))
                .Parameter(documentFileNameQuery, nameof(documentFileNameQuery))
                .ThrowWhenNull();

            this.FixtureItemId = id;
            this.TypeDescription = typeDescription;
            this._configurationItemFactory = configurationItemFactory;
            this._eventBus = eventBus;
            this._eventStore = eventStore;
            this._documentFileNameQuery = documentFileNameQuery;

            this.ConfigurationItem = systemDefault.GetDefaultConfigurationItem(typeDescription, id.RootItemPath)
                .Match(
                    onSuccess: Identity,
                    onFailure: failure => throw new InvalidTypeDescriptionException(failure.Message));
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public FixtureItemId FixtureItemId { get; private set; }

        /// <inheritdoc />
        public ITypeDescription TypeDescription { get; }

        /// <inheritdoc />
        public IConfigurationItem ConfigurationItem { get; private set; }

        #endregion

        #region members

        /// <inheritdoc />
        public IResult Replay(IFixtureItemEvent e)
        {
            this.EnsureParameter(e, nameof(e)).ThrowWhenNull();

            if (!e.FixtureItemId.Equals(this.FixtureItemId))
            {
                return Result.Success<Failure>();
            }

            return e switch
            {
                FixtureItemMemberChangedEvent x => this.Replay(x),
                FixtureItemCreatedEvent x => this.Replay(x),
                FixtureItemConfigurationStartedEvent _ => Result.Success<Failure>(),
                IEventFailed _ => Result.Success<Failure>(),
                _ => throw this.LogAndReturn(new ArgumentOutOfRangeException(nameof(e))),
            };
        }

        /// <inheritdoc />
        public IResult Replay(IEnumerable<IFixtureItemEvent> es) =>
            es.Select(this.Replay).Aggregate();

        /// <inheritdoc />
        public async Task CreateNamedFixtureItem()
        {
            await this.CreateNamedFixtureItemStep()
                .DoAsync(
                    successEvent => this._eventBus.PublishAsync(successEvent),
                    failure => this._eventBus.PublishAsync(failure.Event));
        }

        /// <inheritdoc />
        public async Task ChangeMemberConfiguration(IMemberConfiguration memberConfiguration)
        {
            this.EnsureParameter(memberConfiguration, nameof(memberConfiguration)).ThrowWhenNull();

            var fileName = await this._documentFileNameQuery.GetDocumentFileName(this.FixtureItemId.RootItemPath);

            if (fileName.AsResultValue() is FailureValue<Failure> failure)
            {
                await this._eventBus.PublishAsync(
                    new FixtureItemMemberChangedFailedEvent(
                        this.FixtureItemId,
                        memberConfiguration,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            failure.Value.Message,
                            memberConfiguration.Name)));
                return;
            }

            if (memberConfiguration.Source is not FromBuilderClass)
            {
                memberConfiguration =
                    memberConfiguration.WithSource(new FromBuilderClass(fileName.GetSuccessUnsafe()));
            }

            // check if the configuration item contains the member name.
            if (!this.ConfigurationItem.MemberConfigurations.ContainsKey(memberConfiguration.Name))
            {
                await this._eventBus.PublishAsync(
                    new FixtureItemMemberChangedFailedEvent(
                        this.FixtureItemId,
                        memberConfiguration,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            MessagesDesign.FixtureItemMemberChangedFailed_FixtureItemDoesNotContainMember,
                            memberConfiguration.Name)));
            }

            // Cannot change the configuration of a default link.
            else if (this.FixtureItemId.Name.IsNone)
            {
                await this._eventBus.PublishAsync(
                    new FixtureItemMemberChangedFailedEvent(
                        this.FixtureItemId,
                        memberConfiguration,
                        MessagesDesign.FixtureItemDefinitionNode_ChangeMemberConfiguration_Cannot_change_a_unnamed_fixture_item));
            }

            // All checks where successful.
            else
            {
                var tryCreateFixtureItemResult =
                    await this.TryCreateFixtureItemForLinkConfigIfMissingAsync(memberConfiguration);

                var result =
                    from createResult in await this.CreateIfNotExists()
                        .MapFailureAsync(
                            failure => new FailedEventFailure<FixtureItemMemberChangedFailedEvent>(
                                new FixtureItemMemberChangedFailedEvent(
                                    this.FixtureItemId,
                                    memberConfiguration,
                                    failure.Event.Reason)))
                    from changeMemberConfigurationResult in this.TryChangeMemberConfiguration(memberConfiguration)
                        .MapFailure(
                            failure => new FailedEventFailure<FixtureItemMemberChangedFailedEvent>(
                                new FixtureItemMemberChangedFailedEvent(
                                    this.FixtureItemId,
                                    memberConfiguration,
                                    failure.Message)))
                    from createFixtureItemResult in tryCreateFixtureItemResult
                    select (createResult, changeMemberConfigurationResult, createFixtureItemResult);

                await result.DoAsync(
                    async tuple =>
                    {
                        // create this fixture item if necessary
                        await tuple.createResult.IfSomeAsync(
                            e =>
                                this._eventBus.PublishAsync(e));

                        // create fixture item of the member or members (when ctor) if necessary
                        foreach (var maybe in tuple.createFixtureItemResult)
                        {
                            await maybe.IfSomeAsync(e => this._eventBus.PublishAsync(e));
                        }

                        this.ConfigurationItem = tuple.changeMemberConfigurationResult;

                        await this._eventBus.PublishAsync(
                            new FixtureItemMemberChangedEvent(this.FixtureItemId, memberConfiguration));
                    },
                    async failure => await this._eventBus.PublishAsync(failure.Event));
            }
        }

        /// <inheritdoc />
        protected override bool Equals(FixtureItemId a, FixtureItemId b) =>
            a?.Equals(b) ?? false;

        private async Task<IResult<FixtureItemCreatedEvent, FailedEventFailure<FixtureItemCreatedFailedEvent>>>
            CreateNamedFixtureItemStep()
        {
            // Cannot change the kind if the id is unnamed.
            if (this.FixtureItemId.Name.IsNone)
            {
                return Failure<FixtureItemCreatedEvent>(
                    new FixtureItemCreatedFailedEvent(
                        this.FixtureItemId,
                        MessagesDesign.FixtureItemDefinitionNode_CreateNamedFixtureItem_Cannot_change_kind_of_unnamed_Fixture_Item));
            }

            // Cannot create the fixture item when the name already exists.
            return await this.CheckIfNameAlreadyExists()
                .BindAsync(
                    result => Success(
                        new FixtureItemCreatedEvent(this.FixtureItemId)));
        }

        private async Task<IResult<Unit, FailedEventFailure<FixtureItemCreatedFailedEvent>>> CheckIfNameAlreadyExists()
        {
            if (await this.FindFixtureItemCreatedEventWithName(this.FixtureItemId)
                    .Map(maybe => maybe.AsMaybeValue())
                is SomeValue<FixtureItemCreatedEvent> createdEvent)
            {
                return Failure<Unit>(
                    new FixtureItemCreatedFailedEvent(
                        this.FixtureItemId,
                        string.Format(
                            MessagesDesign
                                .FixtureItemDefinitionNode_CreateNamedFixtureItem_Name__0__already_exists_on_type__1_,
                            this.FixtureItemId.Name.GetValueUnsafe(),
                            createdEvent.Value.FixtureItemId.TypeFullName)));
            }
            else
            {
                return Result.Success<Unit, FailedEventFailure<FixtureItemCreatedFailedEvent>>(
                    Unit.New);
            }
        }

        private Task<Maybe<FixtureItemCreatedEvent>> FindFixtureItemCreatedEventWithName(FixtureItemId fixtureItemId) =>
            fixtureItemId.RootItemPath.BindAsync(
                path =>
                    this._eventStore.FindAll<FixtureItemCreatedEvent>(path)
                        .Map(
                            x =>
                                x.FirstOrNone(e => e.FixtureItemId.Name == fixtureItemId.Name)));

        private async Task<bool> ExistsInEventStore(FixtureItemId fixtureItemId)
        {
            this.EnsureParameter(fixtureItemId, nameof(fixtureItemId)).ThrowWhenNull();

            var maybeEvent = await this._eventStore.FindLast<FixtureItemCreatedEvent>(fixtureItemId);
            return maybeEvent.IsSome;
        }

        private async Task<IResult<Maybe<FixtureItemCreatedEvent>, FailedEventFailure<FixtureItemCreatedFailedEvent>>>
            CreateIfNotExists()
        {
            if (!await this.ExistsInEventStore(this.FixtureItemId))
            {
                return await this.CreateNamedFixtureItemStep()
                    .MapSuccessAsync(Maybe.Some);
            }

            return Result.Success<Maybe<FixtureItemCreatedEvent>, FailedEventFailure<FixtureItemCreatedFailedEvent>>(
                Maybe.None());
        }

        private IResult Replay(FixtureItemMemberChangedEvent e)
        {
            var result = this.TryChangeMemberConfiguration(e.MemberConfiguration);
            result.IfSuccess(item => this.ConfigurationItem = item);
            return result.MapSuccess(item => Unit.New);
        }

        private IResult Replay(FixtureItemCreatedEvent e) =>
            this.ChangeType(e.FixtureItemId.TypeFullName);

        private IResult ChangeType(ITypeFullName type)
        {
            this.FixtureItemId = this.FixtureItemId.WithType(type);
            return Result.Success<Unit, Failure>(Unit.New);
        }

        private async Task<IResult<IEnumerable<Maybe<FixtureItemCreatedEvent>>,
                FailedEventFailure<FixtureItemMemberChangedFailedEvent>>>
            TryCreateFixtureItemForLinkConfigIfMissingAsync(IMemberConfiguration rootMemberConfiguration)
        {
            var x = this.TryCreateFixtureItemIfMissingAsync(rootMemberConfiguration);

            var tryCreateFixtureItemResult =
                Result
                    .Success<IEnumerable<Maybe<FixtureItemCreatedEvent>>,
                        FailedEventFailure<FixtureItemMemberChangedFailedEvent>>(
                        Enumerable.Empty<Maybe<FixtureItemCreatedEvent>>());

            foreach (var task in x)
            {
                var r = await task;

                if (r.IsSuccess)
                {
                    tryCreateFixtureItemResult = tryCreateFixtureItemResult
                        .MapSuccess(maybes => maybes.Append(r.GetSuccessUnsafe()));
                }
                else
                {
                    tryCreateFixtureItemResult = Result.Failure<IEnumerable<Maybe<FixtureItemCreatedEvent>>,
                        FailedEventFailure<FixtureItemMemberChangedFailedEvent>>(r.GetFailureUnsafe());

                    break;
                }
            }

            return tryCreateFixtureItemResult;
        }

        private
            IEnumerable<Task<IResult<Maybe<FixtureItemCreatedEvent>,
                FailedEventFailure<FixtureItemMemberChangedFailedEvent>>>>
            TryCreateFixtureItemIfMissingAsync(IMemberConfiguration rootMemberConfiguration)
        {
            return rootMemberConfiguration switch
            {
                CtorMemberConfiguration x => x.ConstructorParameters.Values
                    .OfType<LinkMemberConfiguration>()
                    .Where(configuration => configuration.ConfigurationLink.Name.IsSome)
                    .Select(
                        async configuration =>
                            await this.TryCreateFixtureItemIfMissingAsync(configuration)),
                MethodConfiguration { ReturnValue: LinkMemberConfiguration link } when link.ConfigurationLink.Name.IsSome =>
                    new[] { this.TryCreateFixtureItemIfMissingAsync(link) },
                LinkMemberConfiguration x => new[]
                {
                    this.TryCreateFixtureItemIfMissingAsync(x),
                },
                _ => Enumerable
                    .Empty<Task<IResult<Maybe<FixtureItemCreatedEvent>,
                        FailedEventFailure<FixtureItemMemberChangedFailedEvent>>>>(),
            };
        }

        private async
            Task<IResult<Maybe<FixtureItemCreatedEvent>, FailedEventFailure<FixtureItemMemberChangedFailedEvent>>>
            TryCreateFixtureItemIfMissingAsync(
                LinkMemberConfiguration linkMemberConfiguration)
        {
            // check if the id does not exists. And it is not a default link.
            if (linkMemberConfiguration.ConfigurationLink.Name.IsSome &&
                !await this.ExistsInEventStore(linkMemberConfiguration.ConfigurationLink))
            {
                // If the link points to a non existing FixtureItem create it.

                return Result
                    .Success<Maybe<FixtureItemCreatedEvent>,
                        FailedEventFailure<FixtureItemMemberChangedFailedEvent>>(
                        new FixtureItemCreatedEvent(linkMemberConfiguration.ConfigurationLink));
            }

            return Result
                .Success<Maybe<FixtureItemCreatedEvent>, FailedEventFailure<FixtureItemMemberChangedFailedEvent>>(
                    Maybe.None());
        }

        private IResult<IConfigurationItem, InvalidConfigurationFailure> TryChangeMemberConfiguration(
            IMemberConfiguration memberConfiguration)
        {
            var members = ImmutableDictionary<string, IMemberConfiguration>.Empty
                .Add(memberConfiguration.Name, memberConfiguration);

            var config = this._configurationItemFactory.CreateConfigurationItem(
                this.FixtureItemId,
                ImmutableDictionary<string, IFixtureConfiguration>.Empty,
                members,
                ImmutableDictionary.Create<string, IImmutableList<object>>());

            return this.ConfigurationItem.Merge(config);
        }

        #endregion
    }
}
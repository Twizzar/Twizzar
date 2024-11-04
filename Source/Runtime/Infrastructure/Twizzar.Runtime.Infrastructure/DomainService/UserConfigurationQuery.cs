using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Twizzar.Fixture;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

#pragma warning disable S2743 // Static fields should not be used in generic types

namespace Twizzar.Runtime.Infrastructure.DomainService
{
    /// <inheritdoc />
    public class UserConfigurationQuery<T> : IUserConfigurationQuery
    {
        #region static fields and constants

        private static readonly IConfigurationSource Source = new FromRuntimeConfig();

        #endregion

        #region fields

        private readonly ImmutableDictionary<FixtureItemId, IConfigurationItem> _userConfigurationItems =
            ImmutableDictionary<FixtureItemId, IConfigurationItem>.Empty;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserConfigurationQuery{T}"/> class.
        /// </summary>
        /// <param name="itemConfig"></param>
        /// <param name="configurationItemFactory"></param>
        public UserConfigurationQuery(
            Maybe<IItemConfig<T>> itemConfig,
            IConfigurationItemFactory configurationItemFactory)
        {
            this.EnsureMany()
                .Parameter(itemConfig, nameof(itemConfig))
                .Parameter(configurationItemFactory, nameof(configurationItemFactory))
                .ThrowWhenNull();

            var builder =
                ImmutableDictionary.CreateBuilder<FixtureItemId, IConfigurationItem>(new FixtureItemIdComparer());

            if (itemConfig.AsMaybeValue() is SomeValue<IItemConfig<T>> someConfig)
            {
                var config = someConfig.Value;

                foreach (var (path, viConfig) in config.MemberConfigurations)
                {
                    var (oldId, configurationItem) =
                        GetOrCreateConfigurationItem(path, config.Name, builder, configurationItemFactory);

                    var memberConfig = MapToMemberConfiguration(viConfig);
                    configurationItem = configurationItem.AddOrUpdateMemberConfiguration(memberConfig);
                    UpdateRepository(builder, oldId, configurationItem);
                }

                foreach (var (path, callbacks) in config.Callbacks)
                {
                    var (oldId, configurationItem) =
                        GetOrCreateConfigurationItem(path, config.Name, builder, configurationItemFactory);

                    configurationItem = configurationItem.AddCallbacks(path.UniqueMemberName, callbacks);
                    UpdateRepository(builder, oldId, configurationItem);
                }

                this._userConfigurationItems = builder.ToImmutable();
            }
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
        public Task<Maybe<IConfigurationItem>> GetNamedConfig(FixtureItemId id) =>
            Task.FromResult(this._userConfigurationItems.GetMaybe(id));

        private static void UpdateRepository(
            IDictionary<FixtureItemId, IConfigurationItem> configRepository,
            FixtureItemId id,
            IConfigurationItem configurationItem)
        {
            if (configRepository.ContainsKey(id))
            {
                configRepository.Remove(id);
                configRepository.Add(id, configurationItem);
            }
            else
            {
                configRepository.Add(id, configurationItem);
            }
        }

        private static (FixtureItemId OldId, IConfigurationItem ConfigurationItem) GetOrCreateConfigurationItem(
            IMemberPath<T> path,
            string rootItemPath,
            IDictionary<FixtureItemId, IConfigurationItem> configRepository,
            IConfigurationItemFactory configurationItemFactory)
        {
            // then the parent is Car.Engine.Cylinder
            var parentMemberPath = path.Parent.SomeOrProvided(
                () => throw new InternalException($"Path {path.UniqueName} has no parent."));

            // create the fixture id for the parent for example Car.Engine.Cylinder of type ICylinder
            var parentId = FixtureItemId.CreateNamed(
                    parentMemberPath.UniqueName,
                    parentMemberPath.ReturnType.ToTypeFullName())
                .WithRootItemPath(rootItemPath);

            // most of the time the same as parentId.
            var newId = FixtureItemId.CreateNamed(
                    parentMemberPath.UniqueName,
                    parentMemberPath.DeclaredType.ToTypeFullName())
                .WithRootItemPath(rootItemPath);

            CreateAncestorToRoot(configurationItemFactory, parentMemberPath, configRepository, parentId);

            // the member configuration must be added to a configuration item
            // first check if the configuration item was already created else create a new one.
            var config = configRepository.GetValueMaybe(parentId)
                .SomeOrProvided(
                    () =>
                        configurationItemFactory.CreateConfigurationItem(newId));

            return (parentId, config.WithId(newId));
        }

        private static void CreateAncestorToRoot(
            IConfigurationItemFactory configurationItemFactory,
            IMemberPath<T> childMemberPath,
            IDictionary<FixtureItemId, IConfigurationItem> builder,
            FixtureItemId childId)
        {
            // the grandparent Car.Engine
            var currentPath = childMemberPath.Parent;

            while (currentPath.AsMaybeValue() is SomeValue<IMemberPath<T>> memberPath)
            {
                var path = memberPath.Value;

                var id = FixtureItemId.CreateNamed(
                        path.UniqueName,
                        path.DeclaredType.ToTypeFullName())
                    .WithRootItemPath(childId.RootItemPath);

                var configurationItem = builder.GetValueMaybe(id)
                    .SomeOrProvided(() => configurationItemFactory.CreateConfigurationItem(id));

                var linkConfig = new LinkMemberConfiguration(childMemberPath.MemberName, childId, Source);
                IMemberConfiguration memberConfiguration = linkConfig;

                if (childMemberPath is ICtorParamMemberPath<T> ctorPath)
                {
                    var ctor = configurationItem.CtorConfiguration
                        .Map(configuration => (CtorMemberConfiguration)configuration)
                        .SomeOrProvided(
                            () => CtorMemberConfiguration.Create(Source));

                    ctor = new CtorMemberConfiguration(
                        ctor.ConstructorParameters.SetItem(ctorPath.MemberName, linkConfig),
                        ctor.ConstructorSignature,
                        Source);

                    memberConfiguration = ctor;
                }
                else if (childMemberPath is IMethodMemberPath<T> methodMemberPath)
                {
                    var returnType = methodMemberPath.DeclaredType;
                    var parameterTypes = methodMemberPath.Parameters.Select(p => p.DeclaredTypeName).ToArray();

                    memberConfiguration = MethodConfiguration.Create(
                        childMemberPath.UniqueMemberName,
                        childMemberPath.MemberName,
                        Source,
                        linkConfig,
                        returnType,
                        parameterTypes);
                }

                builder.Remove(id);
                builder.Add(id, configurationItem.AddOrUpdateMemberConfiguration(memberConfiguration));

                childMemberPath = path;
                childId = id;
                currentPath = path.Parent;
            }
        }

        private static IMemberConfiguration MapToMemberConfiguration(IItemMemberConfig<T> viConfig)
        {
            var memberPath = viConfig.Path;

            return memberPath switch
            {
                IMethodMemberPath<T> methodPath =>
                    MethodConfiguration.Create(
                        memberPath.UniqueMemberName,
                        methodPath.MemberName,
                        Source,
                        MapToMemberConfiguration(viConfig, methodPath.UniqueName, memberPath.ReturnType.FullName),
                        methodPath.DeclaredType,
                        methodPath.Parameters.Select(p => p.DeclaredTypeName).ToArray()),
                ICtorParamMemberPath<T> _ => new CtorMemberConfiguration(
                    ImmutableArray<IMemberConfiguration>.Empty
                        .Add(
                            MapToMemberConfiguration(viConfig, memberPath.UniqueName, memberPath.MemberName)),
                    ImmutableArray<ITypeFullName>.Empty,
                    Source),
                _ => MapToMemberConfiguration(viConfig, memberPath.UniqueName, memberPath.MemberName),
            };
        }

        private static IMemberConfiguration MapToMemberConfiguration(
            IItemMemberConfig<T> viConfig,
            string pathName,
            string memberName) =>
            viConfig switch
            {
                IItemTypeConfig<T> viTypeConfig => new LinkMemberConfiguration(
                    memberName,
                    FixtureItemId.CreateNamed(
                        pathName,
                        viTypeConfig.Type.ToTypeFullName()),
                    Source),

                IItemUniqueConfig<T> _ => new UniqueValueMemberConfiguration(memberName, Source),

                IItemValueConfig<T> { Value: null } =>
                    new NullValueMemberConfiguration(memberName, Source),

                IItemValueConfig<T> viValueConfig => new ValueMemberConfiguration(
                    memberName,
                    viValueConfig.Value,
                    Source),

                IItemDelegateConfig<T> viDelegateConfig => new DelegateValueMemberConfiguration(
                    memberName,
                    viDelegateConfig.DelegateValue,
                    Source),

                IItemUndefinedConfig<T> viUndefinedConfig => new UndefinedMemberConfiguration(
                    memberName,
                    viUndefinedConfig.Path.ReturnType.ToTypeFullName(),
                    Source),

                _ => throw new ArgumentOutOfRangeException(nameof(viConfig)),
            };

        #endregion

        #region Nested type: FixtureItemIdComparer

        private sealed class FixtureItemIdComparer : IEqualityComparer<FixtureItemId>
        {
            #region members

            public bool Equals(FixtureItemId x, FixtureItemId y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (x is null)
                {
                    return false;
                }

                if (y is null)
                {
                    return false;
                }

                if (x.GetType() != y.GetType())
                {
                    return false;
                }

                return x.Name.Equals(y.Name) && Equals(x.TypeFullName, y.TypeFullName);
            }

            public int GetHashCode(FixtureItemId obj)
            {
                unchecked
                {
                    return (obj.Name.GetHashCode() * 397) ^
                           (obj.TypeFullName != null ? obj.TypeFullName.GetHashCode() : 0);
                }
            }

            #endregion
        }

        #endregion
    }
}
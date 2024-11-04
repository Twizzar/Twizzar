using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Twizzar.CompositionRoot;
using Twizzar.Fixture.Member;
using Twizzar.Interfaces;
using Twizzar.Runtime.Core.FixtureItem.Configuration;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Services;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Fixture;

/// <summary>
/// Base class for the item builders.
/// </summary>
/// <typeparam name="TFixtureItem"></typeparam>
public abstract class ItemBuilderBase<TFixtureItem>
{
    #region properties

    /// <summary>
    /// Gets or sets the member configurations.
    /// </summary>
    internal IImmutableDictionary<IMemberPath<TFixtureItem>, IItemMemberConfig<TFixtureItem>> ViMemberConfigurations { get; set; } =
        ImmutableDictionary.Create<IMemberPath<TFixtureItem>, IItemMemberConfig<TFixtureItem>>();

    /// <summary>
    /// Gets or sets the method callbacks. The object should be an Action.
    /// </summary>
    internal IImmutableDictionary<IMemberPath<TFixtureItem>, IList<object>> ViMethodCallbacks { get; set; } =
        ImmutableDictionary.Create<IMemberPath<TFixtureItem>, IList<object>>();

    #endregion

    #region members

    /// <summary>
    /// Add or update a member config.
    /// </summary>
    /// <param name="memberConfig"></param>
    /// <returns>Itself.</returns>
    public ItemBuilderBase<TFixtureItem> AddMemberConfig(MemberConfig<TFixtureItem> memberConfig) =>
        this.AddMemberConfig(memberConfig.ItemMemberConfig);

    /// <summary>
    /// Builds a concrete instance of type TFixtureItem as configured by this builder. <br/>
    /// If TFixtureItem is a twizzar base-type a unique value of the given type will be returned. <br/>
    /// If TFixtureItem is an interface, it will be a stub of Type TFixtureItem. <br/>
    /// If TFixtureItem is a class, the type itself will be created via reflection.
    /// <remarks>
    /// <para>
    /// Unique has different meaning for different types: <br/>
    ///     * Numeric value: Uses an algorithm to create a unique value.<br/>
    ///     * String: generates a GUID.<br/>
    ///     * Chars: Uses the same algorithm as Numeric values.<br/>
    ///     * Bool: Returns True.<br/>
    /// </para>
    /// </remarks>
    /// </summary>
    /// <returns>A instance of type TFixtureItem.</returns>
    public TFixtureItem Build() => this.BuildInternal().Instance;

    /// <summary>
    /// Builds many concrete instance of type TFixtureItem. The method <see cref="Build()"/> will be called n times where <c>n = count</c>.
    /// </summary>
    /// <param name="count">The number of elements returned. Must be greater equal to 0.</param>
    /// <returns>A list of concrete instance of type TFixtureItem.</returns>
    public IReadOnlyList<TFixtureItem> BuildMany(int count) =>
        this.BuildManyInternal(count).Select(tuple => tuple.Instance).ToList();

    /// <summary>
    /// Add a new member configuration and returns the new item config.
    /// </summary>
    /// <param name="memberConfiguration">The memberConfiguration to add.</param>
    /// <returns>The updates item config.</returns>
    internal ItemBuilderBase<TFixtureItem> AddMemberConfig(IItemMemberConfig<TFixtureItem> memberConfiguration)
    {
        if (memberConfiguration is IItemCallbackConfig<TFixtureItem> callbackConfig)
        {
            this.AddMethodCallback(callbackConfig.Path, callbackConfig.Callback);
        }
        else if (this.ViMemberConfigurations.ContainsKey(memberConfiguration.Path))
        {
            this.ViMemberConfigurations = this.ViMemberConfigurations.SetItem(
                memberConfiguration.Path,
                memberConfiguration);
        }
        else
        {
            this.ViMemberConfigurations = this.ViMemberConfigurations.Add(
                memberConfiguration.Path,
                memberConfiguration);
        }

        return this;
    }

    /// <summary>
    /// Add a method callback and return a new item config.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    internal ItemBuilderBase<TFixtureItem> AddMethodCallback(IMemberPath<TFixtureItem> path, object callback)
    {
        if (this.ViMethodCallbacks.ContainsKey(path))
        {
            this.ViMethodCallbacks[path].Add(callback);
        }
        else
        {
            this.ViMethodCallbacks = this.ViMethodCallbacks.Add(path, new List<object>() { callback });
        }

        return this;
    }

    /// <summary>
    /// Get the <see cref="ItemConfig{TFixtureItem}"/>.
    /// The <see cref="ItemBuilder{TFixtureItem,TPathProvider}"/> is not immutable so this can changed.
    /// </summary>
    /// <returns>A new instance of <see cref="ItemConfig{TFixtureItem}"/>.</returns>
    internal ItemConfig<TFixtureItem> GetConfig() => new(typeof(TFixtureItem).FullName, this.ViMemberConfigurations, this.ViMethodCallbacks);

    /// <summary>
    /// Build internal implementation.
    /// </summary>
    /// <returns></returns>
    internal (TFixtureItem Instance, IInstanceCacheQuery InstanceCacheQuery) BuildInternal()
    {
        var config = this.GetConfig();
        var ioc = CreateOrchestrator(Maybe.Some<IItemConfig<TFixtureItem>>(config));

        var instance = ioc
            .Resolve<IFixtureItemContainer>()
            .GetInstance(config);

        return (instance, ioc.Resolve<IInstanceCacheQuery>());
    }

    /// <summary>
    /// Build many internal implementation.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">When count is less than 0.</exception>
    internal IEnumerable<(TFixtureItem Instance, IInstanceCacheQuery InstanceCacheQuery)> BuildManyInternal(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Should be greater or equal to zero.");
        }

        return this.BuildManyInternalIterator(count);
    }

    /// <summary>
    /// Create a new <see cref="IocOrchestrator"/>.
    /// </summary>
    /// <typeparam name="T">The fixture item type.</typeparam>
    /// <param name="itemConfig"></param>
    /// <returns>A new <see cref="IIocOrchestrator"/>.</returns>
    internal static IIocOrchestrator CreateOrchestrator<T>(Maybe<IItemConfig<T>> itemConfig)
    {
        var iocOrchestrator = new IocOrchestrator();
        iocOrchestrator.Register(itemConfig);
        return iocOrchestrator;
    }

    private IEnumerable<(TFixtureItem Instance, IInstanceCacheQuery InstanceCacheQuery)> BuildManyInternalIterator(
        int count)
    {
        for (var i = 0; i < count; i++)
        {
            yield return this.BuildInternal();
        }
    }

    #endregion
}
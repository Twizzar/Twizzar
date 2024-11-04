using System;
using System.Collections.Generic;
using System.Linq;

using Twizzar.Fixture.Member;
using Twizzar.Fixture.Utils;

namespace Twizzar.Fixture
{
    /// <summary>
    /// Builder for configuring a fixture item then building a concrete instance of it.
    /// </summary>
    /// <typeparam name="TFixtureItem">The fixture item type.</typeparam>
    /// <typeparam name="TPathProvider">The PathProvider type.</typeparam>
    public class ItemBuilder<TFixtureItem, TPathProvider> : ItemBuilderBase<TFixtureItem>, IPathSelectionProvider<TFixtureItem, TPathProvider>
        where TPathProvider : new()
    {
        /// <summary>
        /// Configure a member. To configure the member use a lambda.
        /// <example>
        /// <code>
        /// new ItemBuilder&lt;Car&gt;()
        ///     .With(p =&gt; p.Engine.CylinderCount.Value(4))
        ///     .Build();
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="func">A function which returns an <see cref="MemberConfig{TFixtureItem}"/>.
        /// The function parameter provides access to <see cref="MemberPath{TFixtureItem}"/> every member path provides
        /// methods for configure the member for example: <c>.Value()</c>.
        /// </param>
        /// <returns>Itself for further configuration or for building the instance.</returns>
        public ItemBuilder<TFixtureItem, TPathProvider> With(Func<TPathProvider, MemberConfig<TFixtureItem>> func) =>
            (ItemBuilder<TFixtureItem, TPathProvider>)ItemBuilderHelperMethods.With(this, func);

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
        /// <param name="itemContext">The context which can be used for verifications and to get dependencies which are setup before.</param>
        /// <returns>A instance of type TFixtureItem.</returns>
        public TFixtureItem Build(out IItemContext<TFixtureItem, TPathProvider> itemContext)
        {
            var (instance, instanceCacheQuery) = this.BuildInternal();
            itemContext = new ItemContext<TFixtureItem, TPathProvider>(instanceCacheQuery);
            return instance;
        }

        /// <summary>
        /// Calls Build n times where n = count. See <see cref="Build"/> for more information.
        /// </summary>
        /// <param name="count">The number of elements returned. Must be greater equal to 0.</param>
        /// <param name="itemContexts">A context for each instance.</param>
        /// <returns>A list of TFixtureItems.</returns>
        public IReadOnlyList<TFixtureItem> BuildMany(int count, out IReadOnlyList<IItemContext<TFixtureItem, TPathProvider>> itemContexts)
        {
            var items = this.BuildManyInternal(count).ToList();

            itemContexts = items
                .Select(tuple => tuple.InstanceCacheQuery)
                .Select(query => new ItemContext<TFixtureItem, TPathProvider>(query))
                .ToList();

            return items.Select(tuple => tuple.Instance).ToList();
        }
    }
}
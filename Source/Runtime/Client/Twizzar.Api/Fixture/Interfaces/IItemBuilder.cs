using System.Collections.Generic;

namespace Twizzar.Fixture
{
    /// <summary>
    /// Builder for configuring a fixture item then building a concrete instance of it.
    /// </summary>
    /// <typeparam name="TFixtureItem">The fixture item type.</typeparam>
    public interface IItemBuilder<TFixtureItem>
    {
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
        TFixtureItem Build();

        /// <summary>
        /// Builds many concrete instance of type TFixtureItem. The method <see cref="Build()"/> will be called n times where <c>n = count</c>.
        /// </summary>
        /// <param name="count">The number of elements returned. Must be greater equal to 0.</param>
        /// <returns>A list of concrete instance of type TFixtureItem.</returns>
        IReadOnlyList<TFixtureItem> BuildMany(int count);
    }
}
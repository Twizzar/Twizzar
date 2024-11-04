namespace Twizzar.Fixture.Member
{
    /// <summary>
    /// Path of an instance member like Property, Field or an Method.
    /// </summary>
    /// <typeparam name="TFixtureItem">The root fixture item type.</typeparam>
    public interface IInstancePath<TFixtureItem>
    {
        /// <summary>
        /// Configure the member as undefined.
        /// </summary>
        /// <returns>A <see cref="MemberConfig{TFixtureItem}"/>.</returns>
        public MemberConfig<TFixtureItem> Undefined();
    }
}
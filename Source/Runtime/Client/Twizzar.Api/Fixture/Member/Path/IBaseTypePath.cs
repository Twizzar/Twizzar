namespace Twizzar.Fixture.Member
{
    /// <summary>
    /// A member where the return type is a twizzar base-type.
    /// </summary>
    /// <typeparam name="TFixtureItem">The root fixture item type.</typeparam>
    public interface IBaseTypePath<TFixtureItem>
    {
        /// <summary>
        /// Configure as undefined.
        /// </summary>
        /// <returns>The <see cref="MemberConfig{TFixtureItem}"/> for further configuration.</returns>
        public MemberConfig<TFixtureItem> Unique();
    }
}
using Twizzar.Fixture.Utils;

namespace Twizzar.Fixture.Member
{
    /// <summary>
    /// Member path for a property where the return type is a twizzar base-tye.
    /// </summary>
    /// <typeparam name="TFixtureItem">The root fixture item type.</typeparam>
    /// <typeparam name="TReturnType">The return type of the property.</typeparam>
    public class PropertyBasetypeMemberPath<TFixtureItem, TReturnType> :
        PropertyMemberPath<TFixtureItem, TReturnType>,
        IBaseTypePath<TFixtureItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBasetypeMemberPath{TFixtureItem, TReturnType}"/> class.
        /// </summary>
        /// <param name="viMemberName"></param>
        /// <param name="parent"></param>
        public PropertyBasetypeMemberPath(string viMemberName, MemberPath<TFixtureItem> parent)
            : base(viMemberName, parent)
        {
        }

        /// <inheritdoc />
        public MemberConfig<TFixtureItem> Unique() =>
            PathExtensionHelperMethods.Unique(this);
    }
}
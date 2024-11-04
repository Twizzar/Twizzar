using Twizzar.Fixture.Utils;

namespace Twizzar.Fixture.Member
{
    /// <summary>
    /// Member path for a constructor parameter where the type is a twizzar base-tye.
    /// </summary>
    /// <typeparam name="TFixtureItem">The root fixture item type.</typeparam>
    /// <typeparam name="TParameterType">The parameter type.</typeparam>
    public class CtorParamBasetypeMemberPath<TFixtureItem, TParameterType> :
        CtorParamMemberPath<TFixtureItem, TParameterType>,
        IBaseTypePath<TFixtureItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CtorParamBasetypeMemberPath{TFixtureItem, TParameterType}"/> class.
        /// </summary>
        /// <param name="viMemberName"></param>
        /// <param name="parent"></param>
        public CtorParamBasetypeMemberPath(string viMemberName, MemberPath<TFixtureItem> parent)
            : base(viMemberName, parent)
        {
        }

        /// <inheritdoc />
        public MemberConfig<TFixtureItem> Unique() =>
            PathExtensionHelperMethods.Unique(this);
    }
}
using Twizzar.Fixture.Utils;

namespace Twizzar.Fixture.Member
{
    /// <summary>
    /// Member path for a method where the return type is a twizzar base-tye..
    /// </summary>
    /// <typeparam name="TFixtureItem">The root fixture item type.</typeparam>
    /// <typeparam name="TReturnType">The return type configured.</typeparam>
    /// <typeparam name="TDeclaredType">The actual return type of the method.</typeparam>
    public class MethodBasetypeMemberPath<TFixtureItem, TReturnType, TDeclaredType> :
        MethodMemberPath<TFixtureItem, TReturnType, TDeclaredType>,
        IBaseTypePath<TFixtureItem>
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodBasetypeMemberPath{TFixtureItem, TReturnType, TDeclaredType}"/> class.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="parent"></param>
        /// <param name="parameters"></param>
        public MethodBasetypeMemberPath(string methodName, MemberPath<TFixtureItem> parent, params TzParameter[] parameters)
            : base(methodName, parent, parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodBasetypeMemberPath{TFixtureItem, TReturnType, TDeclaredType}"/> class.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="parent"></param>
        /// <param name="genericTypeArguments"></param>
        /// <param name="parameters"></param>
        public MethodBasetypeMemberPath(
            string methodName,
            MemberPath<TFixtureItem> parent,
            string[] genericTypeArguments,
            params TzParameter[] parameters)
            : base(methodName, parent, genericTypeArguments, parameters)
        {
        }

        #endregion

        #region members

        /// <inheritdoc />
        public MemberConfig<TFixtureItem> Unique() => PathExtensionHelperMethods.Unique(this);

        #endregion
    }
}
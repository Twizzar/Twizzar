using System;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;

namespace Twizzar.Fixture.Member
{
    /// <summary>
    /// Member path for a constructor parameter.
    /// </summary>
    /// <typeparam name="TFixtureItem">The root fixture item type.</typeparam>
    /// <typeparam name="TParameterType">The parameter type.</typeparam>
    public class CtorParamMemberPath<TFixtureItem, TParameterType> :
        MemberPath<TFixtureItem, TParameterType>,
        ICtorParamMemberPath<TFixtureItem>
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="CtorParamMemberPath{TFixtureItem, TParameterType}"/> class.
        /// </summary>
        /// <param name="viMemberName"></param>
        /// <param name="parent"></param>
        public CtorParamMemberPath(string viMemberName, MemberPath<TFixtureItem> parent)
            : base(viMemberName, parent)
        {
        }

        #endregion

        #region properties

        /// <inheritdoc />
        protected override Type ViDeclaredType => typeof(TParameterType);

        #endregion
    }
}
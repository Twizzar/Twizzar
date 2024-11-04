using System;
using Twizzar.Fixture.Utils;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;

namespace Twizzar.Fixture.Member
{
    /// <summary>
    /// Member path for a field.
    /// </summary>
    /// <typeparam name="TFixtureItem">The root fixture item type.</typeparam>
    /// <typeparam name="TReturnType">The return type of the field.</typeparam>
    public class FieldMemberPath<TFixtureItem, TReturnType> :
        MemberPath<TFixtureItem, TReturnType>,
        IFieldMemberPath<TFixtureItem>,
        IInstancePath<TFixtureItem>
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldMemberPath{TFixtureItem, TReturnType}"/> class.
        /// </summary>
        /// <param name="viMemberName"></param>
        /// <param name="parent"></param>
        public FieldMemberPath(string viMemberName, MemberPath<TFixtureItem> parent)
            : base(viMemberName, parent)
        {
        }

        #endregion

        #region properties

        /// <inheritdoc />
        protected override Type ViDeclaredType => typeof(TReturnType);

        #endregion

        #region members

        /// <inheritdoc />
        public MemberConfig<TFixtureItem> Undefined() => PathExtensionHelperMethods.Undefined(this);

        #endregion
    }
}
using System;

namespace Twizzar.Fixture.Member
{
    /// <summary>
    /// The root path describes the entry point to a fixture item path.
    /// </summary>
    /// <typeparam name="TFixtureItem">The type of the fixture item.</typeparam>
    internal class RootPath<TFixtureItem> : MemberPath<TFixtureItem, TFixtureItem>
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RootPath{TFixtureItem}"/> class.
        /// </summary>
        /// <param name="viMemberName"></param>
        public RootPath(string viMemberName)
            : base(viMemberName)
        {
        }

        #endregion

        #region properties

        /// <inheritdoc />
        protected override Type ViDeclaredType => typeof(TFixtureItem);

        #endregion
    }
}
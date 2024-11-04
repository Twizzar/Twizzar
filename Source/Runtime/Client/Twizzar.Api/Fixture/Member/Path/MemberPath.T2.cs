using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Fixture.Member
{
    /// <inheritdoc cref="IMemberPath{TFixtureItem}" />
    public abstract class MemberPath<TFixtureItem, TReturnType> : MemberPath<TFixtureItem>, IMemberPath<TFixtureItem, TReturnType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberPath{TFixtureItem, TReturnType}"/> class.
        /// </summary>
        /// <param name="viMemberName">The member name.</param>
        protected MemberPath(string viMemberName)
            : base(viMemberName, typeof(TReturnType), Maybe.None())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberPath{TFixtureItem, TReturnType}"/> class.
        /// </summary>
        /// <param name="viMemberName">The member name.</param>
        /// <param name="parent">The parent path.</param>
        protected MemberPath(string viMemberName, MemberPath<TFixtureItem> parent)
            : base(viMemberName, typeof(TReturnType), parent)
        {
        }

        /// <summary>
        /// Set a value for this member.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The <see cref="MemberConfig{TFixtureItem}"/>.</returns>
        public MemberConfig<TFixtureItem> Value(TReturnType value) =>
            ItemMemberConfigFactory.Value(this, value);

        /// <summary>
        /// Configure the member to be a instance of a class or struct.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance.</typeparam>
        /// <returns>The <see cref="MemberConfig{TFixtureItem}"/>.</returns>
        public MemberConfig<TFixtureItem> InstanceOf<TInstance>()
            where TInstance : TReturnType =>
            ItemMemberConfigFactory.ViType<TFixtureItem, TInstance>(this);

        /// <summary>
        /// Configure the member to be a stub.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance. This needs to be an interface type.</typeparam>
        /// <returns>The <see cref="MemberConfig{TFixtureItem}"/>.</returns>
        public MemberConfig<TFixtureItem> Stub<TInstance>()
            where TInstance : TReturnType =>
            ItemMemberConfigFactory.ViType<TFixtureItem, TInstance>(this);
    }
}
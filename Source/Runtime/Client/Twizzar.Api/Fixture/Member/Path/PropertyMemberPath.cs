using System;
using Twizzar.Fixture.Utils;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;

namespace Twizzar.Fixture.Member
{
    /// <summary>
    /// Member path for a property.
    /// </summary>
    /// <typeparam name="TFixtureItem">The root fixture item type.</typeparam>
    /// <typeparam name="TReturnType">The return type of the property.</typeparam>
    public class PropertyMemberPath<TFixtureItem, TReturnType> :
        MemberPath<TFixtureItem, TReturnType>,
        IPropertyMemberPath<TFixtureItem>,
        IInstancePath<TFixtureItem>
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMemberPath{TFixtureItem, TReturnType}"/> class.
        /// </summary>
        /// <param name="viMemberName"></param>
        /// <param name="parent"></param>
        public PropertyMemberPath(string viMemberName, MemberPath<TFixtureItem> parent)
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

        /// <summary>
        /// Setup a delegate to calculate the return value.
        /// </summary>
        /// <param name="delegateValue">The delegate should be a <see cref="Func{TResult}"/>.</param>
        /// <returns></returns>
        protected MemberConfig<TFixtureItem> Delegate(object delegateValue) =>
            delegateValue switch
            {
                { } => PathExtensionHelperMethods.Delegate(this, delegateValue),
                null when typeof(TReturnType).CanBeNull() => this.Value(default),
                _ => throw new ArgumentNullException(nameof(delegateValue)),
            };

        #endregion
    }
}
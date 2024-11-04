using System;
using System.Linq;
using Twizzar.Fixture.Utils;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;

namespace Twizzar.Fixture.Member
{
    /// <summary>
    /// Member path for a method.
    /// </summary>
    /// <typeparam name="TFixtureItem">The root fixture item type.</typeparam>
    /// <typeparam name="TReturnType">The return type configured.</typeparam>
    /// <typeparam name="TDeclaredType">The actual return type of the method.</typeparam>
    public class MethodMemberPath<TFixtureItem, TReturnType, TDeclaredType> :
        MemberPath<TFixtureItem, TReturnType>,
        IMethodMemberPath<TFixtureItem>,
        IInstancePath<TFixtureItem>
    {
        #region fields

        private readonly TzParameter[] _tzParameters;
        private readonly GenericTypeArgument[] _tzGenericTypeArguments;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodMemberPath{TFixtureItem, TReturnType, TDeclaredType}"/> class.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="parent"></param>
        /// <param name="parameters"></param>
        public MethodMemberPath(string methodName, MemberPath<TFixtureItem> parent, params TzParameter[] parameters)
            : base(methodName, parent)
        {
            this._tzParameters = parameters;
            this._tzGenericTypeArguments = Array.Empty<GenericTypeArgument>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodMemberPath{TFixtureItem, TReturnType, TDeclaredType}"/> class.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="parent"></param>
        /// <param name="genericTypeArguments"></param>
        /// <param name="parameters"></param>
        public MethodMemberPath(string methodName, MemberPath<TFixtureItem> parent, string[] genericTypeArguments, params TzParameter[] parameters)
            : base(methodName, parent)
        {
            this._tzParameters = parameters;

            genericTypeArguments ??= Array.Empty<string>();
            this._tzGenericTypeArguments = genericTypeArguments.Select(s => new GenericTypeArgument(s)).ToArray();
        }

        #endregion

        #region properties

        /// <inheritdoc />
        ITzParameter[] IMethodMemberPath<TFixtureItem>.Parameters => this._tzParameters;

        /// <inheritdoc />
        GenericTypeArgument[] IMethodMemberPath<TFixtureItem>.GenericArguments => this._tzGenericTypeArguments;

        /// <inheritdoc />
        protected override Type ViDeclaredType => typeof(TDeclaredType);

        #endregion

        #region members

        /// <inheritdoc />
        public MemberConfig<TFixtureItem> Undefined() => PathExtensionHelperMethods.Undefined(this);

        /// <summary>
        /// Setup a delegate to calculate the return value.
        /// </summary>
        /// <param name="delegateValue">The delegate should be a <see cref="Func{TResult}"/> with the correct parameter types.</param>
        /// <returns></returns>
        protected MemberConfig<TFixtureItem> Delegate(object delegateValue) =>
            delegateValue switch
            {
                { } => PathExtensionHelperMethods.Delegate(this, delegateValue),
                null when typeof(TReturnType).CanBeNull() => this.Value(default),
                _ => throw new ArgumentNullException(nameof(delegateValue)),
            };

        /// <summary>
        /// Register a callback to call when the method is executed.
        /// </summary>
        /// <param name="callbackDelegate">The delegate should be a <see cref="Action"/> ith the correct parameter types.</param>
        /// <returns></returns>
        protected MemberConfig<TFixtureItem> RegisterCallback(object callbackDelegate)
        {
            if (callbackDelegate is null)
            {
                throw new ArgumentNullException(nameof(callbackDelegate));
            }

            return PathExtensionHelperMethods.Callback(this, callbackDelegate);
        }

        #endregion
    }
}
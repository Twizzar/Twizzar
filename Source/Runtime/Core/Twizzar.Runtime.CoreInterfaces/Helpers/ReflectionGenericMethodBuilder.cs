using System;
using System.Reflection;

namespace Twizzar.Runtime.CoreInterfaces.Helpers
{
    /// <summary>
    /// Helper class for invoking generic methods with reflection.
    /// </summary>
    internal class ReflectionGenericMethodBuilder
    {
        #region fields

        private readonly MethodInfo _methodInfo;
        private object _obj = null;
        private Type[] _genericTypes = Type.EmptyTypes;
        private object[] _parameters = Array.Empty<object>();

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionGenericMethodBuilder"/> class.
        /// </summary>
        /// <param name="methodInfo"></param>
        public ReflectionGenericMethodBuilder(MethodInfo methodInfo)
        {
            this._methodInfo = methodInfo;
        }

        #endregion

        #region members

        /// <summary>
        /// Create form an action with two generic parameters.
        /// </summary>
        /// <param name="d">The delegate.</param>
        /// <returns></returns>
        public static ReflectionGenericMethodBuilder Create(Delegate d) =>
            new(d.Method);

        /// <summary>
        /// Specify the generic types.
        /// </summary>
        /// <param name="genericTypes"></param>
        /// <returns></returns>
        public ReflectionGenericMethodBuilder WithGenericTypes(params Type[] genericTypes)
        {
            this._genericTypes = genericTypes;
            return this;
        }

        /// <summary>
        /// Specify the input parameters.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ReflectionGenericMethodBuilder WithParameters(params object[] parameters)
        {
            this._parameters = parameters;
            return this;
        }

        /// <summary>
        /// Specify the object to call this method on. Null or do not call this method if the method is static.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ReflectionGenericMethodBuilder WithInvocationObject(object obj)
        {
            this._obj = obj;
            return this;
        }

        /// <summary>
        /// Invoke the method.
        /// </summary>
        /// <returns></returns>
        public object Invoke() =>
            this._methodInfo.GetGenericMethodDefinition()
                .MakeGenericMethod(this._genericTypes)
                .Invoke(this._obj, this._parameters);

        /// <summary>
        /// Invoke the method and cast the return type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Invoke<T>() => (T)this.Invoke();

        #endregion
    }
}
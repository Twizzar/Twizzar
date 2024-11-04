using System;
using System.Diagnostics.CodeAnalysis;

namespace Twizzar.SharedKernel.CoreInterfaces.Util
{
    /// <summary>
    /// Class which wraps a lambda to a method to provide a simple way to unsubscribe the event.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of the event arg.</typeparam>
    /// <typeparam name="TSender">The type of the sender.</typeparam>
    [ExcludeFromCodeCoverage]
    public class LambdaEvent<TEventArgs, TSender>
        where TEventArgs : EventArgs
        where TSender : class
    {
        #region fields

        private readonly Lambda _callback;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="LambdaEvent{TEventArgs, TSender}"/> class.
        /// </summary>
        /// <param name="callback"></param>
        public LambdaEvent(Lambda callback)
        {
            this._callback = callback;
        }

        #endregion

        #region delegates

        /// <summary>
        /// Lambda delegate.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The event args.</param>
        /// <param name="lambdaEvent">The lambda event.</param>
        public delegate void Lambda(TSender sender, TEventArgs args, LambdaEvent<TEventArgs, TSender> lambdaEvent);

        #endregion

        #region members

        /// <summary>
        /// Invoke the lambda.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Invoke(object sender, TEventArgs e)
        {
            this._callback?.Invoke(sender as TSender, e, this);
        }

        #endregion
    }
}
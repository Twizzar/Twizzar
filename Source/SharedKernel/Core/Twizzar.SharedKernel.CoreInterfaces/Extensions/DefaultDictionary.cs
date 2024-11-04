using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.Extensions
{
    /// <summary>
    /// A dictionary which returns always a value for a key. When the key is not set returns the default value.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <seealso cref="System.Collections.Generic.Dictionary{TKey, TValue}" />
    [Serializable]
    public class DefaultDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        #region fields

        private readonly Func<TKey, TValue> _defaultValueFactory;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        public DefaultDictionary(TValue defaultValue = default)
            : this(key => defaultValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="defaultValueFactory">Function for getting the default value.</param>
        public DefaultDictionary(Func<TKey, TValue> defaultValueFactory)
        {
            this._defaultValueFactory = defaultValueFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected DefaultDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this._defaultValueFactory = (Func<TKey, TValue>)info.GetValue(nameof(this._defaultValueFactory), typeof(Func<TKey, TValue>));
        }

        #endregion

        #region properties

        /// <summary>
        /// Get the value of the key or the default.
        /// </summary>
        /// <param name="key">
        /// Gets the value of the key when dictionary contains the key; otherwise returns the output of <see cref="_defaultValueFactory"/>.
        /// </param>
        public new TValue this[TKey key]
        {
            get =>
                this.GetMaybe(key)
                    .Match(
                        value => value,
                        () => this.GetAndAddDefaultValue(key));
            set => base[key] = value;
        }

        #endregion

        #region members

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(this._defaultValueFactory), this._defaultValueFactory);
        }

        private TValue GetAndAddDefaultValue(TKey key)
        {
            var value = this._defaultValueFactory(key);
            this.Add(key, value);
            return value;
        }

        #endregion
    }
}
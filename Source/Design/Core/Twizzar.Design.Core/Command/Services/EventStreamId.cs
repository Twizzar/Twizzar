using System;
using System.Collections.Generic;
using Twizzar.SharedKernel.CoreInterfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.Design.Core.Command.Services
{
    /// <summary>
    /// Id for a event in the event stream. Only public used by the <see cref="EventStream"/>.
    /// </summary>
    public class EventStreamId : ValueObject
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamId"/> class.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="order"></param>
        public EventStreamId(DateTime dateTime, uint order)
        {
            this.DateTime = dateTime;
            this.Order = order;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the order comparer.
        /// </summary>
        public static IComparer<EventStreamId> OrderComparer { get; } = new OrderRelationalComparer();

        /// <summary>
        /// Gets the date time.
        /// </summary>
        public DateTime DateTime { get; }

        /// <summary>
        /// Gets the order.
        /// </summary>
        public uint Order { get; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.DateTime;
            yield return this.Order;
        }

        #endregion

        #region Nested type: OrderRelationalComparer

        private sealed class OrderRelationalComparer : IComparer<EventStreamId>
        {
            #region members

            public int Compare(EventStreamId x, EventStreamId y)
            {
                EnsureHelper.GetDefault.Many()
                    .Parameter(x, nameof(x))
                    .Parameter(y, nameof(y))
                    .ThrowWhenNull();

                return x!.Order.CompareTo(y!.Order);
            }

            #endregion
        }

        #endregion
    }
}
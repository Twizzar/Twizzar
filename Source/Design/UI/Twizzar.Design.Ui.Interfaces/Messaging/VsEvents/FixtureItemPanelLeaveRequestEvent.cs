using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.SharedKernel.CoreInterfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.Design.Ui.Interfaces.Messaging.VsEvents
{
    /// <summary>
    /// Event triggered when a Request for leaving the FixtureItemPanel was send.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FixtureItemPanelLeaveRequestEvent : ValueObject, IUiEvent
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemPanelLeaveRequestEvent"/> class.
        /// </summary>
        /// <param name="leaveDirection"></param>
        public FixtureItemPanelLeaveRequestEvent(LeaveDirection leaveDirection)
        {
            EnsureHelper.GetDefault.Parameter(this, nameof(leaveDirection)).ThrowWhenNull();
            this.Direction = leaveDirection;
        }

        #endregion

        #region LeaveDirection enum

        /// <summary>
        /// Describes the leave direction.
        /// </summary>
        public enum LeaveDirection
        {
            /// <summary>
            /// Leave the panel upwards.
            /// </summary>
            Up,

            /// <summary>
            /// Leave the panel downwards.
            /// </summary>
            Down,
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the direction to leave.
        /// </summary>
        public LeaveDirection Direction { get; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Direction;
        }

        #endregion
    }
}
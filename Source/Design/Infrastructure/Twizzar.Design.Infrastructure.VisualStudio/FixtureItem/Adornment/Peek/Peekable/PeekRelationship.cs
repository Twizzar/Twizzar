using Microsoft.VisualStudio.Language.Intellisense;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.SharedKernel.CoreInterfaces;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment.Peek.Peekable
{
    /// <inheritdoc cref="IPeekRelationship" />
    public sealed class PeekRelationship : Entity<PeekRelationship, AdornmentId>, IPeekRelationship
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="PeekRelationship"/> class.
        /// </summary>
        /// <param name="entityId">The Id of this entity. An <see cref="AdornmentId"/> is Used here.</param>
        public PeekRelationship(AdornmentId entityId)
            : base(entityId)
        {
            this.EnsureParameter(entityId, nameof(entityId)).ThrowWhenNull();
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public string Name => "TWIZZAR";

        /// <inheritdoc />
        public string DisplayName => "TWIZZAR";

        #endregion

        #region members

        /// <inheritdoc />
        protected override bool Equals(AdornmentId a, AdornmentId b) =>
            a == b;

        #endregion
    }
}
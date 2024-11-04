using System;

using Twizzar.SharedKernel.NLog.Interfaces;

using ViCommon.EnsureHelper;

#pragma warning disable S4035 // Classes implementing "IEquatable<T>" should be sealed

namespace Twizzar.SharedKernel.CoreInterfaces
{
    /// <summary>
    /// Entity object which is identified by his id.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityId">The type of the entity id.</typeparam>
    public abstract class Entity<TEntity, TEntityId> : IEntity, IEquatable<TEntity>
        where TEntity : Entity<TEntity, TEntityId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Entity{TEntity, TEntityId}"/> class.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        protected Entity(TEntityId entityId)
        {
            this.EntityId = entityId;
        }

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        /// <summary>
        /// Gets the entity id.
        /// </summary>
        protected TEntityId EntityId { get; }

        /// <summary>
        /// Check if the two Entities are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Entity<TEntity, TEntityId> left, Entity<TEntity, TEntityId> right) =>
            Equals(left, right);

        /// <summary>
        /// Check if the two Entities are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Entity<TEntity, TEntityId> left, Entity<TEntity, TEntityId> right) =>
            !Equals(left, right);

        /// <summary>
        /// Check if the two Entities are equal.
        /// </summary>
        /// <param name="left">The first entity.</param>
        /// <param name="right">The second entity.</param>
        /// <returns>True when they are equal.</returns>
        public static bool Equals(Entity<TEntity, TEntityId> left, Entity<TEntity, TEntityId> right) =>
            left?.Equals(right) ?? right is null;

        /// <inheritdoc />
        public bool Equals(TEntity other) =>
            other != null &&
            this.Equals(this.EntityId, other.EntityId);

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return this.Equals((Entity<TEntity, TEntityId>)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() =>
            this.EntityId.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            $"{this.GetType().Name}({this.EntityId})";

        /// <summary>
        /// Check if two entity ids are equal.
        /// </summary>
        /// <param name="a">Fist id.</param>
        /// <param name="b">Second id.</param>
        /// <returns>True when equals else false.</returns>
        protected abstract bool Equals(TEntityId a, TEntityId b);

        /// <summary>
        /// Checks if this entity is equal with another entity.
        /// They get compared over there <see cref="EntityId"/>.
        /// </summary>
        /// <param name="other">The other entity.</param>
        /// <returns>True when equals else false.</returns>
        protected bool Equals(Entity<TEntity, TEntityId> other) =>
            other != null &&
            this.Equals(this.EntityId, other.EntityId);
    }
}

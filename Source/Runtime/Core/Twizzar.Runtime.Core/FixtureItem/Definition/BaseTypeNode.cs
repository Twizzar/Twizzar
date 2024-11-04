using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Runtime.Core.FixtureItem.Definition
{
    /// <summary>
    /// Definition node of a base type fixture item.
    /// </summary>
    public sealed class BaseTypeNode : Entity<BaseTypeNode, FixtureItemId>, IBaseTypeNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTypeNode"/> class.
        /// </summary>
        /// <param name="typeDescription">The type description.</param>
        /// <param name="fixtureId">The fixture item it.</param>
        /// <param name="valueDefinition">The value definition.</param>
        /// <param name="isNullable">Is the base type nullable.</param>
        public BaseTypeNode(
            IRuntimeTypeDescription typeDescription,
            FixtureItemId fixtureId,
            IValueDefinition valueDefinition,
            bool isNullable)
            : base(fixtureId)
        {
            this.IsNullable = isNullable;
            this.ValueDefinition = valueDefinition;
            this.TypeDescription = typeDescription;
            this.FixtureItemId = fixtureId;
        }

        #region Properties

        /// <summary>
        /// Gets the value definition which describes how the value is constructed.
        /// </summary>
        public IValueDefinition ValueDefinition { get; }

        /// <inheritdoc />
        public bool IsNullable { get; }

        /// <inheritdoc />
        public IRuntimeTypeDescription TypeDescription { get; }

        /// <inheritdoc />
        public FixtureItemId FixtureItemId { get; }

        #endregion

        #region public methods

        /// <inheritdoc />
        public CreatorType GetCreatorType() => CreatorType.BaseType;

        #endregion

        #region Overrides of Entity<BaseTypeNode,FixtureItemId>

        /// <inheritdoc />
        protected override bool Equals(FixtureItemId a, FixtureItemId b)
        {
            this.EnsureMany<FixtureItemId>()
                .Parameter(a, nameof(a))
                .Parameter(b, nameof(b))
                .ThrowWhenNull();

            return a.Equals(b);
        }

        #endregion

    }
}

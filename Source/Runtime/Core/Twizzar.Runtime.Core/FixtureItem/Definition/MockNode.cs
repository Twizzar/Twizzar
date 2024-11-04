using System;
using System.Collections.Immutable;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.MemberDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Runtime.Core.FixtureItem.Definition
{
    /// <summary>
    /// Interface definition node contains all information to create an instance of a interface.
    /// </summary>
    public class MockNode : Entity<MockNode, FixtureItemId>, IMockNode
    {
        private readonly CreatorType _creatorType;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockNode"/> class.
        /// </summary>
        /// <param name="typeDescription">The type description.</param>
        /// <param name="fixtureItemId">The fixture item id.</param>
        /// <param name="type">The type of the interface.</param>
        /// <param name="properties">The property configurations.</param>
        /// <param name="methods">The method configurations.</param>
        /// <param name="creatorType">The creator type (mock or concrete class).</param>
        public MockNode(
            IRuntimeTypeDescription typeDescription,
            FixtureItemId fixtureItemId,
            Type type,
            ImmutableArray<IPropertyDefinition> properties,
            ImmutableArray<IMethodDefinition> methods,
            CreatorType creatorType)
            : base(fixtureItemId)
        {
            this.TypeDescription = typeDescription;
            this.FixtureItemId = fixtureItemId;
            this.Type = type;
            this.Properties = properties;
            this.Methods = methods;
            this._creatorType = creatorType;
        }

        #region Properties

        /// <summary>
        /// Gets the type of the interface.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets all properties definitions of the interface.
        /// </summary>
        public ImmutableArray<IPropertyDefinition> Properties { get; }

        /// <inheritdoc />
        public ImmutableArray<IMethodDefinition> Methods { get; }

        /// <inheritdoc />
        public IRuntimeTypeDescription TypeDescription { get; }

        /// <inheritdoc />
        public FixtureItemId FixtureItemId { get; }

        #endregion

        #region public methods

        /// <inheritdoc />
        public CreatorType GetCreatorType() => this._creatorType;

        #region Equals methods

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

        #endregion
    }
}

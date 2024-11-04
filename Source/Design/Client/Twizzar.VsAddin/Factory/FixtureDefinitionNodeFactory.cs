using System.Diagnostics.CodeAnalysis;

using Autofac;
using Twizzar.Design.CoreInterfaces.Command.FixtureItem.Definition;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.Factories;

namespace Twizzar.VsAddin.Factory
{
    /// <inheritdoc cref="IFixtureDefinitionNodeFactory" />
    [ExcludeFromCodeCoverage]
    public class FixtureDefinitionNodeFactory : FactoryBase, IFixtureDefinitionNodeFactory
    {
        private readonly FactoryDelegate _factoryDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureDefinitionNodeFactory"/> class.
        /// </summary>
        /// <param name="componentContext">The Autofac componentContext.</param>
        /// <param name="factoryDelegate">Factory for autofac.</param>
        public FixtureDefinitionNodeFactory(
            IComponentContext componentContext,
            FactoryDelegate factoryDelegate)
            : base(componentContext)
        {
            this._factoryDelegate = factoryDelegate;
        }

#pragma warning disable CS1591, CA1724 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
        public delegate IFixtureItemDefinitionNode FactoryDelegate(FixtureItemId id, ITypeDescription typeDescription);
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591, CA1724 // Missing XML comment for publicly visible type or member

        /// <inheritdoc />
        public IFixtureItemDefinitionNode Create(FixtureItemId id, ITypeDescription typeDescription) =>
            this._factoryDelegate(id, typeDescription);
    }
}

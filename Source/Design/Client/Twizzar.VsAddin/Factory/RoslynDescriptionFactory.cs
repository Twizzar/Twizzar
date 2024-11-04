using System.Diagnostics.CodeAnalysis;

using Autofac;

using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.Factories;

namespace Twizzar.VsAddin.Factory
{
    /// <inheritdoc cref="IRoslynDescriptionFactory"/>
    [ExcludeFromCodeCoverage]
    public class RoslynDescriptionFactory : FactoryBase, IRoslynDescriptionFactory
    {
        #region fields

        private readonly Factory _factory;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynDescriptionFactory"/> class.
        /// </summary>
        /// <param name="componentContext"></param>
        /// <param name="factory"></param>
        public RoslynDescriptionFactory(IComponentContext componentContext, Factory factory)
            : base(componentContext)
        {
            this._factory = factory;
        }

        #endregion

        #region delegates

        /// <summary>
        /// Delegate for autofac.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <returns>A new instance of <see cref="ITypeDescription"/>.</returns>
        public delegate ITypeDescription Factory(ITypeSymbol typeSymbol);

        #endregion

        #region members

        /// <inheritdoc />
        public ITypeDescription CreateDescription(ITypeSymbol symbol) => this._factory(symbol);

        #endregion
    }
}
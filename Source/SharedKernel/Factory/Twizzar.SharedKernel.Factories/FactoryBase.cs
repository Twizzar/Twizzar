using System.Diagnostics.CodeAnalysis;
using Autofac;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;

namespace Twizzar.SharedKernel.Factories
{
    /// <summary>
    /// Base class for Autofac factories.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class FactoryBase : IFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryBase"/> class.
        /// </summary>
        /// <param name="componentContext">The component context.</param>
        protected FactoryBase(IComponentContext componentContext)
        {
            componentContext?.InjectProperties(this);
        }

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion
    }
}

using System.Diagnostics.CodeAnalysis;

using Autofac;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.SharedKernel.Factories;
using VSLangProj;

namespace Twizzar.VsAddin.Factory
{
    /// <summary>
    /// Factory for creating <see cref="IVsProjectEventsPublisher"/>.
    /// </summary>
    /// <seealso cref="FactoryBase" />
    [ExcludeFromCodeCoverage]
    public class VsProjectEventPublisherFactory : FactoryBase, IVsProjectEventPublisherFactory
    {
        #region fields

        private readonly Factory _factory;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="VsProjectEventPublisherFactory"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="componentContext">The component context.</param>
        public VsProjectEventPublisherFactory(Factory factory, IComponentContext componentContext)
            : base(componentContext)
        {
            this._factory = factory;
        }

        #endregion

        #region delegates

        /// <summary>
        /// Delegate Factory for autofac.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>The resolved instance of <see cref="IVsProjectEventsPublisher"/>.</returns>
        public delegate IVsProjectEventsPublisher Factory(VSProject project);

        #endregion

        #region members

        /// <summary>
        /// Creates the specified project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>The new instance.</returns>
        public IVsProjectEventsPublisher Create(VSProject project) => this._factory(project);

        #endregion
    }
}
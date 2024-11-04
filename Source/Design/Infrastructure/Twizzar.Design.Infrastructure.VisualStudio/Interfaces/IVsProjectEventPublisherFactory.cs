using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using VSLangProj;

namespace Twizzar.Design.Infrastructure.VisualStudio.Interfaces
{
    /// <summary>
    /// Factory for creating <see cref="IVsProjectEventsPublisher"/>.
    /// </summary>
    public interface IVsProjectEventPublisherFactory
    {
        /// <summary>
        /// Creates the specified project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>The new instance.</returns>
        IVsProjectEventsPublisher Create(VSProject project);
    }
}
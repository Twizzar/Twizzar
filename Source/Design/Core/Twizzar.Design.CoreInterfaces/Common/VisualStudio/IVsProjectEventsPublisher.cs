using System;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.CoreInterfaces.Common.VisualStudio
{
    /// <summary>
    /// Service for publishing events to IEventHun when there are changes to a Project.
    /// </summary>
    /// <seealso cref="IDisposable" />
    /// <seealso cref="IService" />
    public interface IVsProjectEventsPublisher : IDisposable, IService
    {
        /// <summary>
        /// Registers the listeners.
        /// </summary>
        void RegisterListeners();
    }
}
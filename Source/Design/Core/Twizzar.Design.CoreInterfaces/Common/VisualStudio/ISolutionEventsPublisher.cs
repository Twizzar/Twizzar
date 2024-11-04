using System;

using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.CoreInterfaces.Common.VisualStudio
{
    /// <summary>
    /// Service for publishing events to the IEventHub when there are changes in the solution.
    /// </summary>
    /// <seealso cref="IService" />
    /// <seealso cref="System.IDisposable" />
    public interface ISolutionEventsPublisher : IDisposable, IInitializableService
    {
    }
}
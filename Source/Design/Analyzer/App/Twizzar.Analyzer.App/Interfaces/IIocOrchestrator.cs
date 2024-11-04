using System;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;

namespace Twizzar.Analyzer.Core.Interfaces
{
    /// <summary>
    /// Does everything containing IoC.
    /// </summary>
    public interface IIocOrchestrator : IHasEnsureHelper, IHasLogger, IDisposable
    {
        /// <summary>
        /// Resolves a Interface of T to generate a member of the registered base type.
        /// </summary>
        /// <typeparam name="T">The type of the instance that should be generated.</typeparam>
        /// <returns>A instance of the resolved type T.</returns>
        T Resolve<T>();
    }
}
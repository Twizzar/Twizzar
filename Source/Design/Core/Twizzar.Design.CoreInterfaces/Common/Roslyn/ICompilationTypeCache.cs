using System.Collections.Generic;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.CoreInterfaces.Common.Roslyn;

/// <summary>
/// Cache for compilation types.
/// </summary>
public interface ICompilationTypeCache : IInitializableService
{
    /// <summary>
    /// Get all types for a certain assembly name.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    IReadOnlyList<ITypeDescription> GetAllTypeForAssembly(string assemblyName);
}
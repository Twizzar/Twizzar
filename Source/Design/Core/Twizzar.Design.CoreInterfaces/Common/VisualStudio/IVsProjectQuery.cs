using System.Threading.Tasks;

using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.Common.VisualStudio;

/// <summary>
/// Query for finding projects in the IDE.
/// </summary>
public interface IVsProjectQuery
{
    /// <summary>
    /// Find the project.
    /// </summary>
    /// <param name="name">The name of the project, not the file path.</param>
    /// <returns>Some if found; else None.</returns>
    Task<Maybe<IIdeProject>> FindProjectAsync(string name);
}
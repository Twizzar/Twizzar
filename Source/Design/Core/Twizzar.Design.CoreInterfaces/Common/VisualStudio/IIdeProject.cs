using System.Threading.Tasks;
using ViCommon.Functional;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.CoreInterfaces.Common.VisualStudio;

/// <summary>
/// Represents a project which can access IDE specific function to manipulate itself.
/// </summary>
public interface IIdeProject : IViProject
{
    #region properties

    /// <summary>
    /// Gets a value indicating whether the project was loaded.
    /// </summary>
    bool IsLoaded { get; }

    #endregion

    #region members

    /// <summary>
    /// Add a nuget package to the project.
    /// <remarks>If the nuget package is already references this does nothing.</remarks>
    /// </summary>
    /// <param name="packageId">The nuget package id, for example <c>Twizzar.Api</c>.</param>
    /// <param name="version">If null then take the latest.</param>
    /// <returns></returns>
    Task AddNugetPackageAsync(string packageId, string version = null);

    /// <summary>
    /// Add a project reference to this project.
    /// <remarks>If the project  is already references this does nothing.</remarks>
    /// </summary>
    /// <param name="project">The project to add as reference.</param>
    /// <returns>Success if successful else a <see cref="Failure"/> with the message why it failed.</returns>
    Task<IResult<Unit, Failure>> AddProjectReferenceAsync(IViProject project);

    #endregion
}
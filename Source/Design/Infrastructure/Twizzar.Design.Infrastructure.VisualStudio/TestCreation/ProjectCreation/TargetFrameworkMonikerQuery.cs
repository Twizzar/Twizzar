using System.Diagnostics.CodeAnalysis;
using Microsoft.Build.Evaluation;
using Twizzar.Design.CoreInterfaces.TestCreation.ProjectCreation;

namespace TestCreation.ProjectCreation;

/// <inheritdoc crref="ITargetFrameworkMonikerQuery" />
[ExcludeFromCodeCoverage]
public class TargetFrameworkMonikerQuery : ITargetFrameworkMonikerQuery
{
    #region members

    /// <inheritdoc />
    public string GetTargetFrameworkMoniker(string projectPath)
    {
        var projectCollection = new ProjectCollection();
        var msProject = projectCollection.LoadProject(projectPath);
        return msProject.GetPropertyValue("TargetFramework");
    }

    #endregion
}
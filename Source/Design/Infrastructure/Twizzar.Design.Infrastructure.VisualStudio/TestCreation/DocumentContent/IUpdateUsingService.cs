using Microsoft.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.DocumentContent;

/// <summary>
/// Interface for update usings.
/// </summary>
public interface IUpdateUsingService
{
    #region members

    /// <summary>
    /// Updates the usings.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="roslynRootNode"></param>
    /// <param name="templateContext"></param>
    /// <returns></returns>
    Document UpdateUsings(
        Document document,
        SyntaxNode roslynRootNode,
        ITemplateContext templateContext);

    #endregion
}
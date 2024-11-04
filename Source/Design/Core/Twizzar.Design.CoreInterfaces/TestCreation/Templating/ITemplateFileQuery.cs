using System.Threading;
using System.Threading.Tasks;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.CoreInterfaces.TestCreation.Templating;

/// <summary>
/// Query for loading the <see cref="ITemplateFile"/>.
/// </summary>
public interface ITemplateFileQuery
{
    #region members

    /// <summary>
    /// Gets the <see cref="ITemplateFile"/> from a path async.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<IResult<ITemplateFile, Failure>> GetAsync(string filePath, CancellationToken token);

    /// <summary>
    /// Gets the <see cref="ITemplateFile" /> for the default template.
    /// </summary>
    /// <returns></returns>
    Task<ITemplateFile> GetDefaultAsync();

    #endregion
}
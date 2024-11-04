using System.Threading.Tasks;

using Twizzar.Design.CoreInterfaces.TestCreation.Templating;

namespace Twizzar.Design.CoreInterfaces.TestCreation;

/// <summary>
/// Parse the template and add it to the context.
/// </summary>
public interface ITemplateService
{
    /// <summary>
    /// Add the template to the context.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    Task<CreationContext> AddTemplate(CreationContext source, CreationContext destination, ITemplateFile file);
}
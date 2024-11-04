using System.Threading;
using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.TestCreation.BaseConfig;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;

namespace Twizzar.Design.CoreInterfaces.TestCreation.Config;

/// <summary>
/// Config query for loading all config files.
/// </summary>
public interface IBaseConfigQuery
{
    /// <summary>
    /// Get or create the configs. When they not exists the configs will be created and an exception will be thrown.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="ConfigFilesNotExistsException">When one or more of the config files does not exits.</exception>
    /// <exception cref="InternalException">When an error occurred, for example a parse error.</exception>
    Task<(TestCreationConfig Config, ITemplateFile TemplateFile)> GetOrCreateConfigsAsync(CreationInfo info, CancellationToken token);
}
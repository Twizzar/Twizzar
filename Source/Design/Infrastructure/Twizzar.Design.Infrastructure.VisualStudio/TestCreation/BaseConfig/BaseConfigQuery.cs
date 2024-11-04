using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.Design.CoreInterfaces.TestCreation.BaseConfig;
using Twizzar.Design.CoreInterfaces.TestCreation.Config;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.BaseConfig;

/// <inheritdoc cref="IBaseConfigQuery"/>
public class BaseConfigQuery : IBaseConfigQuery
{
    #region static fields and constants

    private const string TemplateConfigFileName = "twizzar.template";
    private const string ConfigFileName = "twizzar.config";

    #endregion

    #region fields

    private readonly ITemplateFileQuery _templateFileQuery;
    private readonly IConfigQuery _configQuery;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseConfigQuery"/> class.
    /// </summary>
    /// <param name="templateFileQuery"></param>
    /// <param name="configQuery"></param>
    public BaseConfigQuery(ITemplateFileQuery templateFileQuery, IConfigQuery configQuery)
    {
        EnsureHelper.GetDefault.Many()
            .Parameter(templateFileQuery, nameof(templateFileQuery))
            .Parameter(configQuery, nameof(configQuery))
            .ThrowWhenNull();

        this._templateFileQuery = templateFileQuery;
        this._configQuery = configQuery;
    }

    #endregion

    #region members

    /// <inheritdoc />
    public async Task<(TestCreationConfig Config, ITemplateFile TemplateFile)> GetOrCreateConfigsAsync(
        CreationInfo info,
        CancellationToken token)
    {
        var solutionFolder = info.Solution.SplitPath().Prefix;
        var configPath = Path.Combine(solutionFolder, ConfigFileName);
        var templateFilePath = Path.Combine(solutionFolder, TemplateConfigFileName);

        var configFileResult = await this._configQuery.GetConfigAsync(configPath, token);
        var templateFileResult = await this.LoadTemplateFileAsync(templateFilePath, token);

        return (configFileResult.AsResultValue(), templateFileResult.AsResultValue()) switch
        {
            (FailureValue<Failure> { Value: ConfigFileNotExistsFailure f1 }, FailureValue<Failure>
                {
                    Value: ConfigFileNotExistsFailure f2,
                }) =>
                throw new ConfigFilesNotExistsException(f1.FilePath, f2.FilePath),

            (SuccessValue<TestCreationConfig>, FailureValue<Failure> { Value: ConfigFileNotExistsFailure f2 }) =>
                throw new ConfigFilesNotExistsException(f2.FilePath),

            (FailureValue<Failure> { Value: ConfigFileNotExistsFailure f1 }, SuccessValue<ITemplateFile> _) =>
                throw new ConfigFilesNotExistsException(f1.FilePath),

            (SuccessValue<TestCreationConfig> config, SuccessValue<ITemplateFile> templateFile) =>
                (config.Value, templateFile.Value),

            (FailureValue<Failure> { Value: { } f1 }, _) =>
                throw new InternalException(f1.Message),

            (_, FailureValue<Failure> { Value: { } f2 }) =>
                throw new InternalException(f2.Message),

            _ => throw new InternalException("Cannot load the config files, but could not find the error message."),
        };
    }

    private async Task<IResult<ITemplateFile, Failure>> LoadTemplateFileAsync(string filePath, CancellationToken token)
    {
        var templateFile = await this._templateFileQuery.GetAsync(filePath, token);
        var defaultFile = await this._templateFileQuery.GetDefaultAsync();
        return templateFile.MapSuccess(file => file.WithBackupFile(defaultFile));
    }

    #endregion
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.TestCreation.BaseConfig;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

/// <summary>
/// Dummy implementation for getting the template form a file.
/// </summary>
public class TemplateFileQuery : ITemplateFileQuery
{
    #region static fields and constants

    private readonly ITemplateSnippetFactory _snippetFactory;
    private readonly ITemplateFileService _templateFileService;
    private readonly IBaseConfigParser _baseConfigParser;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateFileQuery"/> class.
    /// </summary>
    /// <param name="snippetFactory">Snippet factory used for parsing file content.</param>
    /// <param name="templateFileService">Service for accessing a file in local file system.</param>
    /// <param name="baseConfigParser"></param>
    public TemplateFileQuery(
        ITemplateSnippetFactory snippetFactory,
        ITemplateFileService templateFileService,
        IBaseConfigParser baseConfigParser)
    {
        EnsureHelper.GetDefault.Many()
            .Parameter(snippetFactory, nameof(snippetFactory))
            .Parameter(templateFileService, nameof(templateFileService))
            .Parameter(baseConfigParser, nameof(baseConfigParser))
            .ThrowWhenNull();

        this._snippetFactory = snippetFactory;
        this._templateFileService = templateFileService;
        this._baseConfigParser = baseConfigParser;
    }

    #region members

    /// <inheritdoc />
    public async Task<IResult<ITemplateFile, Failure>> GetAsync(string filePath, CancellationToken token)
    {
        var maybeReader = await this._templateFileService.GetFileReaderAsync(filePath);

        if (maybeReader.AsMaybeValue() is not SomeValue<TextReader> someReader)
        {
            await this._templateFileService.CreateDefaultFile(filePath);
            return new ConfigFileNotExistsFailure(filePath)
                .ToResult<ITemplateFile, ConfigFileNotExistsFailure>();
        }

        using var reader = someReader.Value;

        return this.Parse(reader)
            .MapFailure(failure =>
                new Failure(
                    $"{filePath} contains an syntax error:{Environment.NewLine}{failure.Message} at {failure.OutputPoint.LineAndColumn}."))
            .MapSuccess(snippets => new TemplateFile(filePath, snippets.ToList()));
    }

    /// <inheritdoc />
    public async Task<ITemplateFile> GetDefaultAsync()
    {
        using var reader = await this._templateFileService.GetDefaultFileReaderAsync();

        var snippets = this.Parse(reader);

        return new TemplateFile("default", snippets.GetSuccessUnsafe().ToList());
    }

    private IResult<IEnumerable<ITemplateSnippet>, ParseFailure> Parse(TextReader stringReader) =>
        this._baseConfigParser.ParseBaseConfig(stringReader.ReadToEnd())
            .MapSuccess(this.MapFunc);

    private IEnumerable<ITemplateSnippet> MapFunc(ConfigSyntax configSyntax) =>
        configSyntax.Entries
            .Select(entry =>
                this._snippetFactory.Create(entry.Tag, entry.Content.TrimEnd(' ', '\n', '\r')));

    #endregion
}
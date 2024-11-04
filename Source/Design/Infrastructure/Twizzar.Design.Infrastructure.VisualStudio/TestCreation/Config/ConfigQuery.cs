using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.TestCreation.BaseConfig;
using Twizzar.Design.CoreInterfaces.TestCreation.Config;
using Twizzar.Design.CoreInterfaces.TestCreation.Mapping;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Mapping;

/// <inheritdoc cref="IConfigQuery" />
public class ConfigQuery : IConfigQuery
{
    #region fields

    private readonly IConfigFileService _configFileService;
    private readonly IBaseConfigParser _baseConfigParser;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigQuery"/> class.
    /// </summary>
    /// <param name="configFileService"></param>
    /// <param name="baseConfigParser"></param>
    public ConfigQuery(IConfigFileService configFileService, IBaseConfigParser baseConfigParser)
    {
        EnsureHelper.GetDefault.Many()
            .Parameter(configFileService, nameof(configFileService))
            .Parameter(baseConfigParser, nameof(baseConfigParser))
            .ThrowWhenNull();

        this._configFileService = configFileService;
        this._baseConfigParser = baseConfigParser;
    }

    #endregion

    #region members

    /// <inheritdoc />
    public async Task<IResult<TestCreationConfig, Failure>> GetConfigAsync(
        string filePath,
        CancellationToken cancellationToken)
    {
        var maybeReader = await this._configFileService.GetFileReaderAsync(filePath);
        maybeReader.IfNone(() => this._configFileService.CreateDefaultFile(filePath));

        return maybeReader
            .ToResult((Failure)new ConfigFileNotExistsFailure(filePath))
            .MapSuccess(reader =>
            {
                using var r = reader;
                return r.ReadToEnd();
            })
            .Bind(text => this.Parse(text, filePath))
            .Bind(GetConfig)
            .MapFailure(failure => failure is not ConfigFileNotExistsFailure
                ? new Failure($"{filePath} contains an error: {failure.Message}")
                : failure);
    }

    private IResult<ConfigSyntax, Failure> Parse(string text, string filePath) =>
        this._baseConfigParser.ParseBaseConfig(text)
            .MapFailure(failure =>
                new Failure(
                    $"{filePath} contains an syntax error:{Environment.NewLine}{failure.Message} at {failure.OutputPoint.LineAndColumn}."));

    private static IResult<TestCreationConfig, Failure> GetConfig(ConfigSyntax configSyntax)
    {
        var entries = configSyntax.Entries.ToImmutableDictionary(entry => entry.Tag);

        return
            from projectMapping in GetValueKeyEntries(entries, "testProject")
            from projectPathMapping in GetValueKeyEntries(entries, "testProjectPath")
            from fileMapping in GetValueKeyEntries(entries, "testFile")
            from filePathMapping in GetValueKeyEntries(entries, "testFilePath")
            from namespaceMapping in GetValueKeyEntries(entries, "testNamespace")
            from typeMapping in GetValueKeyEntries(entries, "testClass")
            from memberMapping in GetValueKeyEntries(entries, "testMethod")
            from nugetPackages in GetValueKeyEntries(entries, "nugetPackages")
            select new TestCreationConfig(
                new MappingConfig(
                    ToMappingEntries(projectMapping),
                    ToMappingEntries(projectPathMapping),
                    ToMappingEntries(fileMapping),
                    ToMappingEntries(filePathMapping),
                    ToMappingEntries(namespaceMapping),
                    ToMappingEntries(typeMapping),
                    ToMappingEntries(memberMapping)),
                nugetPackages.ToImmutableArray());
    }

    private static ImmutableArray<MappingEntry> ToMappingEntries(
        IEnumerable<(string Key, Maybe<string> Value)> keyValuePairs) =>
        keyValuePairs
            .Select(tuple => tuple.Value.IsSome
                ? new MappingEntry(tuple.Key, tuple.Value.GetValueUnsafe())
                : new MappingEntry(Maybe.None(), tuple.Key))
            .ToImmutableArray();

    private static IResult<ConfigEntry, Failure> GetEntry(
        IImmutableDictionary<string, ConfigEntry> dictionary,
        string tag) =>
        dictionary.GetMaybe(tag)
            .ToResult(new Failure($"No entry for the Tag {tag} was found in the config file."));

    private static IResult<IEnumerable<(string Key, Maybe<string> Value)>, Failure> GetValueKeyEntries(
        IImmutableDictionary<string, ConfigEntry> entries,
        string tag) =>
        GetEntry(entries, tag)
            .Bind(entry => ParseMappingEntries(entry.Content));

    private static IResult<IEnumerable<(string Key, Maybe<string> Value)>, Failure> ParseMappingEntries(string text)
    {
        var list = new List<(string, Maybe<string>)>();

        foreach (var result in text.Split(new[] { "\n\r", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                     .Select(ParseKeyValueEntry))
        {
            if (result.AsResultValue() is SuccessValue<(string, Maybe<string>)> success)
            {
                list.Add(success);
            }
            else
            {
                return result.GetFailureUnsafe()
                    .ToResult<IEnumerable<(string, Maybe<string>)>, Failure>();
            }
        }

        return list.ToSuccess<IEnumerable<(string, Maybe<string>)>, Failure>();
    }

    private static IResult<(string Key, Maybe<string> Value), Failure> ParseKeyValueEntry(string line)
    {
        var split = line.Split(':');

        return split switch
        {
            { Length: 1 } =>
                Result.Success<(string, Maybe<string>), Failure>(
                    (split[0].Trim(), Maybe.None())),
            { Length: 2 } =>
                Result.Success<(string, Maybe<string>), Failure>(
                    (split[0].Trim(), Maybe.Some(split[1].Trim()))),
            _ => new Failure("Every mapping entry should maximum have one :")
                .ToResult<(string, Maybe<string>), Failure>(),
        };
    }

    #endregion
}
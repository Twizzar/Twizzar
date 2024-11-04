using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.Design.CoreInterfaces.TestCreation.Config;
using Twizzar.Design.CoreInterfaces.TestCreation.Mapping;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.ResultMonad;

namespace TestCreation.Services;

/// <inheritdoc cref="IMappingService" />
public class MappingService : IMappingService
{
    #region fields

    private readonly IWildcardPatternMatcher _patternMatcher;
    private readonly IMappingReplacementService _mappingReplacementService;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="MappingService"/> class.
    /// </summary>
    /// <param name="patternMatcher"></param>
    /// <param name="mappingReplacementService"></param>
    public MappingService(
        IWildcardPatternMatcher patternMatcher,
        IMappingReplacementService mappingReplacementService)
    {
        EnsureHelper.GetDefault.Many()
            .Parameter(patternMatcher, nameof(patternMatcher))
            .Parameter(mappingReplacementService, nameof(mappingReplacementService))
            .ThrowWhenNull();

        this._patternMatcher = patternMatcher;
        this._mappingReplacementService = mappingReplacementService;
    }

    #endregion

    #region members

    /// <inheritdoc />
    public Task<CreationInfo> MapAsync(CreationInfo source, TestCreationConfig config)
    {
        var mappingConfig = config.MappingConfig;
        mappingConfig = this._mappingReplacementService.ReplacePlaceholders(mappingConfig, source);

        var mappingResult = this.Map(source, mappingConfig);

        return Task.FromResult(mappingResult.Match(
            info => info,
            failure => throw new InternalException(failure.Message)));
    }

    private IResult<CreationInfo, Failure> Map(CreationInfo source, MappingConfig mappingConfig) =>
        from project in this.MapFile(source.Project, mappingConfig.ProjectMapping)
        from projectPath in this.MapPath(source.Project, mappingConfig.ProjectPathMapping)
        from file in this.MapFile(source.File, mappingConfig.FileMapping)
        from filePath in this.MapRelativePath(source.File, source.Project.SplitPath().Prefix, mappingConfig.FilePathMapping)
        from ns in this.Map(source.Namespace, mappingConfig.NamespaceMapping)
        from type in this.Map(source.Type, mappingConfig.TypeMapping)
        from member in this.Map(source.Member, mappingConfig.MemberMapping)
        let filePrefix = source.File.SplitPath().Prefix
        let projectPrefix = source.Project.SplitPath().Prefix
        select new CreationInfo(
            source.Solution,
            Path.Combine(projectPath, project),
            Path.Combine(
                projectPath,
                filePath,
                file),
            ns,
            type,
            member);

    private IResult<string, Failure> Map(string name, IEnumerable<MappingEntry> mapping) =>
        this._patternMatcher.Match(name, mapping);

    private IResult<string, Failure> MapFile(string path, IEnumerable<MappingEntry> mapping)
    {
        var (_, fileName, extension) = path.SplitPath();

        if (fileName == string.Empty)
        {
            return new Failure($"{path} is a directory and not a file path.")
                .ToResult<string, Failure>();
        }

        return this._patternMatcher.Match(fileName, mapping)
            .MapSuccess(targetFileName => $"{targetFileName}{extension}");
    }

    private IResult<string, Failure> MapPath(string path, IEnumerable<MappingEntry> mapping)
    {
        var (filePrefix, _, _) = path.SplitPath();

        return this._patternMatcher.Match(filePrefix, mapping)
            .MapSuccess(Path.GetFullPath);
    }

    private IResult<string, Failure> MapRelativePath(
        string absolutePath,
        string relativePart,
        IEnumerable<MappingEntry> mapping)
    {
        var path = absolutePath.ReplaceSafe(relativePart, string.Empty);
        var (filePrefix, _, _) = path.SplitPath();

        return this._patternMatcher.Match(filePrefix, mapping)
            .MapSuccess(NormalizePath);
    }

    private static string NormalizePath(string path) =>
        path
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar);

    #endregion
}
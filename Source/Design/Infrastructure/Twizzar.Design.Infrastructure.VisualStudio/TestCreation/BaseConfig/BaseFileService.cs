using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.TestCreation.BaseConfig;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.BaseConfig;

/// <inheritdoc cref="IBaseFileService"/>
[ExcludeFromCodeCoverage]
public abstract class BaseFileService : IBaseFileService
{
    /// <summary>
    /// Gets the default content.
    /// </summary>
    protected abstract string Default { get; }

    /// <inheritdoc />
    public Task<Maybe<TextReader>> GetFileReaderAsync(string filePath) =>
        File.Exists(filePath)
            ? Task.FromResult<Maybe<TextReader>>(
                new StreamReader(File.OpenRead(filePath)))
            : Task.FromResult<Maybe<TextReader>>(
                Maybe.None());

    /// <inheritdoc />
    public Task<TextReader> GetDefaultFileReaderAsync() =>
        Task.FromResult<TextReader>(
            new StringReader(this.Default));

    /// <inheritdoc />
    public async Task CreateDefaultFile(string filePath)
    {
        using var writer = new StreamWriter(
            File.Create(filePath));

        await writer.WriteAsync(this.Default);
        await writer.FlushAsync();
    }
}
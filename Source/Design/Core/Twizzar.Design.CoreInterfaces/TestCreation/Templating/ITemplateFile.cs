using System.Collections.Generic;
using System.Collections.Immutable;

namespace Twizzar.Design.CoreInterfaces.TestCreation.Templating;

/// <summary>
/// Container for all the template snippets.
/// </summary>s
public interface ITemplateFile
{
    #region properties

    /// <summary>
    /// Gets all the snippets.
    /// </summary>
    IImmutableDictionary<SnippetType, IReadOnlyList<ITemplateSnippet>> Snippets { get; }

    /// <summary>
    /// Gets the file path.
    /// </summary>
    string Path { get; }

    #endregion

    #region members

    /// <summary>
    /// Gets a single snippet by its type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    ITemplateSnippet GetSingleSnipped(SnippetType type);

    /// <summary>
    /// Add a backup file, this fill will be used if the snippets are not present in the current file.
    /// </summary>
    /// <param name="templateFile"></param>
    /// <returns></returns>
    ITemplateFile WithBackupFile(ITemplateFile templateFile);

    #endregion
}
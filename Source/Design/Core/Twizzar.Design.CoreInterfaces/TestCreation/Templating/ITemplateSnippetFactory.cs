namespace Twizzar.Design.CoreInterfaces.TestCreation.Templating;

/// <summary>
/// Factory for creating the <see cref="ITemplateSnippet"/>.
/// </summary>
public interface ITemplateSnippetFactory
{
    /// <summary>
    /// Check if the line is a tag.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    bool IsTag(string line);

    /// <summary>
    /// Extract the tag from the line.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    string ExtractTag(string line);

    /// <summary>
    /// Create a new <see cref="ITemplateSnippet"/>.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    ITemplateSnippet Create(string tag, string content);
}
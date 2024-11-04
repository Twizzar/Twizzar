namespace Twizzar.Design.CoreInterfaces.TestCreation.Templating;

/// <summary>
///  Represents a snipped of code in the template config, which can also reference to other <see cref="ITemplateSnippet"/>.
/// </summary>
public interface ITemplateSnippet
{
    /// <summary>
    /// Gets the string which will be replaced by this snipped in the template.
    /// </summary>
    string TagUsage { get; }

    /// <summary>
    /// Gets the content of the snipped. This string contains non replaced variable and tags.
    /// </summary>
    string Content { get; }

    /// <summary>
    /// Gets the snipped type.
    /// </summary>
    SnippetType Type { get; }

    /// <summary>
    /// Create a new snipped with the same tag and type but a different content.
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    ITemplateSnippet WithContent(string content);
}
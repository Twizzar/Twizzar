using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

/// <inheritdoc cref="ITemplateSnippet"/>
[ExcludeFromCodeCoverage]
public record TemplateSnippet(
    string Tag,
    string Content,
    SnippetType Type) : ITemplateSnippet
{
    /// <inheritdoc />
    public string TagUsage => $"<{this.Tag}>";

    /// <inheritdoc />
    public ITemplateSnippet WithContent(string content) =>
        this with { Content = content };

    /// <summary>
    /// Create a empty <see cref="TemplateSnippet"/>.
    /// </summary>
    /// <returns></returns>
    public static TemplateSnippet Empty() => new(string.Empty, string.Empty, SnippetType.Default);

    /// <summary>
    /// Create a new <see cref="TemplateSnippet"/>.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public static TemplateSnippet Create(SnippetType type, string content = "") =>
        new(type.ToTag(), content, type);
}
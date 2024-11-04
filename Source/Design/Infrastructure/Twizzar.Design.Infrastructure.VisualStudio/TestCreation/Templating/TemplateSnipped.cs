using Twizzar.Design.CoreInterfaces.TestCreation.Templating;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

public record TemplateSnippet(
    string Tag,
    string Content,
    SnippetType Type) : ITemplateSnippet
{
    public string TagUsage => $"<{this.Tag}>";
    public string TagDefinition => $"[{this.Tag}]";

    /// <inheritdoc />
    public ITemplateSnippet WithContent(string content) =>
        this with { Content = content};

    public static TemplateSnippet Empty() => new(string.Empty, string.Empty, SnippetType.Default);

    public static TemplateSnippet Create(SnippetType type, string content = "") =>
        new TemplateSnippet(type.ToTag(), content, type);
}
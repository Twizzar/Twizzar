using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

/// <inheritdoc cref="ITemplateSnippetFactory" />
[ExcludeFromCodeCoverage]
public class TemplateSnippetFactory : ITemplateSnippetFactory
{
    private static readonly Regex IsTagRegex = new(@"^\[[a-zA-Z][a-zA-Z0-9_-]*:\]");

    /// <inheritdoc />
    public bool IsTag(string line) =>
        IsTagRegex.IsMatch(line);

    /// <inheritdoc />
    public string ExtractTag(string line) =>
        line.Replace("[", string.Empty)
            .Replace(":]", string.Empty);

    /// <inheritdoc />
    public ITemplateSnippet Create(string tag, string content) =>
        new TemplateSnippet(
            tag,
            content,
            tag.ToSnippedType());
}
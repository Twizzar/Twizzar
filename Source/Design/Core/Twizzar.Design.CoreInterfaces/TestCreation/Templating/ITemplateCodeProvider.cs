namespace Twizzar.Design.CoreInterfaces.TestCreation.Templating;

/// <summary>
/// Provides code snippets.
/// </summary>
public interface ITemplateCodeProvider
{
    /// <summary>
    /// Returns a code snippet for the provided snippet type.
    /// </summary>
    /// <param name="type">The snipped type.
    /// This cannot be <see cref="SnippetType.Default"/> or <see cref="SnippetType.Warning"/>.</param>
    /// <param name="context"></param>
    /// <returns>Code snipped as a string.</returns>
    string GetCode(SnippetType type, ITemplateContext context);
}
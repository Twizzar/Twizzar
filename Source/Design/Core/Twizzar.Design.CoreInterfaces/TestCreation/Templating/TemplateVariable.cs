namespace Twizzar.Design.CoreInterfaces.TestCreation.Templating;

/// <summary>
/// Tag and value for a template variable.
/// </summary>
/// <param name="Tag"></param>
/// <param name="Value"></param>
public record TemplateVariable(string Tag, string Value);
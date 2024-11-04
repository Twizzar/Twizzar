using System.Collections.Immutable;

namespace Twizzar.Design.CoreInterfaces.TestCreation.Templating;

/// <summary>
/// Context needed for generating code snipped form the template.
/// </summary>
public interface ITemplateContext
{
    #region properties

    /// <summary>
    /// Gets the source context, this represent the member form which the test should be created.
    /// </summary>
    CreationContext SourceCreationContext { get; }

    /// <summary>
    /// Gets the target context, this are the information about the test which should be created.
    /// </summary>
    CreationContext TargetCreationContext { get; }

    /// <summary>
    /// Gets the template file. This will be provided by the <see cref="ITemplateFileQuery"/>.
    /// </summary>
    ITemplateFile File { get; }

    /// <summary>
    /// Gets the additional usings which will be added conditionally to the static usings.
    /// </summary>
    IImmutableSet<string> AdditionalUsings { get; }

    #endregion

    #region members

    /// <summary>
    /// Replace <see cref="AdditionalUsings"/>.
    /// </summary>
    /// <param name="usings"></param>
    /// <returns></returns>
    ITemplateContext WithAdditionalUsings(IImmutableSet<string> usings);

    #endregion
}
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

/// <summary>
/// Context needed for creating code form the template.
/// </summary>
/// <param name="SourceCreationContext"></param>
/// <param name="TargetCreationContext"></param>
/// <param name="File"></param>
/// <param name="AdditionalUsings"></param>
[ExcludeFromCodeCoverage]
public record TemplateContext(
    CreationContext SourceCreationContext,
    CreationContext TargetCreationContext,
    ITemplateFile File,
    IImmutableSet<string> AdditionalUsings) : ITemplateContext
{
    #region Implementation of ITemplateContext

    /// <inheritdoc />
    public ITemplateContext WithAdditionalUsings(IImmutableSet<string> usings) => this with
    {
        AdditionalUsings = usings,
    };

    #endregion
}
using System;
using System.Diagnostics.CodeAnalysis;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

/// <summary>
/// Exception thrown when the snipped was not found.
/// </summary>
[ExcludeFromCodeCoverage]
public class TemplateSnippetNotFoundException : Exception
{
    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateSnippetNotFoundException"/> class.
    /// </summary>
    /// <param name="tag"></param>
    public TemplateSnippetNotFoundException(string tag)
        : base($"Template snipped with the tag {tag} was not found.")
    {
        this.Tag = tag;
    }

    #endregion

    #region properties

    /// <summary>
    /// Gets the tag of the snipped.
    /// </summary>
    public string Tag { get; }

    #endregion
}
using System;
using System.Diagnostics.CodeAnalysis;

using Twizzar.Design.CoreInterfaces.TestCreation.Templating;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

/// <summary>
/// Exception thrown when more than one snipped was found for one tag.
/// </summary>
[ExcludeFromCodeCoverage]
public class TemplateSnippetDuplicateException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateSnippetDuplicateException"/> class.
    /// </summary>
    /// <param name="type"></param>
    public TemplateSnippetDuplicateException(SnippetType type)
        : base($"More than one tag found for {type.ToTag()}")
    {
    }
}
using System.Collections.Generic;
using System.Linq;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

/// <inheritdoc cref="ITemplateVisitor" />
public class TemplateVisitor : ITemplateVisitor
{
    private readonly ISnippetNodeFactory _nodeFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateVisitor"/> class.
    /// </summary>
    /// <param name="nodeFactory"></param>
    public TemplateVisitor(ISnippetNodeFactory nodeFactory)
    {
        EnsureHelper.GetDefault.Parameter(nodeFactory, nameof(nodeFactory)).ThrowWhenNull();

        this._nodeFactory = nodeFactory;
    }

    /// <inheritdoc/>
    public string Visit(ITemplateSnippet snippet, ITemplateContext context) =>
        this.Visit(this._nodeFactory.Create(snippet, context, Maybe.None()));

    private string Visit(ISnippetNode current)
    {
        this.VisitInternal(current);

        return current.GetCode();
    }

    private void VisitInternal(ISnippetNode template)
    {
        foreach (var currentTemplate in this.GetUsedTemplates(template))
        {
            if (currentTemplate.IsCyclic)
            {
                continue;
            }

            this.VisitInternal(currentTemplate);
        }
    }

    private IEnumerable<ISnippetNode> GetUsedTemplates(ISnippetNode template) =>
        template.File.Snippets.Values
            .SelectMany(list => list)
            .Where(snipped => ContainsTag(template.Snippet.Content, snipped))
            .Select(t => this._nodeFactory.Create(t, template.Context, Maybe.Some(template)));

    private static bool ContainsTag(string content, ITemplateSnippet template) =>
        content.Contains(template.TagUsage);
}
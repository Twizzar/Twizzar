using System;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

/// <inheritdoc cref="ISnippetNodeFactory"/>
public class SnippetNodeFactory : ISnippetNodeFactory
{
    private readonly IShortTypesConverter _shortTypesConverter;

    /// <summary>
    /// Initializes a new instance of the <see cref="SnippetNodeFactory"/> class.
    /// </summary>
    /// <param name="shortTypesConverter"></param>
    public SnippetNodeFactory(IShortTypesConverter shortTypesConverter)
    {
        this._shortTypesConverter = shortTypesConverter ?? throw new ArgumentNullException(nameof(shortTypesConverter));
    }

    /// <inheritdoc/>
    public ISnippetNode Create(ITemplateSnippet snippet, ITemplateContext context, Maybe<ISnippetNode> parent) =>
        snippet.Type switch
        {
            SnippetType.ArgumentUsing => new ArgumentUsingsNode(snippet, context, parent),
            SnippetType.Arrange => new ArrangeNode(snippet, context, parent),
            SnippetType.Act => new ActNode(snippet, context, parent),
            SnippetType.ArgumentArrange => new ArgumentArrangeNode(snippet, context, parent, this._shortTypesConverter),
            SnippetType.MethodSignature => new MethodSignatureNode(snippet, context, parent),
            _ => new SnippetNode(snippet, context, parent),
        };
}
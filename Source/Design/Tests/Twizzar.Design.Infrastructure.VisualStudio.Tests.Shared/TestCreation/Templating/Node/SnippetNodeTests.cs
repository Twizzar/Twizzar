using FluentAssertions;

using NUnit.Framework;

using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.TestCreation.Templating.Node;

[TestFixture]
public class SnippetNodeTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // arrange
        var context = new ItemBuilder<ITemplateContext>()
            .With(p => p.File.Stub<ITemplateFile>())
            .Build();

        // assert
        Verify.Ctor<SnippetNode>()
            .SetupParameter("context", context)
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void Node_with_no_children_is_not_Cyclic()
    {
        // arrange
        var node = new SnippetNodeBuilder()
            .Build();

        // act
        var isCyclic = node.IsCyclic;

        // assert
        isCyclic.Should().BeFalse();
    }

    [Test]
    public void Node_with_cycle_children_is_not_Cyclic()
    {
        // arrange
        var nodeA = new SnippetNodeBuilder()
            .Build();

        var nodeB = new SnippetNodeBuilder()
            .Build();

        var nodeC = new SnippetNodeBuilder()
            .Build();

        // act

        /*
         *      a
         *     ↗ ↘
         *    c ← b
         */
        nodeA.Add(nodeB);
        nodeB.Add(nodeC);
        nodeC.Add(nodeA);

        var isCyclic = nodeA.IsCyclic && nodeB.IsCyclic && nodeC.IsCyclic;

        // assert
        isCyclic.Should().BeTrue();
        nodeA.Content.Should().Contain("!!! WARNING");
    }

    [Test]
    public void Node_will_replace_tags_with_children()
    {
        // arrange
        var node = new SnippetNodeBuilder()
            .With(p => p.Snippet.Content.Value("<a> <b>"))
            .Build();

        new SnippetNodeBuilder()
            .With(p => p.Snippet.Content.Value("One"))
            .With(p => p.Snippet.TagUsage.Value("<a>"))
            .With(p => p.Ctor.parent.Value(node))
            .Build();

        new SnippetNodeBuilder()
            .With(p => p.Snippet.Content.Value("Two"))
            .With(p => p.Snippet.TagUsage.Value("<b>"))
            .With(p => p.Ctor.parent.Value(node))
            .Build();

        // act
        var code = node.Content;

        // assert
        code.Should().Be("One Two");
    }

    private class SnippetNodeBuilder : ItemBuilder<SnippetNode, SnippetNodeCustomPaths>
    {
        public SnippetNodeBuilder()
        {
            this.With(p => p.Ctor.context.File.Stub<ITemplateFile>());
        }
    }
}
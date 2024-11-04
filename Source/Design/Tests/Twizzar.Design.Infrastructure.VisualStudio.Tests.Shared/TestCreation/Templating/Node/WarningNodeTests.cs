using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.TestCreation.Templating.Node;

[TestFixture]
public class WarningNodeTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // arrange
        var context = new ItemBuilder<ITemplateContext>()
            .With(p => p.File.Stub<ITemplateFile>())
            .Build();

        // assert
        Verify.Ctor<WarningNode>()
            .SetupParameter("context", context)
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void Content_contains_Warning()
    {
        // arrange
        var node = WarningNode.Create(
            new ItemBuilder<ISnippetNode>()
                .With(p => p.Context.File.Stub<ITemplateFile>())
                .With(p => p.File.Path.Value("PATH"))
                .With(p => p.Snippet.TagUsage.Value("<TAG>"))
                .Build());

        // act & assert
        node.Content.Should().Contain("!!! WARNING");
    }
}
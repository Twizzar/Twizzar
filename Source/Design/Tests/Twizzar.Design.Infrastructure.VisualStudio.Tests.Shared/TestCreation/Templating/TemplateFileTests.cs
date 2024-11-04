using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.TestCreation.Templating;

[TestFixture]
public class TemplateFileTests
{
    [Test]
    public void Correct_snipped_is_return_when_backup_is_set()
    {
        // arrange
        var expectedFileSnip = new ItemBuilder<ITemplateSnippet>()
            .With(p => p.Type.Value(SnippetType.File))
            .Build();

        var expectedClassSnip = new ItemBuilder<ITemplateSnippet>()
            .With(p => p.Type.Value(SnippetType.Class))
            .Build();

        var otherFileSnip = new ItemBuilder<ITemplateSnippet>()
            .With(p => p.Type.Value(SnippetType.File))
            .Build();

        var sut = new TemplateFile("", new[] { expectedFileSnip });
        var backup = new TemplateFile("", new[] { otherFileSnip, expectedClassSnip });

        // act
        var newFile = sut.WithBackupFile(backup);

        var actSnip = newFile.GetSingleSnipped(SnippetType.File);
        var classSnip = newFile.GetSingleSnipped(SnippetType.Class);

        // assert
        actSnip.Should().Be(expectedFileSnip);
        classSnip.Should().Be(expectedClassSnip);
    }
}
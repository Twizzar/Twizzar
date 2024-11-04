using System.Collections.Immutable;

using FluentAssertions;

using NUnit.Framework;

using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.TestCreation.Templating.Node;

[TestFixture]
public class ArgumentUsingsNodeTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<ArgumentUsingsNode>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void All_additional_usings_get_generated()
    {
        // arrange
        var usings = new[]
            { "Using1.Tests", "A.B.C", "A" };

        var node = new ItemBuilder<ArgumentUsingsNode>()
            .With(p => p.Snippet.Content.Value("$argumentNamespace$"))
            .With(p => p.Ctor.context.AdditionalUsings.Value(usings.ToImmutableHashSet()))
            .Build();

        // act
        var code = node.GetCode();

        // assert
        code.Should().ContainAll(usings);
    }
}
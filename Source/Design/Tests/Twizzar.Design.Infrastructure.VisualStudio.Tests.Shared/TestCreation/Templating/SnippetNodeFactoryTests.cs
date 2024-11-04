using System;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.TestCreation.Templating;

[TestFixture]
public class SnippetNodeFactoryTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<SnippetNodeFactory>()
            .ShouldThrowArgumentNullException();
    }

    [TestCase(SnippetType.Act, typeof(ActNode))]
    [TestCase(SnippetType.ArgumentArrange, typeof(ArgumentArrangeNode))]
    [TestCase(SnippetType.ArgumentUsing, typeof(ArgumentUsingsNode))]
    [TestCase(SnippetType.Arrange, typeof(ArrangeNode))]
    public void Factory_create_the_correct_type(SnippetType type, Type expectedType)
    {
        // arrange
        var sut = new ItemBuilder<SnippetNodeFactory>()
            .Build();

        var snippet = new ItemBuilder<ITemplateSnippet>()
            .With(p => p.Type.Value(type))
            .Build();

        var context = new ItemBuilder<ITemplateContext>()
            .With(p => p.File.GetSingleSnipped__SnippetType.Content.Value(string.Empty))
            .With(p => p.SourceCreationContext.SourceMember.Stub<IMemberDescription>())
            .Build();

        // act
        var snippedNode = sut.Create(snippet, context, Maybe.None());

        // assert
        snippedNode.Should().BeAssignableTo(expectedType);
    }
}
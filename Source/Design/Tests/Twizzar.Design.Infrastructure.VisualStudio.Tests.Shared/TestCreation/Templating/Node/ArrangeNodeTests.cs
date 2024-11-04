using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.TestCommon.TypeDescription.Builders;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.TestCreation.Templating.Node;

[TestFixture]
public class ArrangeNodeTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // arrange
        var context = new ItemBuilder<ITemplateContext>()
            .With(p => p.File.GetSingleSnipped__SnippetType.Content.Value(string.Empty))
            .With(p => p.SourceCreationContext.SourceMember.Stub<IMemberDescription>())
            .Build();

        // assert
        Verify.Ctor<ArrangeNode>()
            .SetupParameter("context", context)
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void Non_static_method_uses_SutArrange()
    {
        // arrange
        var methodDescription = new MethodDescriptionBuilder()
            .WithIsStatic(false)
            .Build();

        // act
        var _ = new ArrangeNodeBuilder(methodDescription, string.Empty)
            .Build(out var context);

        // assert
        context.Verify(p => p.Ctor.context.File.GetSingleSnipped__SnippetType)
            .WhereTypeIs(SnippetType.SutArrange)
            .Called(1);
    }

    [Test]
    public void Properties_uses_SutArrange()
    {
        // arrange
        var methodDescription = new PropertyDescriptionBuilder()
            .Build();

        // act
        var _ = new ArrangeNodeBuilder(methodDescription, string.Empty)
            .Build(out var context);

        // assert
        context.Verify(p => p.Ctor.context.File.GetSingleSnipped__SnippetType)
            .WhereTypeIs(SnippetType.SutArrange)
            .Called(1);
    }

    [Test]
    public void Static_method_returns_empty()
    {
        // arrange
        var methodDescription = new MethodDescriptionBuilder()
            .WithIsStatic(true)
            .Build();

        // act
        var node = new ArrangeNodeBuilder(methodDescription, string.Empty)
            .Build(out var context);

        // assert
        context.Verify(p => p.Ctor.context.File.GetSingleSnipped__SnippetType)
            .WhereTypeIs(SnippetType.SutArrange)
            .Called(0);

        node.GetCode().Should().BeEmpty();
    }

    private class ArrangeNodeBuilder : ItemBuilder<ArrangeNode, ArrangeNodeCustomPaths>
    {
        public ArrangeNodeBuilder(IMemberDescription memberDescription, string content)
        {
            this.With(p => p.Snippet.Content.Value(content));
            this.With(p => p.Ctor.context.SourceCreationContext.SourceMember.Value(memberDescription));
            this.With(p => p.Ctor.context.File.GetSingleSnipped__SnippetType.Content.Value(string.Empty));
        }
    }
}
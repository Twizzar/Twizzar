using System;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.TestCommon.TypeDescription.Builders;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.TestCreation.Templating.Node;

[TestFixture]
public class ArgumentArrangeNodeTests
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
        Verify.Ctor<ArgumentArrangeNode>()
            .SetupParameter("context", context)
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void For_methods_all_parameters_get_arranged()
    {
        // arrange
        var methodDescription = new MethodDescriptionBuilder()
            .WithDeclaredParameter(
                new ParameterDescriptionBuilder()
                    .WithName("a")
                    .WithType(TypeFullName.CreateFromType(typeof(int)))
                    .Build(),
                new ParameterDescriptionBuilder()
                    .WithName("b")
                    .WithType(TypeFullName.CreateFromType(typeof(string)))
                    .Build())
            .Build();

        var node = new ArgumentArrangeNodeBuilder(methodDescription, "$argumentType$$argumentName$")
            .Build();

        // act
        var code = node.GetCode();

        // assert
        code.Should().Be($"Int32a{Environment.NewLine}Stringb{Environment.NewLine}");
    }

    [Test]
    public void For_properties_with_a_setter_propertyFieldSetterArrange_gets_replaced()
    {
        // arrange
        var methodDescription = new PropertyDescriptionBuilder()
            .WithCanWrite(true)
            .WithType(TypeFullName.CreateFromType(typeof(int)))
            .Build();

        var node = new ArgumentArrangeNodeBuilder(methodDescription, "$argumentType$")
            .Build();

        // act
        var code = node.GetCode();

        // assert
        code.Should().Be("Int32");
    }

    [Test]
    public void For_properties_with_a_no_setter_an_empty_string_is_returned()
    {
        // arrange
        var methodDescription = new PropertyDescriptionBuilder()
            .WithCanWrite(false)
            .WithType(TypeFullName.CreateFromType(typeof(int)))
            .Build();

        var node = new ArgumentArrangeNodeBuilder(methodDescription, string.Empty)
            .Build();

        // act
        var code = node.GetCode();

        // assert
        code.Should().BeEmpty();
    }

    private class ArgumentArrangeNodeBuilder : ItemBuilder<ArgumentArrangeNode, ArgumentArrangeNodeCustomPaths>
    {
        public ArgumentArrangeNodeBuilder(IMemberDescription memberDescription, string content)
        {
            this.With(p => p.Snippet.Content.Value(content));
            this.With(p => p.Ctor.context.SourceCreationContext.SourceMember.Value(memberDescription));
            this.With(p => p.Ctor.context.File.GetSingleSnipped__SnippetType.Content.Value(string.Empty));
            this.With(p => p.Ctor.shortTypesConverter.ConvertToShort.Value((name, s) => s));
        }
    }
}
using System;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.TestCommon.TypeDescription.Builders;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.TestCreation.Templating.Node;

[TestFixture]
public class ActNodeTests
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
        Verify.Ctor<ActNode>()
            .SetupParameter("context", context)
            .ShouldThrowArgumentNullException();
    }


    /**
       | Member                               | act                            |
       | ------------------------------------ | ------------------------------ |
       | non-static void method               | void-method-act                |
       | non-static non-void method           | non-void-method-act            |
       | non-static task method               | async-method-act               |
       | non-static task result method        | async-result-method-act        |
       | static extension void method         | void-method-act                |
       | static extension non-void method     | non-void-method-act            |
       | static non-extension void method     | static-void-method-act         |
       | static non-extension non-void method | static-non-void-method-act     |
       | static task method                   | static-result-method-act       |
       | static task result method            | static-async-result-method-act |
     */
    [TestCase(false, typeof(void), SnippetType.VoidMethodAct)]
    [TestCase(false, typeof(int), SnippetType.NonVoidMethodAct)]
    [TestCase(true, typeof(void), SnippetType.StaticVoidMethodAct)]
    [TestCase(true, typeof(int), SnippetType.StaticNonVoidMethodAct)]
    [TestCase(false, typeof(System.Threading.Tasks.Task), SnippetType.AsyncMethodAct)]
    [TestCase(true, typeof(System.Threading.Tasks.Task), SnippetType.StaticAsyncMethodAct)]
    [TestCase(false, typeof(System.Threading.Tasks.Task<>), SnippetType.AsyncResultMethodAct)]
    [TestCase(true, typeof(System.Threading.Tasks.Task<>), SnippetType.StaticAsyncResultMethodAct)]
    public void For_method_snipped_is_created_correctly(bool isStatic, Type type, SnippetType snippedType)
    {
        // arrange
        var typeFullName = type.FullName;

        var methodDesc = new MethodDescriptionBuilder()
            .WithIsStatic(isStatic)
            .WithType(typeFullName)
            .Build();

        new ActNodeBuilder(methodDesc)
            .Build(out var context);

        // assert
        context.Verify(paths => paths.Ctor.context.File.GetSingleSnipped__SnippetType)
            .WhereTypeIs(snippedType)
            .Called(1);
    }

    /**
       | Member                               | act                        |
       | ------------------------------------ | -------------------------- |
       | property/field getter                | property-field-getter-act  |
       | property/field setter                | property-field-setter-act  |
    */
    [TestCase(true, false, SnippetType.PropertyFieldGetterAct)]
    [TestCase(false, true, SnippetType.PropertyFieldSetterAct)]
    [TestCase(true, true, SnippetType.PropertyFieldSetterAct)]
    public void For_property_snipped_is_created_correctly(bool hasGetter, bool hasSetter, SnippetType type)
    {
        // arrange
        var methodDesc = new PropertyDescriptionBuilder()
            .WithCanRead(hasGetter)
            .WithCanWrite(hasSetter)
            .Build();

        new ActNodeBuilder(methodDesc)
            .Build(out var context);

        // assert
        context.Verify(paths => paths.Ctor.context.File.GetSingleSnipped__SnippetType)
            .WhereTypeIs(type)
            .Called(1);
    }

    private class ActNodeBuilder : ItemBuilder<ActNode, ActNodeCustomPaths>
    {
        public ActNodeBuilder(IMemberDescription memberDescription)
        {
            this.With(p => p.Ctor.context.SourceCreationContext.SourceMember.Value(memberDescription));
            this.With(p => p.Ctor.context.File.GetSingleSnipped__SnippetType.Content.Value(string.Empty));
        }
    }
}
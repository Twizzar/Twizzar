using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.TestCommon.TypeDescription.Builders;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.TestCreation.Templating.Node;

[TestFixture]
public class MethodSignatureNodeTests
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
        Verify.Ctor<MethodSignatureNode>()
            .SetupParameter("context", context)
            .ShouldThrowArgumentNullException();
    }

    [TestCase(typeof(Task), SnippetType.AsyncMethodSignature)]
    [TestCase(typeof(Task<int>), SnippetType.AsyncMethodSignature)]
    [TestCase(typeof(int), SnippetType.VoidMethodSignature)]
    [TestCase(typeof(void), SnippetType.VoidMethodSignature)]
    public void If_task_return_AsyncMethodSignature(Type type, SnippetType snippedType)
    {
        // arrange
        var typeFullName = type.FullName;

        var methodDesc = new MethodDescriptionBuilder()
            .WithType(typeFullName)
            .Build();

        // act
        new MethodSignatureNodeBuilder(methodDesc)
            .Build(out var context);

        // assert
        context.Verify(paths => paths.Ctor.context.File.GetSingleSnipped__SnippetType)
            .WhereTypeIs(snippedType)
            .Called(1);
    }

    private class MethodSignatureNodeBuilder : ItemBuilder<MethodSignatureNode, MethodSignatureNodeCustomPaths>
    {
        public MethodSignatureNodeBuilder(IMemberDescription memberDescription)
        {
            this.With(p => p.Ctor.context.SourceCreationContext.SourceMember.Value(memberDescription));
            this.With(p => p.Ctor.context.File.GetSingleSnipped__SnippetType.Content.Value(string.Empty));
        }
    }
}
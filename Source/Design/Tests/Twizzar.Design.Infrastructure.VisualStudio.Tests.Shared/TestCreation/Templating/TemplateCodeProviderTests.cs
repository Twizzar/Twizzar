using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.BaseConfig;
using Twizzar.TestCommon.Builder;
using Twizzar.TestCommon.TypeDescription.Builders;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

[TestFixture]
public class TemplateCodeProviderTests
{
    [Test]
    public async Task IntegrationTest()
    {
        // assert
        var defaultTemplateFile = await new TemplateFileService().GetDefaultFileReaderAsync();

        var fileService = new ItemBuilder<ITemplateFileService>()
            .With(p => p.GetFileReaderAsync.Value(
                Task.FromResult(Maybe.Some(defaultTemplateFile))))
            .Build();

        var fileQuery = new TemplateFileQuery(new TemplateSnippetFactory(), fileService, new BaseConfigParser());
        var file = await fileQuery.GetAsync(@"$solution$\generator-templates.twizzar.config", CancellationToken.None)
            .Map(result => result.GetSuccessUnsafe());

        var methodDescription = new MethodDescriptionBuilder()
            .WithIsStatic(false)
            .WithDeclaredParameter(
                new ParameterDescriptionBuilder()
                    .WithName("param1")
                    .WithType(new TypeFullNameBuilder(typeof(int)).Build())
                    .Build())
            .Build();

        var context = new ItemBuilder<ITemplateContext>()
            .With(p => p.File.Value(file))
            .With(p => p.AdditionalUsings.Value(new[] { "testUsing1", "testUsing2" }.ToImmutableHashSet()))
            .With(p => p.SourceCreationContext.Ctor.Info.Project.Value("SourceProject"))
            .With(p => p.SourceCreationContext.Ctor.Info.Member.Value("SourceMember"))
            .With(p => p.SourceCreationContext.Ctor.Info.File.Value("SourceMember"))
            .With(p => p.SourceCreationContext.Ctor.Info.Namespace.Value("SourceNamespace"))
            .With(p => p.SourceCreationContext.Ctor.Info.Type.Value("TypeUnderTest"))
            .With(p => p.SourceCreationContext.Ctor.SourceType.TypeFullName.Value(new TypeFullNameBuilder("").Build()))
            .With(p => p.TargetCreationContext.Ctor.Info.Project.Value("TargetProject"))
            .With(p => p.TargetCreationContext.Ctor.Info.Member.Value("TargetMember"))
            .With(p => p.TargetCreationContext.Ctor.Info.File.Value("TargetMember"))
            .With(p => p.TargetCreationContext.Ctor.Info.Namespace.Value("TargetNamespace"))
            .With(p => p.TargetCreationContext.Ctor.Info.Type.Value("MyTestClass"))
            .With(p => p.SourceCreationContext.Ctor.SourceMember.Value(methodDescription))
            .Build();

        var sut = new TemplateCodeProvider(
            new TemplateVisitor(new SnippetNodeFactory(new ItemBuilder<IShortTypesConverter>().Build())),
            new VariableReplacer(),
            new VariablesProviderFactory());

        var code = sut.GetCode(SnippetType.File, context);
        Console.WriteLine(code);
    }
}